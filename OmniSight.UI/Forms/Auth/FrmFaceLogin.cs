using Emgu.CV;
using Emgu.CV.Structure;
using MaterialSkin.Controls;
using OmniSight.Services;
using System;
using System.Windows.Forms;
using OmniSight.Core.Entities;
using System.Globalization;
namespace OmniSight.UI.Forms.Auth
{
    public partial class FrmFaceLogin : MaterialForm
    {
        private bool _isProcessing = false;
        private readonly FaceAiService _faceAiService;
        private readonly AuthService _authService;
        private System.Windows.Forms.Timer _scanTimer;
        public List<User> MatchedUsers { get; set; } = new List<User>();
        public FrmFaceLogin(FaceAiService faceAiService, AuthService authService)
        {
            InitializeComponent();
            _faceAiService = faceAiService;
            _authService = authService;

            _scanTimer = new System.Windows.Forms.Timer { Interval = 100 }; // Quét mỗi 0.1 giây
            _scanTimer.Tick += ScanTimer_Tick;
        }

        private void FrmFaceLogin_Load(object sender, EventArgs e)
        {
            try
            {
                // 1. Khởi tạo Model AI ngay tại đây (vì MainForm chưa chạy)
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string detector = Path.Combine(baseDir, "Models", "face_detection_yunet_2023mar.onnx");
                string recognizer = Path.Combine(baseDir, "Models", "face_recognition_sface_2021dec.onnx");

                _faceAiService.InitializeModels(detector, recognizer);

                // 2. Bật Camera
                if (_faceAiService.StartCamera())
                {
                    _scanTimer.Start();
                }
                else
                {
                    MessageBox.Show("Không mở được camera!");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khởi tạo AI tại Login: " + ex.Message);
                this.Close();
            }
        }

        private async void ScanTimer_Tick(object? sender, EventArgs e)
        {
            if (_isProcessing) return;

            using (var frame = _faceAiService.GetFrame())
            {
                if (frame == null || frame.IsEmpty) return;
                picFace.Image = frame.ToBitmap();

                var embedding = _faceAiService.ExtractEmbedding(frame);
                if (embedding != null)
                {
                    try
                    {
                        _isProcessing = true;

                        // --- THAY ĐỔI QUAN TRỌNG TẠI ĐÂY ---
                        // Không gọi LoginWithFaceAsync nữa, mà gọi FindMatchingUsersAsync
                        var matches = await _authService.FindMatchingUsersAsync(embedding);

                        if (matches != null && matches.Count > 0)
                        {
                            _scanTimer.Stop();
                            _faceAiService.StopCamera();

                            // Lưu danh sách vào biến MatchedUsers để FrmLogin sử dụng
                            this.MatchedUsers = matches;

                            this.DialogResult = DialogResult.OK; // Báo cho FrmLogin là đã tìm thấy người
                            this.Close();
                        }
                    }
                    catch (Exception ex) { /* Log lỗi */ }
                    finally { _isProcessing = false; }
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _scanTimer.Stop();
            _faceAiService.StopCamera();
            base.OnFormClosing(e);
        }
    }
}