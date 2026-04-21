using Emgu.CV;
using OmniSight.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace OmniSight.UI.Forms
{
    public class FrmAntiCheatTester : Form
    {
        private readonly FaceAiService _faceAiService;
        private PictureBox picCamera;
        private Label lblStatus, lblDebug;
        private System.Windows.Forms.Timer _timer;

        // Sliders
        private TrackBar tbYawHigh, tbYawLow, tbPitchUp, tbPitchDown, tbSafeZone, tbTooFar, tbTooClose, tbGazeMin, tbGazeMax, tbGazeYMin, tbGazeYMax;

        // CHECKBOXES ĐỂ LỌC LỖI (MỚI)
        private CheckBox chkPose, chkGaze, chkDistance, chkSafeZone;

        private float YawHigh => tbYawHigh.Value / 100f;
        private float YawLow => tbYawLow.Value / 100f;
        private float PitchUp => tbPitchUp.Value / 100f;
        private float PitchDown => tbPitchDown.Value / 100f;
        private float SafeZone => tbSafeZone.Value / 100f;
        private float TooFar => tbTooFar.Value / 1000f;
        private float TooClose => tbTooClose.Value / 100f;
        private float GazeMin => tbGazeMin.Value / 100f;
        private float GazeMax => tbGazeMax.Value / 100f;
        private float GazeYMin => tbGazeYMin.Value / 100f;
        private float GazeYMax => tbGazeYMax.Value / 100f;

        private float _lastPupilX = 0.5f, _lastPupilY = 0.5f;
        private int _frameCount = 0, _violationFrames = 0;

        public FrmAntiCheatTester(FaceAiService faceAiService)
        {
            _faceAiService = faceAiService;
            BuildUI();
            _faceAiService.StartCamera(0);
            _timer = new System.Windows.Forms.Timer { Interval = 100 };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void BuildUI()
        {
            this.Text = "OmniSight - Tinh chỉnh AI (Chế độ tập trung)";
            this.Size = new Size(1150, 950);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormClosing += (s, e) => { _timer?.Stop(); _faceAiService?.StopCamera(); };

            // PANEL TRÁI
            Panel pnlLeft = new Panel { Dock = DockStyle.Left, Width = 450, Padding = new Padding(15), BackColor = Color.FromArgb(240, 242, 245) };
            picCamera = new PictureBox { Dock = DockStyle.Top, Height = 340, BackColor = Color.Black, SizeMode = PictureBoxSizeMode.StretchImage };
            lblStatus = new Label { Dock = DockStyle.Top, Height = 60, Font = new Font("Segoe UI", 20, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, Text = "NORMAL", Margin = new Padding(0, 10, 0, 0) };
            lblDebug = new Label { Dock = DockStyle.Fill, Font = new Font("Consolas", 10), BackColor = Color.FromArgb(30, 30, 30), ForeColor = Color.Lime, Padding = new Padding(10), Margin = new Padding(0, 10, 0, 0) };
            pnlLeft.Controls.AddRange(new Control[] { lblDebug, lblStatus, picCamera });

            // PANEL PHẢI
            FlowLayoutPanel flpSliders = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(20), WrapContents = false, FlowDirection = FlowDirection.TopDown };

            // --- KHU VỰC CHẾ ĐỘ TẬP TRUNG (MỚI) ---
            GroupBox gbFilter = new GroupBox { Text = "🎯 CHẾ ĐỘ TẬP TRUNG (Tích để bắt lỗi)", Width = 550, Height = 80, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            FlowLayoutPanel flpFilter = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, Padding = new Padding(10) };
            chkPose = new CheckBox { Text = "Tư thế đầu", Checked = true, AutoSize = true };
            chkGaze = new CheckBox { Text = "Liếc mắt", Checked = true, AutoSize = true };
            chkDistance = new CheckBox { Text = "Khoảng cách", Checked = true, AutoSize = true };
            chkSafeZone = new CheckBox { Text = "Lệch tâm", Checked = true, AutoSize = true };
            flpFilter.Controls.AddRange(new Control[] { chkPose, chkGaze, chkDistance, chkSafeZone });
            gbFilter.Controls.Add(flpFilter);
            flpSliders.Controls.Add(gbFilter);

            Button btnReset = new Button { Text = "↺ RESET", Width = 80, Height = 35, FlatStyle = FlatStyle.Flat, BackColor = Color.Gray, ForeColor = Color.White };
            btnReset.Click += (s, e) => ResetDefaults();
            Button btnCopy = new Button { Text = "📋 COPY CẤU HÌNH", Width = 150, Height = 35, FlatStyle = FlatStyle.Flat, BackColor = Color.DodgerBlue, ForeColor = Color.White };
            btnCopy.Click += (s, e) => CopyValues();
            flpSliders.Controls.Add(new FlowLayoutPanel { AutoSize = true, Controls = { btnReset, btnCopy } });

            // --- CÁC NHÓM SLIDER ---
            flpSliders.Controls.Add(new Label { Text = "🔄 TƯ THẾ ĐẦU (YAW/PITCH)", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.DarkBlue, Margin = new Padding(0, 15, 0, 5) });
            (tbYawHigh, _) = CreateSliderGroup(flpSliders, "Quay TRÁI >", 80, 200, 140);
            (tbYawLow, _) = CreateSliderGroup(flpSliders, "Quay PHẢI <", 40, 100, 70);
            (tbPitchUp, _) = CreateSliderGroup(flpSliders, "Ngửa LÊN <", 30, 100, 65);
            (tbPitchDown, _) = CreateSliderGroup(flpSliders, "Cúi XUỐNG >", 100, 250, 135);

            flpSliders.Controls.Add(new Label { Text = "👁️ LIẾC MẮT (X/Y)", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.Purple, Margin = new Padding(0, 15, 0, 5) });
            (tbGazeMin, _) = CreateSliderGroup(flpSliders, "Liếc TRÁI X <", 10, 50, 25);
            (tbGazeMax, _) = CreateSliderGroup(flpSliders, "Liếc PHẢI X >", 50, 90, 75);
            (tbGazeYMin, _) = CreateSliderGroup(flpSliders, "Liếc LÊN Y <", 10, 50, 30);
            (tbGazeYMax, _) = CreateSliderGroup(flpSliders, "Liếc XUỐNG Y >", 50, 90, 70);

            flpSliders.Controls.Add(new Label { Text = "📐 VÙNG AN TOÀN & KHOẢNG CÁCH", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.Gray, Margin = new Padding(0, 15, 0, 5) });
            (tbSafeZone, _) = CreateSliderGroup(flpSliders, "Lệch tâm %", 10, 50, 25);
            (tbTooFar, _) = CreateSliderGroup(flpSliders, "Quá XA %", 10, 100, 30);
            (tbTooClose, _) = CreateSliderGroup(flpSliders, "Quá GẦN %", 30, 90, 60);

            this.Controls.Add(flpSliders); this.Controls.Add(pnlLeft);
        }

        private (TrackBar, Label) CreateSliderGroup(FlowLayoutPanel parent, string name, int min, int max, int val, bool isFloat = true)
        {
            Panel group = new Panel { Width = 550, Height = 60 };
            Label title = new Label { Text = name + ": " + (isFloat ? (val / 100f).ToString("F2") : val.ToString()), Dock = DockStyle.Top, AutoSize = true, Font = new Font("Segoe UI", 9) };
            TrackBar tb = new TrackBar { Dock = DockStyle.Bottom, Minimum = min, Maximum = max, Value = val, TickStyle = TickStyle.None, Height = 25 };
            tb.ValueChanged += (s, e) => title.Text = name + ": " + (isFloat ? (tb.Value / 100f).ToString("F2") : tb.Value.ToString());
            group.Controls.AddRange(new Control[] { title, tb });
            parent.Controls.Add(group);
            return (tb, title);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            using var frame = _faceAiService.GetFrame();
            if (frame == null || frame.IsEmpty) return;
            picCamera.Image?.Dispose(); picCamera.Image = frame.ToBitmap();

            string statusMsg = "✓ BÌNH THƯỜNG";
            Color statusBg = Color.LightGreen;

            var result = AnalyzeWithFilter(frame);
            if (result != "Bình thường")
            {
                statusMsg = "⚠️ " + result.ToUpper();
                statusBg = Color.Tomato;
                _violationFrames++;
            }
            _frameCount++;

            lblStatus.Text = statusMsg;
            lblStatus.BackColor = statusBg;
            lblDebug.Text = $"REALTIME DATA:\n" +
                            $"------------------------------\n" +
                            $"Pupil X: {_lastPupilX:F3} | Pupil Y: {_lastPupilY:F3}\n" +
                            $"------------------------------\n" +
                            $"Frames: {_frameCount} | Lỗi: {_violationFrames}\n" +
                            $"Tỷ lệ lỗi: {(_frameCount > 0 ? (_violationFrames * 100f / _frameCount) : 0):F1}%\n" +
                            $"Kết quả: {result}";
        }

        private string AnalyzeWithFilter(Mat frame)
        {
            try
            {
                var detectorField = typeof(FaceAiService).GetField("_faceDetector", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var detector = (Emgu.CV.FaceDetectorYN)detectorField.GetValue(_faceAiService);
                detector.InputSize = new Size(frame.Width, frame.Height);
                using Mat faces = new Mat(); detector.Detect(frame, faces);

                if (faces.IsEmpty || faces.Rows == 0) return "Mất mặt";
                float[] d = new float[15]; Marshal.Copy(faces.Row(0).DataPointer, d, 0, 15);
                float x = d[0], y = d[1], w = d[2], h = d[3], rx = d[4], ry = d[5], lx = d[6], ly = d[7], nx = d[8], ny = d[9], mrx = d[10], mry = d[11], mlx = d[12], mly = d[13];

                // 1. KHOẢNG CÁCH (Chỉ bắt lỗi nếu tích Checkbox)
                if (chkDistance.Checked)
                {
                    float areaRatio = (w * h) / (float)(frame.Width * frame.Height);
                    if (areaRatio < TooFar) return "Quá xa";
                    if (areaRatio > TooClose) return "Quá gần";
                }

                // 2. LỆCH TÂM
                if (chkSafeZone.Checked)
                {
                    if (Math.Abs((x + w / 2) - frame.Width / 2f) > frame.Width * SafeZone) return "Lệch tâm";
                }

                // 3. TƯ THẾ ĐẦU (Quay mặt / Cúi / Ngửa)
                if (chkPose.Checked)
                {
                    float dNR = (float)Math.Sqrt(Math.Pow(nx - rx, 2) + Math.Pow(ny - ry, 2));
                    float dNL = (float)Math.Sqrt(Math.Pow(nx - lx, 2) + Math.Pow(ny - ly, 2));
                    float yaw = dNR / (dNL + 0.001f);
                    if (yaw > YawHigh) return "Quay trái";
                    if (yaw < YawLow) return "Quay phải";

                    float pitch = (ny - (ry + ly) / 2) / ((mry + mly) / 2 - ny + 0.001f);
                    if (pitch < PitchUp) return "Ngửa lên";
                    if (pitch > PitchDown) return "Cúi xuống";
                }

                // 4. LIẾC MẮT
                int ew = (int)(w * 0.22f), eh = (int)(w * 0.12f);
                int ex = (int)(lx - ew / 2f), ey = (int)(ly - eh / 2f);
                if (ex >= 0 && ey >= 0 && ex + ew <= frame.Width && ey + eh <= frame.Height)
                {
                    using Mat eye = new Mat(frame, new Rectangle(ex, ey, ew, eh));
                    using Mat gray = new Mat();
                    Emgu.CV.CvInvoke.CvtColor(eye, gray, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
                    double minV = 0, maxV = 0; Point minL = new Point(), maxL = new Point();
                    Emgu.CV.CvInvoke.MinMaxLoc(gray, ref minV, ref maxV, ref minL, ref maxL);

                    _lastPupilX = (float)minL.X / ew; _lastPupilY = (float)minL.Y / eh;

                    if (chkGaze.Checked)
                    {
                        if (_lastPupilX < GazeMin) return "Mắt liếc trái";
                        if (_lastPupilX > GazeMax) return "Mắt liếc phải";
                        if (_lastPupilY < GazeYMin) return "Mắt liếc lên";
                        if (_lastPupilY > GazeYMax) return "Mắt liếc xuống";
                    }
                }

                return "Bình thường";
            }
            catch { return "Bình thường"; }
        }

        private void ResetDefaults()
        {
            tbYawHigh.Value = 140; tbYawLow.Value = 70; tbPitchUp.Value = 65; tbPitchDown.Value = 135;
            tbGazeMin.Value = 25; tbGazeMax.Value = 75; tbGazeYMin.Value = 30; tbGazeYMax.Value = 70;
            tbSafeZone.Value = 25; tbTooFar.Value = 30; tbTooClose.Value = 60;
        }

        private void CopyValues()
        {
            string code = $"// --- CẬP NHẬT FaceAiService ---\n" +
                          $"float yawHigh = {YawHigh:F2}f; float yawLow = {YawLow:F2}f;\n" +
                          $"float pitchUp = {PitchUp:F2}f; float pitchDown = {PitchDown:F2}f;\n" +
                          $"float gazeMinX = {GazeMin:F2}f; float gazeMaxX = {GazeMax:F2}f;\n" +
                          $"float gazeMinY = {GazeYMin:F2}f; float gazeMaxY = {GazeYMax:F2}f;";
            Clipboard.SetText(code); MessageBox.Show("Đã copy cấu hình!");
        }
    }
}