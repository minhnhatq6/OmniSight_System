using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using Emgu.CV;
using NAudio.Wave;
using OmniSight.Core.Entities;
using OmniSight.Data;

namespace OmniSight.Services
{
    // AntiCheatService.cs - Thêm lock để tránh đúp, fix focus detection
    public class AntiCheatService : IDisposable
    {
        private readonly OmniSightDbContext _db;
        private readonly FaceAiService _faceAiService;

        private WaveInEvent _waveIn;
        private float _currentMicVolume = 0;
        private bool _isMonitoring = false;
        private int _examResultId;
        private IntPtr _examFormHandle;
        private Action<string> _onViolationAlert;

        private const int SCAN_INTERVAL_MS = 1000;
        private int _talkingDurationMs = 0;
        private int _badPoseDurationMs = 0;
        private int _lookUpCount = 0;
        private int _lookDownCount = 0;
        private int _glanceCount = 0;
        private bool _isCurrentlyTabbedOut = false;
        private const string ImgBB_API_KEY = "8e336e9eee38de601c4a73eae1093253";

        // FIX 1: Thêm lock để tránh 2 scan chạy song song
        private bool _isScanning = false;

        // FIX 2: Đếm số handle hợp lệ của app (bao gồm MessageBox)
        private readonly HashSet<IntPtr> _ownedHandles = new HashSet<IntPtr>();

        private FaceStatus _previousFaceStatus = FaceStatus.Normal;
        public bool IsShowingAlert { get; set; } = false;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public AntiCheatService(OmniSightDbContext db, FaceAiService faceAiService)
        {
            _db = db;
            _faceAiService = faceAiService;
        }

        public void StartMonitoring(IntPtr examFormHandle, Action<string> onViolationAlert, int examResultId)
        {
            _examFormHandle = examFormHandle;
            _onViolationAlert = onViolationAlert;
            _examResultId = examResultId;
            _isMonitoring = true;

            try
            {
                _waveIn = new WaveInEvent();
                _waveIn.WaveFormat = new WaveFormat(44100, 1);
                _waveIn.DataAvailable += OnMicDataAvailable;
                _waveIn.StartRecording();
            }
            catch { }
        }

        // FIX 2: Thêm handle của MessageBox vào danh sách hợp lệ
        public void RegisterOwnedHandle(IntPtr handle)
        {
            _ownedHandles.Add(handle);
        }

        public void UnregisterOwnedHandle(IntPtr handle)
        {
            _ownedHandles.Remove(handle);
        }

        private void OnMicDataAvailable(object sender, WaveInEventArgs e)
        {
            short[] values = new short[e.BytesRecorded / 2];
            Buffer.BlockCopy(e.Buffer, 0, values, 0, e.BytesRecorded);
            double sum = 0;
            for (int i = 0; i < values.Length; i++) sum += values[i] * values[i];
            _currentMicVolume = (float)(Math.Sqrt(sum / values.Length) / 32768.0);
        }

        public async Task ScanForViolationsAsync()
        {
            // FIX 1: Nếu đang scan rồi thì bỏ qua tick này hoàn toàn
            if (!_isMonitoring || IsShowingAlert || _isScanning) return;

            _isScanning = true;
            try
            {
                // FIX 2: Kiểm tra focus bằng ProcessId thay vì Handle
                // Handle của MessageBox khác với form nhưng cùng Process → không báo vi phạm
                IntPtr activeWindow = GetForegroundWindow();
                if (activeWindow != IntPtr.Zero)
                {
                    GetWindowThreadProcessId(activeWindow, out uint activePid);
                    GetWindowThreadProcessId(_examFormHandle, out uint examPid);

                    bool isSameProcess = (activePid == examPid);

                    if (!isSameProcess)
                    {
                        _isCurrentlyTabbedOut = true;
                    }
                    else if (_isCurrentlyTabbedOut && isSameProcess)
                    {
                        _isCurrentlyTabbedOut = false;
                        await LogAndSaveViolationAsync("Phát hiện chuyển Tab/Mở phần mềm khác", null);
                        return;
                    }
                }

                // Speech detection
                if (_currentMicVolume > 0.1f)
                {
                    _talkingDurationMs += SCAN_INTERVAL_MS;
                    if (_talkingDurationMs >= 10000)
                    {
                        await LogAndSaveViolationAsync("Có tiếng ồn/nói chuyện liên tục quá 10 giây", null);
                        _talkingDurationMs = 0;
                    }
                }
                else _talkingDurationMs = 0;

                // Face AI detection
                using (var frame = _faceAiService.GetFrame())
                {
                    if (frame != null && !frame.IsEmpty)
                    {
                        FaceStatus currentStatus = _faceAiService.AnalyzeFaceBehavior(frame);

                        if (currentStatus != FaceStatus.Normal)
                        {
                            _badPoseDurationMs += SCAN_INTERVAL_MS;
                            if (_badPoseDurationMs >= 3000)
                            {
                                string reason = currentStatus switch
                                {
                                    FaceStatus.NoFace => "Mất khuôn mặt khỏi Camera",
                                    FaceStatus.MultipleFaces => "Có người lạ trợ giúp",
                                    FaceStatus.TooFar => "Ngồi quá xa Camera",
                                    FaceStatus.TooClose => "Dí sát mặt vào màn hình",
                                    FaceStatus.OutOfSafeZone => "Nép ra rìa khung hình Camera",
                                    FaceStatus.LookingUp => "Ngước nhìn lên trên liên tục",
                                    FaceStatus.LookingDown => "Cúi gầm mặt xuống bàn liên tục",
                                    FaceStatus.LookingLeft or FaceStatus.LookingRight => "Quay mặt sang bên liên tục",
                                    FaceStatus.Glancing => "Liếc mắt nhìn tài liệu liên tục",
                                    _ => "Hành vi bất thường"
                                };
                                await LogAndSaveViolationAsync($"{reason} (Kéo dài quá 3s)", frame);
                                _badPoseDurationMs = 0;
                            }
                        }
                        else
                        {
                            _badPoseDurationMs = 0;
                        }

                        if (currentStatus != FaceStatus.Normal && currentStatus != _previousFaceStatus)
                        {
                            if (currentStatus == FaceStatus.LookingDown)
                            {
                                _lookDownCount++;
                                if (_lookDownCount > 3)
                                {
                                    await LogAndSaveViolationAsync("Nhìn xuống gầm bàn/điện thoại LẶP LẠI QUÁ 3 LẦN", frame);
                                    _lookDownCount = 0;
                                }
                            }
                            else if (currentStatus == FaceStatus.LookingUp)
                            {
                                _lookUpCount++;
                                if (_lookUpCount > 3)
                                {
                                    await LogAndSaveViolationAsync("Ngước nhìn tài liệu trên cao LẶP LẠI QUÁ 3 LẦN", frame);
                                    _lookUpCount = 0;
                                }
                            }
                            else if (currentStatus == FaceStatus.LookingLeft ||
                                     currentStatus == FaceStatus.LookingRight ||
                                     currentStatus == FaceStatus.Glancing)
                            {
                                _glanceCount++;
                                if (_glanceCount > 3)
                                {
                                    await LogAndSaveViolationAsync("Liếc mắt/Quay ngang LẶP LẠI QUÁ 3 LẦN", frame);
                                    _glanceCount = 0;
                                }
                            }
                        }

                        _previousFaceStatus = currentStatus;
                    }
                }
            }
            finally
            {
                // FIX 1: Luôn mở khóa dù có lỗi hay không
                _isScanning = false;
            }
        }

        private async Task LogAndSaveViolationAsync(string violationType, Mat frame)
        {
            string imageUrl = "";
            if (frame != null)
            {
                using (Bitmap bmp = frame.ToBitmap())
                {
                    imageUrl = await UploadToImgBBAsync(bmp);
                }
            }

            var log = new ViolationLog
            {
                ResultId = _examResultId,
                ViolationType = violationType,
                ImageUrl = imageUrl,
                Timestamp = DateTime.Now,
                Status = "Nghi vấn"
            };
            _db.ViolationLogs.Add(log);
            await _db.SaveChangesAsync();

            _onViolationAlert?.Invoke(violationType);
        }

        private async Task<string> UploadToImgBBAsync(Bitmap bmp)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Jpeg);
                    string base64Image = Convert.ToBase64String(ms.ToArray());
                    using (HttpClient client = new HttpClient())
                    {
                        var content = new MultipartFormDataContent();
                        content.Add(new StringContent(ImgBB_API_KEY), "key");
                        content.Add(new StringContent(base64Image), "image");
                        var response = await client.PostAsync("https://api.imgbb.com/1/upload", content);
                        if (response.IsSuccessStatusCode)
                        {
                            using (JsonDocument doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync()))
                            {
                                return doc.RootElement.GetProperty("data").GetProperty("url").GetString();
                            }
                        }
                    }
                }
            }
            catch { }
            return "";
        }

        public void StopMonitoring()
        {
            _isMonitoring = false;
            if (_waveIn != null)
            {
                _waveIn.StopRecording();
                _waveIn.Dispose();
                _waveIn = null;
            }
        }

        public void Dispose() => StopMonitoring();
    }
}