using Emgu.CV;
using MaterialSkin.Controls;
using Microsoft.Extensions.DependencyInjection;
using OmniSight.Core.Entities;
using OmniSight.Data;
using OmniSight.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OmniSight.UI.Forms
{
    public class FrmTakeExam : Form
    {
        // 1. CÁC BIẾN DỮ LIỆU VÀ SERVICE
        private readonly Exam _exam;
        private readonly ExamResult _examResult;
        private readonly ExamService _examService;
        private readonly AntiCheatService _antiCheatService;
        private FaceAiService _faceAiService; // Khai báo ở đây để dùng được trong toàn class

        // 2. BIẾN QUẢN LÝ BÀI THI
        private List<Question> _questions;
        private Dictionary<int, string> _answers = new Dictionary<int, string>();
        private int _currentQuestionIndex = 0;
        private System.Windows.Forms.Timer _timerCountdown;
        private int _remainingSeconds;

        // 3. BIẾN UI BÀI THI
        private MaterialLabel lblTimer;
        private MaterialLabel lblProgress;
        private MaterialLabel lblQuestion;
        private MaterialRadioButton rbOptionA, rbOptionB, rbOptionC, rbOptionD;
        private MaterialButton btnPrevious, btnNext, btnSubmit;
        private FlowLayoutPanel flpQuestions;

        // 4. BIẾN GIÁM SÁT GIAN LẬN
        private System.Windows.Forms.Timer _antiCheatTimer;
        private System.Windows.Forms.Timer _cameraUiTimer;
        private PictureBox _picCameraFeed;
        private Label _lblMonitoringStatus;
        private int _violationCount = 0;
        private bool _isForceSubmitting = false;
        private bool _isProcessingViolation = false;
        public FrmTakeExam(Exam exam, ExamResult examResult, ExamService examService, AntiCheatService antiCheatService)
        {
            _exam = exam;
            _examResult = examResult;
            _examService = examService;
            _antiCheatService = antiCheatService;
            _remainingSeconds = exam.DurationMinutes * 60;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = $"Làm bài: {_exam.Title}";
            this.Width = 1000;
            this.Height = 700;
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormClosing += FrmTakeExam_FormClosing;

            // Timer label
            lblTimer = new MaterialLabel
            {
                Text = "Thời gian: --:--",
                Font = new Font("Roboto", 14, FontStyle.Bold),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(800, 20)
            };
            this.Controls.Add(lblTimer);

            // Progress label
            lblProgress = new MaterialLabel
            {
                Text = "Câu 1/??",
                Font = new Font("Roboto", 12),
                Location = new Point(20, 20)
            };
            this.Controls.Add(lblProgress);

            // Question label
            lblQuestion = new MaterialLabel
            {
                Text = "Loading...",
                Font = new Font("Roboto", 12, FontStyle.Bold),
                AutoSize = true,
                MaximumSize = new Size(900, 0),
                Location = new Point(20, 70),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(lblQuestion);

            // Radio buttons for options
            rbOptionA = new MaterialRadioButton { Text = "A. ", Location = new Point(40, 130), AutoSize = true };
            rbOptionB = new MaterialRadioButton { Text = "B. ", Location = new Point(40, 170), AutoSize = true };
            rbOptionC = new MaterialRadioButton { Text = "C. ", Location = new Point(40, 210), AutoSize = true };
            rbOptionD = new MaterialRadioButton { Text = "D. ", Location = new Point(40, 250), AutoSize = true };

            this.Controls.Add(rbOptionA);
            this.Controls.Add(rbOptionB);
            this.Controls.Add(rbOptionC);
            this.Controls.Add(rbOptionD);

            // Question list panel
            flpQuestions = new FlowLayoutPanel
            {
                Location = new Point(20, 310),
                Size = new Size(940, 300),
                AutoScroll = true,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(flpQuestions);

            // Buttons
            btnPrevious = new MaterialButton
            {
                Text = "← Câu trước",
                Location = new Point(20, 620),
                Width = 120,
                Height = 40
            };
            btnPrevious.Click += BtnPrevious_Click;
            this.Controls.Add(btnPrevious);

            btnNext = new MaterialButton
            {
                Text = "Câu sau →",
                Location = new Point(150, 620),
                Width = 120,
                Height = 40
            };
            btnNext.Click += BtnNext_Click;
            this.Controls.Add(btnNext);

            btnSubmit = new MaterialButton
            {
                Text = "📤 NỘP BÀI",
                Location = new Point(800, 620),
                Width = 160,
                Height = 40,
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White
            };
            btnSubmit.Click += BtnSubmit_Click;
            this.Controls.Add(btnSubmit);

            // Timer
            _timerCountdown = new System.Windows.Forms.Timer { Interval = 1000 };
            _timerCountdown.Tick += TimerCountdown_Tick;

            LoadQuestionsAsync();
        }

        private async void LoadQuestionsAsync()
        {
            try
            {
                // 1. Tải câu hỏi từ service
                _questions = await _examService.GetQuestionsByExamIdAsync(_exam.ExamId);

                if (_questions == null || _questions.Count == 0)
                {
                    MessageBox.Show("Chưa có câu hỏi cho đề thi này.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                    return;
                }

                // 2. YÊU CẦU BẬT CAM/MIC & ĐỌC QUY CHẾ
                DialogResult confirm = MessageBox.Show(
                    "Hệ thống yêu cầu BẬT CAMERA và MICROPHONE để giám sát và Xác thực Face ID.\n\n" +
                    "Luật thi:\n" +
                    "- Vắng mặt/Có người lạ/Nhìn ra ngoài/Cúi mặt/Liếc mắt > 3 giây: Cảnh báo.\n" +
                    "- Có tiếng ồn > 10 giây: Cảnh báo.\n" +
                    "- Chuyển Tab/Mở ứng dụng khác: Cảnh báo.\n\n" +
                    "⚠️ NẾU VI PHẠM QUÁ 3 LẦN, HỆ THỐNG SẼ TỰ ĐỘNG THU BÀI.\n" +
                    "Bạn có đồng ý không?", "Xác nhận & Thỏa thuận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirm == DialogResult.No)
                {
                    this.Close();
                    return;
                }

                // 3. KHỞI TẠO GIAO DIỆN & DỊCH VỤ
                SetupAntiCheatUI();

                // Lấy FaceAiService từ DI Container (thông qua AntiCheatService)
                // Đây là một kỹ thuật Reflection nhỏ để truy cập vào biến private của service đã được tiêm
                var faceAiServiceField = typeof(AntiCheatService).GetField("_faceAiService", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                _faceAiService = (FaceAiService)faceAiServiceField.GetValue(_antiCheatService); // Bỏ chữ var ở đầu dòng này

                // Bật Camera phần cứng
                if (!_faceAiService.StartCamera(0))
                {
                    MessageBox.Show("Không thể kết nối với Camera. Vui lòng kiểm tra lại thiết bị!", "Lỗi Camera", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                // 4. XÁC THỰC FACE ID TRƯỚC KHI BẮT ĐẦU
                _lblMonitoringStatus.Text = "ĐANG XÁC THỰC KHUÔN MẶT...";
                _lblMonitoringStatus.ForeColor = Color.Blue;
                bool isVerified = false;

                var authService = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<AuthService>(Program.ServiceProvider);

                // Quét trong vòng 10 giây để tìm khuôn mặt khớp với DB
                for (int i = 0; i < 50; i++)
                {
                    using (var frame = _faceAiService.GetFrame())
                    {
                        if (frame != null && !frame.IsEmpty)
                        {
                            _picCameraFeed.Image?.Dispose();
                            _picCameraFeed.Image = frame.ToBitmap(); // Hiện ảnh đang quét

                            var embedding = _faceAiService.ExtractEmbedding(frame);
                            if (embedding != null)
                            {
                                var loginResult = await authService.LoginWithFaceAsync(embedding);
                                // Chỉ xác thực thành công khi UserID khớp
                                if (loginResult.success && loginResult.user.UserId == _examResult.StudentId)
                                {
                                    isVerified = true;
                                    break; // Thoát vòng lặp khi đã xác thực
                                }
                            }
                        }
                    }
                    await Task.Delay(200); // Đợi 200ms mỗi lần quét
                }

                if (!isVerified)
                {
                    MessageBox.Show("Xác thực khuôn mặt thất bại! Khuôn mặt không khớp với hồ sơ đăng ký hoặc không phải bạn.", "Lỗi Face ID", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                // 5. BẮT ĐẦU GIÁM SÁT VÀ TÍNH GIỜ
                _lblMonitoringStatus.Text = "🔴 ĐANG GIÁM SÁT (CAM & MIC)";
                _lblMonitoringStatus.ForeColor = Color.Red;

                // Bắt đầu giám sát
                _antiCheatService.StartMonitoring(this.Handle, (violationMsg) =>
                {
                    this.Invoke(new Action(() => {
                        HandleViolationAlert(violationMsg);
                    }));
                }, _examResult.ResultId);

                // Timer 1: Quét gian lận (1 giây / lần)
                _antiCheatTimer = new System.Windows.Forms.Timer { Interval = 1000 };
                _antiCheatTimer.Tick += async (s, ev) => await _antiCheatService.ScanForViolationsAsync();
                _antiCheatTimer.Start();

                // Timer 2: Hiển thị hình ảnh Camera lên UI (100ms / lần)
                _cameraUiTimer = new System.Windows.Forms.Timer { Interval = 100 };
                _cameraUiTimer.Tick += (s, ev) =>
                {
                    using (var frame = _faceAiService.GetFrame())
                    {
                        if (frame != null && !frame.IsEmpty)
                        {
                            var oldImg = _picCameraFeed.Image;
                            _picCameraFeed.Image = frame.ToBitmap();
                            oldImg?.Dispose();
                        }
                    }
                };
                _cameraUiTimer.Start();

                // 6. HIỂN THỊ ĐỀ THI
                RestoreAnswersLocal();
                DisplayQuestion(0);
                DisplayQuestionList();

                // Bắt đầu tính giờ thi
                _timerCountdown.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khởi tạo bài thi: " + ex.Message);
                this.Close();
            }
        }
        private void HandleViolationAlert(string violationMsg)
        {
            // 1. Nếu đang nộp bài hoặc đang hiện một bảng báo lỗi rồi thì THOÁT LUÔN, không hiện thêm
            if (_isForceSubmitting || _isProcessingViolation) return;

            try
            {
                _isProcessingViolation = true; // Khóa chặn re-entry
                _antiCheatTimer?.Stop();       // DỪNG QUÉT NGAY LẬP TỨC để không sinh thêm lỗi mới

                _violationCount++;
                _lblMonitoringStatus.Text = $"⚠️ CẢNH BÁO ({_violationCount}/3): {violationMsg}";
                _lblMonitoringStatus.ForeColor = Color.DarkOrange;
                System.Media.SystemSounds.Beep.Play();

                // Nếu đạt ngưỡng 3 lần -> Thu bài ngay
                if (_violationCount >= 3)
                {
                    _isForceSubmitting = true;
                    CleanupResources(); // TẮT CAM/TIMER NGAY LẬP TỨC TRƯỚC KHI HIỆN THÔNG BÁO

                    MessageBox.Show(this,
                        $"BẠN ĐÃ VI PHẠM QUY CHẾ THI 3 LẦN: {violationMsg}.\nBài thi sẽ được tự động nộp!",
                        "THU BÀI TỰ ĐỘNG", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    SaveCurrentAnswer();
                    SubmitExamAsync();
                    return;
                }

                // Nếu chưa tới 3 lần -> Cảnh báo nhắc nhở
                MessageBox.Show(this,
                    $"HỆ THỐNG PHÁT HIỆN: {violationMsg}.\nĐây là lần cảnh báo thứ {_violationCount}/3.",
                    "CẢNH BÁO GIAN LẬN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                // Chỉ khi học sinh bấm OK và chưa bị thu bài thì mới mở khóa và cho quét tiếp
                if (!_isForceSubmitting)
                {
                    _isProcessingViolation = false;
                    _antiCheatTimer?.Start();
                    _lblMonitoringStatus.Text = "🔴 ĐANG GIÁM SÁT (CAM & MIC)";
                    _lblMonitoringStatus.ForeColor = Color.Red;
                }
            }
        }

        private void DisplayQuestion(int index)
        {
            if (index < 0 || index >= _questions.Count) return;

            _currentQuestionIndex = index;
            var question = _questions[index];

            lblProgress.Text = $"Câu {index + 1}/{_questions.Count}";
            lblQuestion.Text = question.Content;

            rbOptionA.Text = "A. " + (question.OptionA ?? "");
            rbOptionB.Text = "B. " + (question.OptionB ?? "");
            rbOptionC.Text = "C. " + (question.OptionC ?? "");
            rbOptionD.Text = "D. " + (question.OptionD ?? "");

            // Clear selection
            rbOptionA.Checked = false;
            rbOptionB.Checked = false;
            rbOptionC.Checked = false;
            rbOptionD.Checked = false;

            // Restore previous answer if exists
            if (_answers.ContainsKey(question.QuestionId))
            {
                string answer = _answers[question.QuestionId];
                if (answer == "A") rbOptionA.Checked = true;
                else if (answer == "B") rbOptionB.Checked = true;
                else if (answer == "C") rbOptionC.Checked = true;
                else if (answer == "D") rbOptionD.Checked = true;
            }

            btnPrevious.Enabled = index > 0;
            btnNext.Enabled = index < _questions.Count - 1;
        }

        private void DisplayQuestionList()
        {
            flpQuestions.Controls.Clear();
            for (int i = 0; i < _questions.Count; i++)
            {
                Button btn = new Button
                {
                    Text = (i + 1).ToString(),
                    Width = 40,
                    Height = 40,
                    Margin = new Padding(5),
                    BackColor = _answers.ContainsKey(_questions[i].QuestionId) ? Color.FromArgb(76, 175, 80) : Color.LightGray,
                    ForeColor = i == _currentQuestionIndex ? Color.White : Color.Black,
                    Font = new Font("Roboto", 10, FontStyle.Bold)
                };
                int questionIndex = i;
                btn.Click += (s, e) =>
                {
                    SaveCurrentAnswer(); // Lưu câu đang làm trước khi nhảy sang câu khác
                    DisplayQuestion(questionIndex); // Sau đó mới hiển thị câu được chọn
                };
                flpQuestions.Controls.Add(btn);
            }
        }

        private void BtnPrevious_Click(object sender, EventArgs e)
        {
            SaveCurrentAnswer();
            if (_currentQuestionIndex > 0)
            {
                DisplayQuestion(_currentQuestionIndex - 1);
            }
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            SaveCurrentAnswer();
            if (_currentQuestionIndex < _questions.Count - 1)
            {
                DisplayQuestion(_currentQuestionIndex + 1);
            }
        }

        private void SaveCurrentAnswer()
        {
            if (_currentQuestionIndex < _questions.Count)
            {
                var question = _questions[_currentQuestionIndex];
                if (rbOptionA.Checked) _answers[question.QuestionId] = "A";
                else if (rbOptionB.Checked) _answers[question.QuestionId] = "B";
                else if (rbOptionC.Checked) _answers[question.QuestionId] = "C";
                else if (rbOptionD.Checked) _answers[question.QuestionId] = "D";
                else _answers.Remove(question.QuestionId);

                DisplayQuestionList();

                // ===== THÊM DÒNG NÀY =====
                BackupAnswersLocal(); // Tự động lưu nháp mỗi khi chuyển câu hoặc tick đáp án
            }
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            SaveCurrentAnswer();
            var result = MessageBox.Show($"Bạn đã trả lời {_answers.Count}/{_questions.Count} câu.\n\nBạn chắc chắn muốn nộp bài?", "Xác nhận nộp bài", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                SubmitExamAsync();
            }
        }
        private void CleanupResources()
        {
            // Dừng tất cả timer trước
            _timerCountdown?.Stop();
            _antiCheatTimer?.Stop();
            _cameraUiTimer?.Stop();

            // Giải phóng dịch vụ giám sát
            _antiCheatService?.StopMonitoring();

            // QUAN TRỌNG: Tắt phần cứng Camera
            if (_faceAiService != null)
            {
                _faceAiService.StopCamera();
                // Đợi một chút để phần cứng kịp phản hồi
                System.Threading.Thread.Sleep(100);
            }

            // Xóa ảnh trên PictureBox để đèn cam tắt hẳn
            if (_picCameraFeed != null)
            {
                _picCameraFeed.Image?.Dispose();
                _picCameraFeed.Image = null;
            }
        }
        private async void SubmitExamAsync()
        {
            try
            {
                btnSubmit.Enabled = false;
                _timerCountdown.Stop();
                CleanupResources();

                // Calculate score
                int correctCount = 0;
                foreach (var answer in _answers)
                {
                    var question = _questions.FirstOrDefault(q => q.QuestionId == answer.Key);
                    if (question != null && question.CorrectOption == answer.Value)
                    {
                        correctCount++;
                    }
                }

                float score = (_questions.Count > 0) ? (correctCount * 10f / _questions.Count) : 0;

                // Update exam result
                _examResult.Score = score;
                _examResult.CompletedAt = DateTime.Now;

                // ===== THÊM 1 DÒNG NÀY ĐỂ LƯU ĐÁP ÁN =====
                _examResult.AnswersData = JsonSerializer.Serialize(_answers);
                // ==========================================

                // Save to DB
                await _examService.UpdateExamResultAsync(_examResult);
                // ===== THÊM ĐOẠN NÀY =====
                // Nộp bài thành công lên Server rồi thì xóa file nháp đi để lần sau thi lại không bị dính
                string backupPath = GetBackupFilePath();
                if (File.Exists(backupPath))
                {
                    File.Delete(backupPath);
                }
                MessageBox.Show($"Nộp bài thành công!\n\nKết quả: {correctCount}/{_questions.Count} câu đúng\nĐiểm: {score:F1}/10", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nộp bài: " + ex.Message);
                btnSubmit.Enabled = true;
            }
        }

        private void TimerCountdown_Tick(object sender, EventArgs e)
        {
            if (_isForceSubmitting) return;
            _remainingSeconds--;
            int minutes = _remainingSeconds / 60;
            int seconds = _remainingSeconds % 60;
            lblTimer.Text = $"Thời gian: {minutes:D2}:{seconds:D2}";

            if (_remainingSeconds <= 0)
            {
                _timerCountdown.Stop();
                MessageBox.Show("Hết thời gian! Bài làm của bạn sẽ được nộp.", "Hết giờ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SaveCurrentAnswer();
                SubmitExamAsync();
            }
        }

        private void FrmTakeExam_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 1. Dừng các Timer (Dùng dấu ? để nếu chưa khởi tạo cũng không bị lỗi)
            _timerCountdown?.Stop();
            _antiCheatTimer?.Stop();
            _cameraUiTimer?.Stop();

            // 2. Dừng dịch vụ giám sát
            _antiCheatService?.StopMonitoring();

            // 3. Tắt phần cứng camera
            if (_faceAiService != null)
            {
                _faceAiService.StopCamera();
            }

            // 4. GIẢI PHÓNG ẢNH (Đoạn này gây lỗi, đã được sửa an toàn)
            if (_picCameraFeed != null && _picCameraFeed.Image != null)
            {
                var img = _picCameraFeed.Image;
                _picCameraFeed.Image = null;
                img.Dispose();
            }
        }
        // Tạo đường dẫn file lưu nháp (Vd: C:\Users\Admin\AppData\Local\Temp\OmniSight_Exam_222.json)
        private string GetBackupFilePath()
        {
            return Path.Combine(Path.GetTempPath(), $"OmniSight_Exam_{_exam.ExamId}_Backup.json");
        }

        // Hàm lưu nháp xuống ổ cứng
        private void BackupAnswersLocal()
        {
            try
            {
                string json = JsonSerializer.Serialize(_answers);
                File.WriteAllText(GetBackupFilePath(), json);
            }
            catch { /* Lỗi ghi file tạm thì bỏ qua, không làm gián đoạn học sinh */ }
        }
        private void SetupAntiCheatUI()
        {
            // Tạo Label trạng thái
            _lblMonitoringStatus = new Label
            {
                Text = "🔴 ĐANG GIÁM SÁT (CAM & MIC)",
                ForeColor = Color.Red,
                Font = new Font("Roboto", 10, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(this.Width - 280, 70), // Căn góc phải
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            // Tạo PictureBox hiển thị Camera
            _picCameraFeed = new PictureBox
            {
                Size = new Size(240, 180),
                Location = new Point(this.Width - 280, 100), // Nằm dưới Label
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                SizeMode = PictureBoxSizeMode.StretchImage,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.Black
            };

            this.Controls.Add(_lblMonitoringStatus);
            this.Controls.Add(_picCameraFeed);
        }

        // Hàm khôi phục bài làm từ ổ cứng
        private void RestoreAnswersLocal()
        {
            try
            {
                string filePath = GetBackupFilePath();
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    var savedAnswers = JsonSerializer.Deserialize<Dictionary<int, string>>(json);
                    if (savedAnswers != null && savedAnswers.Count > 0)
                    {
                        _answers = savedAnswers;
                        MessageBox.Show("Phát hiện bài làm bị gián đoạn trước đó. Đã khôi phục các câu trả lời!", "Khôi phục thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch { /* Lỗi đọc file thì bắt đầu làm bài mới */ }
        }

    }
}
