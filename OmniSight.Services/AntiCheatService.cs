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
        // AntiCheatService.cs
        // Thêm các biến này vào đầu class:

        // SMOOTHING BUFFER: Lưu lịch sử N frame gần nhất để tránh false positive 1 frame lẻ
        private const int SMOOTH_WINDOW = 3; // Cần 3 frame liên tiếp cùng trạng thái mới tính
        private Queue<FaceStatus> _faceStatusHistory = new Queue<FaceStatus>();

        // Đếm thời gian giữ trạng thái xấu LIÊN TỤC (không bị reset bởi 1 frame Normal lẻ)
        private int _consecutiveNormalFrames = 0;
        private const int NORMAL_GRACE_FRAMES = 2; // Cần 2 frame Normal liên tiếp mới reset timer
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

                // Thay toàn bộ phần "// 3. FACE AI DETECTION" trong ScanForViolationsAsync:

                // 3. FACE AI DETECTION
                using (var frame = _faceAiService.GetFrame())
                {
                    if (frame != null && !frame.IsEmpty)
                    {
                        FaceStatus currentStatus = _faceAiService.AnalyzeFaceBehavior(frame);

                        // BƯỚC 1: Thêm vào buffer, giữ tối đa SMOOTH_WINDOW phần tử
                        _faceStatusHistory.Enqueue(currentStatus);
                        if (_faceStatusHistory.Count > SMOOTH_WINDOW)
                            _faceStatusHistory.Dequeue();

                        // BƯỚC 2: Lấy trạng thái "đa số" trong buffer (majority vote)
                        // Nếu 2/3 frame gần nhất cùng 1 trạng thái xấu → mới tính là xấu thật
                        FaceStatus smoothedStatus = GetMajorityStatus(_faceStatusHistory);

                        // BƯỚC 3: Đếm thời gian liên tục
                        if (smoothedStatus != FaceStatus.Normal)
                        {
                            _consecutiveNormalFrames = 0; // Reset grace counter
                            _badPoseDurationMs += SCAN_INTERVAL_MS;

                            // Phạt sau 3 giây liên tục
                            if (_badPoseDurationMs >= 3000)
                            {
                                string reason = smoothedStatus switch
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
                            // GRACE PERIOD: Cần N frame Normal liên tiếp mới reset timer
                            // Tránh trường hợp 1 frame Normal lẻ reset hết bộ đếm
                            _consecutiveNormalFrames++;
                            if (_consecutiveNormalFrames >= NORMAL_GRACE_FRAMES)
                            {
                                _badPoseDurationMs = 0;
                            }
                        }

                        // BƯỚC 4: Đếm số lần lặp lại (chỉ tính khi transition từ Normal → Xấu)
                        // Dùng smoothedStatus thay vì currentStatus thô
                        if (smoothedStatus != FaceStatus.Normal && smoothedStatus != _previousFaceStatus
                            && _previousFaceStatus == FaceStatus.Normal) // Chỉ tính khi VỪA chuyển từ Normal sang xấu
                        {
                            if (smoothedStatus == FaceStatus.LookingDown)
                            {
                                _lookDownCount++;
                                if (_lookDownCount > 5) // Tăng từ 3 → 5 lần
                                {
                                    await LogAndSaveViolationAsync("Nhìn xuống gầm bàn/điện thoại LẶP LẠI QUÁ 5 LẦN", frame);
                                    _lookDownCount = 0;
                                }
                            }
                            else if (smoothedStatus == FaceStatus.LookingUp)
                            {
                                _lookUpCount++;
                                if (_lookUpCount > 5)
                                {
                                    await LogAndSaveViolationAsync("Ngước nhìn tài liệu trên cao LẶP LẠI QUÁ 5 LẦN", frame);
                                    _lookUpCount = 0;
                                }
                            }
                            else if (smoothedStatus == FaceStatus.LookingLeft ||
                                     smoothedStatus == FaceStatus.LookingRight ||
                                     smoothedStatus == FaceStatus.Glancing)
                            {
                                _glanceCount++;
                                if (_glanceCount > 5)
                                {
                                    await LogAndSaveViolationAsync("Liếc mắt/Quay ngang LẶP LẠI QUÁ 5 LẦN", frame);
                                    _glanceCount = 0;
                                }
                            }
                        }

                        _previousFaceStatus = smoothedStatus; // Dùng smoothed thay vì raw
                    }
                }
            }
            finally
            {
                // FIX 1: Luôn mở khóa dù có lỗi hay không
                _isScanning = false;
            }
        }
        private FaceStatus GetMajorityStatus(Queue<FaceStatus> history)
        {
            if (history.Count == 0) return FaceStatus.Normal;

            // Đếm tần suất mỗi trạng thái
            var counts = new Dictionary<FaceStatus, int>();
            foreach (var s in history)
            {
                if (!counts.ContainsKey(s)) counts[s] = 0;
                counts[s]++;
            }

            // Tìm trạng thái xuất hiện nhiều nhất
            FaceStatus majority = FaceStatus.Normal;
            int maxCount = 0;
            foreach (var kv in counts)
            {
                if (kv.Value > maxCount)
                {
                    maxCount = kv.Value;
                    majority = kv.Key;
                }
            }

            // Chỉ trả về trạng thái xấu nếu chiếm ít nhất 2/3 số frame trong buffer
            // Nếu không đủ đa số → coi là Normal (nhiễu)
            float threshold = history.Count * 0.6f; // 60% frame phải cùng trạng thái
            if (maxCount >= threshold && majority != FaceStatus.Normal)
                return majority;

            return FaceStatus.Normal;
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