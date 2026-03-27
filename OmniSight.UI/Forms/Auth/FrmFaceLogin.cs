using Emgu.CV;
using Emgu.CV.Structure;
using MaterialSkin.Controls;
using OmniSight.Services;
using System;
using System.Windows.Forms;

namespace OmniSight.UI.Forms.Auth
{
    public partial class FrmFaceLogin : MaterialForm
    {
        private readonly FaceAiService _faceAiService;
        private readonly AuthService _authService;
        private System.Windows.Forms.Timer _scanTimer;

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

        private async void ScanTimer_Tick(object sender, EventArgs e)
        {
            using (var frame = _faceAiService.GetFrame())
            {
                if (frame == null || frame.IsEmpty) return;

                picFace.Image = frame.ToBitmap();

                // Trích xuất vector từ camera
                var embedding = _faceAiService.ExtractEmbedding(frame);
                if (embedding != null)
                {
                    // So khớp với Database
                    var result = await _authService.LoginWithFaceAsync(embedding);
                    if (result.success)
                    {
                        _scanTimer.Stop();
                        _faceAiService.StopCamera();
                        this.DialogResult = DialogResult.OK; // Báo thành công
                        this.Close();
                    }
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