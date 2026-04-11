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


        // Thêm các biến đếm vào đầu class AntiCheatService
        private const int SCAN_INTERVAL_MS = 1000;
        private int _talkingDurationMs = 0;
        private int _badPoseDurationMs = 0; // Đếm thời gian giữ nguyên tư thế xấu

        // BỘ ĐẾM SỐ LẦN VI PHẠM TỨC THỜI (Tích lũy)
        private int _lookUpCount = 0;
        private int _lookDownCount = 0;
        private int _glanceCount = 0; // Đếm liếc ngang
        private bool _isCurrentlyTabbedOut = false;
        private const string ImgBB_API_KEY = "8e336e9eee38de601c4a73eae1093253"; // Nhớ đổi API Key
                                                                                 //
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        // Thêm cờ này để tạm dừng Alt-Tab khi hiện MessageBox
        private FaceStatus _previousFaceStatus = FaceStatus.Normal;
        public bool IsShowingAlert { get; set; } = false;   
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
            if (!_isMonitoring || IsShowingAlert) return;

            // 1. FOCUS TRACKING (Alt-Tab) - Bắt lỗi ngay lập tức
            IntPtr activeWindow = GetForegroundWindow();
            if (activeWindow != _examFormHandle && activeWindow != IntPtr.Zero)
            {
                _isCurrentlyTabbedOut = true;
            }
            else if (_isCurrentlyTabbedOut && activeWindow == _examFormHandle)
            {
                _isCurrentlyTabbedOut = false;
                await LogAndSaveViolationAsync("Phát hiện chuyển Tab/Mở phần mềm khác", null);
                return;
            }

            // 2. SPEECH DETECTION (> 10s)
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

            // 3. FACE AI DETECTION
            using (var frame = _faceAiService.GetFrame())
            {
                if (frame != null && !frame.IsEmpty)
                {
                    // Gọi hàm phân tích hành vi siêu nhạy vừa viết
                    FaceStatus currentStatus = _faceAiService.AnalyzeFaceBehavior(frame);

                    // A. KIỂM TRA LỖI KÉO DÀI LIÊN TỤC 3 GIÂY
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
                                FaceStatus.Glancing => "Liếc mắt nhìn tài liệu liên tục", // <--- THÊM DÒNG NÀY
                                _ => "Hành vi bất thường"
                            };
                            await LogAndSaveViolationAsync($"{reason} (Kéo dài quá 3s)", frame);
                            _badPoseDurationMs = 0; // Reset để không báo liên tục
                        }
                    }
                    else
                    {
                        _badPoseDurationMs = 0; // Trở lại bình thường thì reset bộ đếm thời gian
                    }

                    // B. KIỂM TRA LỖI LẶP LẠI (ĐẾM SỐ LẦN)
                    // Nếu trạng thái thay đổi từ Bình thường -> Xấu (Hành động vừa mới xảy ra)
                    if (currentStatus != FaceStatus.Normal && currentStatus != _previousFaceStatus)
                    {
                        if (currentStatus == FaceStatus.LookingDown)
                        {
                            _lookDownCount++;
                            if (_lookDownCount > 3)
                            {
                                await LogAndSaveViolationAsync("Nhìn xuống gầm bàn/điện thoại LẶP LẠI QUÁ 3 LẦN", frame);
                                _lookDownCount = 0; // Đã phạt thì reset lại
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
                                 currentStatus == FaceStatus.Glancing) // <--- THÊM TRẠNG THÁI GLANCING
                        {
                            _glanceCount++;
                            if (_glanceCount > 3)
                            {
                                await LogAndSaveViolationAsync("Liếc mắt/Quay ngang LẶP LẠI QUÁ 3 LẦN", frame);
                                _glanceCount = 0;
                            }
                        }
                    }

                    // Cập nhật trạng thái cũ
                    _previousFaceStatus = currentStatus;
                }
            }
        }

        private async Task LogAndSaveViolationAsync(string violationType, Mat frame)
        {
            string imageUrl = "";
            if (frame != null)
            {
                using (Bitmap bmp = frame.ToBitmap()) { imageUrl = await UploadToImgBBAsync(bmp); }
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

            // Gửi thông điệp cảnh báo về UI
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