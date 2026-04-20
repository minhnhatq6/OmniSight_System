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
        // ===== SERVICES & DATA =====
        private readonly Exam _exam;
        private readonly ExamResult _examResult;
        private readonly ExamService _examService;
        private readonly AntiCheatService _antiCheatService;
        private FaceAiService _faceAiService;

        // ===== QUIZ STATE =====
        private List<Question> _questions;
        private Dictionary<int, string> _answers = new Dictionary<int, string>();
        private int _currentQuestionIndex = 0;
        private System.Windows.Forms.Timer _timerCountdown;
        private int _remainingSeconds;

        // ===== FLAGS =====
        private int _violationCount = 0;
        private bool _isForceSubmitting = false;
        private bool _isSubmitted = false; // FIX: track nếu đã nộp bài rồi

        // ===== UI: HEADER =====
        private Label lblTimer;
        private Label lblProgress;

        // ===== UI: MAIN CONTENT =====
        private Panel pnlQuestionCard;
        private Label lblQuestionNumber;
        private Label lblQuestion;
        private RadioButton rbA, rbB, rbC, rbD;

        // ===== UI: RIGHT SIDEBAR =====
        private PictureBox _picCameraFeed;
        private Label _lblMonitoringStatus;
        private Label lblViolationBar;

        // ===== UI: QUESTION NAVIGATOR =====
        private FlowLayoutPanel flpQuestions;

        // ===== UI: BOTTOM BAR =====
        private Button btnPrevious, btnNext, btnSubmit;

        // ===== TIMERS =====
        private System.Windows.Forms.Timer _antiCheatTimer;
        private System.Windows.Forms.Timer _cameraUiTimer;

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
            this.Size = new Size(1200, 780);
            this.MinimumSize = new Size(1000, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.FormClosing += FrmTakeExam_FormClosing;

            BuildLayout();
            _timerCountdown = new System.Windows.Forms.Timer { Interval = 1000 };
            _timerCountdown.Tick += TimerCountdown_Tick;

            LoadQuestionsAsync();
        }

        // =================================================================
        //  BUILD LAYOUT
        // =================================================================
        private void BuildLayout()
        {
            // ── HEADER (top 60px) ──────────────────────────────────────
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(21, 101, 192),
                Padding = new Padding(20, 0, 20, 0)
            };

            lblProgress = new Label
            {
                Text = "Câu 1 / ??",
                Font = new Font("Segoe UI Semibold", 14),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 16)
            };

            lblTimer = new Label
            {
                Text = "⏱  --:--",
                Font = new Font("Segoe UI Semibold", 14),
                ForeColor = Color.FromArgb(255, 235, 59),
                AutoSize = true,
                // Đổi sang Anchor + Location rõ ràng hơn:
                Location = new Point(700, 16)  // đổi từ 900 xuống 700
            };



            pnlHeader.Controls.AddRange(new Control[] { lblProgress, lblTimer });

            // ── RIGHT SIDEBAR (260px) ──────────────────────────────────
            var pnlRight = new Panel
            {
                Width = 260,
                Dock = DockStyle.Right,
                BackColor = Color.FromArgb(250, 251, 252),
                Padding = new Padding(12, 12, 12, 12)
            };

            
            // Camera feed
            _picCameraFeed = new PictureBox
            {
                Width = 236,
                Height = 177,
                Location = new Point(12, 12),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.FromArgb(30, 30, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            // Rounded border look via Paint
            _picCameraFeed.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(21, 101, 192), 2);
                e.Graphics.DrawRectangle(pen, 0, 0, _picCameraFeed.Width - 1, _picCameraFeed.Height - 1);
            };

            _lblMonitoringStatus = new Label
            {
                Text = "🔴  ĐANG GIÁM SÁT",
                Font = new Font("Segoe UI Semibold", 9),
                ForeColor = Color.FromArgb(198, 40, 40),
                BackColor = Color.FromArgb(255, 235, 235),
                Width = 236,
                Height = 28,
                Location = new Point(12, 195),
                TextAlign = ContentAlignment.MiddleCenter,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            lblViolationBar = new Label
            {
                Text = "Vi phạm: 0 / 3",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(80, 80, 80),
                BackColor = Color.Transparent,
                Width = 236,
                Height = 22,
                Location = new Point(12, 228),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Divider
            var divider = new Label
            {
                Width = 236,
                Height = 1,
                Location = new Point(12, 258),
                BackColor = Color.FromArgb(220, 220, 220)
            };

            // Question navigator label
            var lblNavTitle = new Label
            {
                Text = "DANH SÁCH CÂU HỎI",
                Font = new Font("Segoe UI Semibold", 8),
                ForeColor = Color.FromArgb(120, 120, 120),
                BackColor = Color.Transparent,
                Width = 236,
                Height = 22,
                Location = new Point(12, 268),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Question navigator (scrollable grid)
            flpQuestions = new FlowLayoutPanel
            {
                Width = 236,
                Location = new Point(12, 292),
                Height = 360,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                AutoScroll = true,
                BackColor = Color.Transparent,
                Padding = new Padding(0)
            };

            pnlRight.Controls.AddRange(new Control[]
            {
                _picCameraFeed, _lblMonitoringStatus, lblViolationBar,
                divider, lblNavTitle, flpQuestions
            });
            this.Controls.Add(pnlRight);

            // ── BOTTOM BAR (60px) ─────────────────────────────────────
            var pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.White,
                Padding = new Padding(20, 10, 20, 10)
            };

            // Left/Right border line
            pnlBottom.Paint += (s, e) =>
            {
                e.Graphics.DrawLine(new Pen(Color.FromArgb(220, 220, 220)), 0, 0, pnlBottom.Width, 0);
            };

            btnPrevious = CreateNavButton("◀  Câu trước", Color.FromArgb(240, 242, 245), Color.FromArgb(60, 60, 60));
            btnPrevious.Location = new Point(20, 12);
            btnPrevious.Click += BtnPrevious_Click;

            btnNext = CreateNavButton("Câu sau  ▶", Color.FromArgb(21, 101, 192), Color.White);
            btnNext.Location = new Point(150, 12);
            btnNext.Click += BtnNext_Click;

            btnSubmit = CreateNavButton("📤  NỘP BÀI", Color.FromArgb(198, 40, 40), Color.White);
            btnSubmit.Location = new Point(680, 12);
            btnSubmit.Width = 130;
            btnSubmit.Font = new Font("Segoe UI Semibold", 11);
            btnSubmit.Click += BtnSubmit_Click;

            pnlBottom.Controls.AddRange(new Control[] { btnPrevious, btnNext, btnSubmit });
            this.Controls.Add(pnlBottom);

            // ── CENTER: QUESTION CARD ─────────────────────────────────
            var pnlCenter = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 247, 250),
                Padding = new Padding(30, 24, 30, 16)
            };

            pnlQuestionCard = new Panel
            {
                BackColor = Color.White,
                Dock = DockStyle.Fill,
                Padding = new Padding(32, 28, 32, 20)
            };

            // Question number badge
            lblQuestionNumber = new Label
            {
                Text = "CÂU 1",
                Font = new Font("Segoe UI Semibold", 9),
                ForeColor = Color.FromArgb(21, 101, 192),
                BackColor = Color.FromArgb(232, 240, 254),
                AutoSize = false,
                Size = new Size(72, 26),
                Location = new Point(32, 28),
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(4, 0, 4, 0)
            };

            // Question text
            lblQuestion = new Label
            {
                Text = "Đang tải câu hỏi...",
                Font = new Font("Segoe UI Semibold", 13),
                ForeColor = Color.FromArgb(20, 20, 20),
                AutoSize = false,
                Location = new Point(32, 68),
                Size = new Size(800, 80),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Separator
            var sep = new Label
            {
                BackColor = Color.FromArgb(235, 235, 235),
                AutoSize = false,
                Location = new Point(32, 155),
                Size = new Size(800, 1),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Answer options
            rbA = CreateOptionRadio("A", 172);
            rbB = CreateOptionRadio("B", 232);
            rbC = CreateOptionRadio("C", 292);
            rbD = CreateOptionRadio("D", 352);

            pnlQuestionCard.Controls.AddRange(new Control[]
            {
                lblQuestionNumber, lblQuestion, sep, rbA, rbB, rbC, rbD
            });

            pnlCenter.Controls.Add(pnlQuestionCard);
            this.Controls.Add(pnlCenter);
        }

        private Button CreateNavButton(string text, Color bg, Color fg)
        {
            return new Button
            {
                Text = text,
                Width = 120,
                Height = 36,
                BackColor = bg,
                ForeColor = fg,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 10),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };
        }

        private RadioButton CreateOptionRadio(string letter, int top)
        {
            var rb = new RadioButton
            {
                Text = $"{letter}. ...",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(32, top),
                AutoSize = false,
                Width = 800,
                Height = 44,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Padding = new Padding(8, 0, 0, 0)
            };
            rb.MouseEnter += (s, e) => { if (!rb.Checked) rb.BackColor = Color.FromArgb(245, 248, 255); };
            rb.MouseLeave += (s, e) => { if (!rb.Checked) rb.BackColor = Color.White; };
            rb.CheckedChanged += (s, e) =>
            {
                rb.BackColor = rb.Checked ? Color.FromArgb(232, 240, 254) : Color.White;
                rb.ForeColor = rb.Checked ? Color.FromArgb(21, 101, 192) : Color.FromArgb(30, 30, 30);
            };
            return rb;
        }

        // =================================================================
        //  LOAD QUESTIONS & INIT MONITORING
        // =================================================================
        private async void LoadQuestionsAsync()
        {
            try
            {
                _questions = await _examService.GetQuestionsByExamIdAsync(_exam.ExamId);

                if (_questions == null || _questions.Count == 0)
                {
                    MessageBox.Show("Chưa có câu hỏi cho đề thi này.");
                    this.Close();
                    return;
                }

                var existingViolations = await _examService.GetViolationCountAsync(_examResult.ResultId);
                _violationCount = existingViolations;

                DialogResult confirm = MessageBox.Show(
                    "Hệ thống yêu cầu BẬT CAMERA và MICROPHONE để giám sát và Xác thực Face ID.\n\n" +
                    "Luật thi:\n" +
                    "- Vắng mặt/Có người lạ/Nhìn ra ngoài/Cúi mặt/Liếc mắt > 3 giây: Cảnh báo.\n" +
                    "- Có tiếng ồn > 10 giây: Cảnh báo.\n" +
                    "- Chuyển Tab/Mở ứng dụng khác: Cảnh báo.\n\n" +
                    (_violationCount > 0 ? $"⚠️ BẠN ĐÃ CÓ {_violationCount}/3 CẢNH BÁO TỪ TRƯỚC.\n\n" : "") +
                    "⚠️ NẾU VI PHẠM QUÁ 3 LẦN, HỆ THỐNG SẼ TỰ ĐỘNG THU BÀI.\n\nBạn có đồng ý không?",
                    "Xác nhận & Thỏa thuận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirm == DialogResult.No) { this.Close(); return; }

                var faceAiServiceField = typeof(AntiCheatService).GetField("_faceAiService",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                _faceAiService = (FaceAiService)faceAiServiceField.GetValue(_antiCheatService);

                if (!_faceAiService.StartCamera(0))
                {
                    MessageBox.Show("Không thể kết nối với Camera!", "Lỗi Camera", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                _lblMonitoringStatus.Text = "🔵  ĐANG XÁC THỰC KHUÔN MẶT...";
                _lblMonitoringStatus.ForeColor = Color.FromArgb(21, 101, 192);
                _lblMonitoringStatus.BackColor = Color.FromArgb(232, 240, 254);

                bool isVerified = false;
                var authService = Microsoft.Extensions.DependencyInjection
                    .ServiceProviderServiceExtensions.GetRequiredService<AuthService>(Program.ServiceProvider);

                for (int i = 0; i < 50; i++)
                {
                    using (var frame = _faceAiService.GetFrame())
                    {
                        if (frame != null && !frame.IsEmpty)
                        {
                            _picCameraFeed.Image?.Dispose();
                            _picCameraFeed.Image = frame.ToBitmap();
                            var embedding = _faceAiService.ExtractEmbedding(frame);
                            if (embedding != null)
                            {
                                var loginResult = await authService.LoginWithFaceAsync(embedding);
                                if (loginResult.success && loginResult.user.UserId == _examResult.StudentId)
                                {
                                    isVerified = true;
                                    break;
                                }
                            }
                        }
                    }
                    await Task.Delay(200);
                }

                if (!isVerified)
                {
                    MessageBox.Show("Xác thực khuôn mặt thất bại!", "Lỗi Face ID", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                UpdateMonitoringLabel();

                _antiCheatService.StartMonitoring(this.Handle, (violationMsg) =>
                {
                    if (this.IsHandleCreated && !this.IsDisposed)
                        this.Invoke(new Action(() => HandleViolationAlert(violationMsg)));
                }, _examResult.ResultId);

                _antiCheatTimer = new System.Windows.Forms.Timer { Interval = 1000 };
                _antiCheatTimer.Tick += async (s, ev) => await _antiCheatService.ScanForViolationsAsync();
                _antiCheatTimer.Start();

                _cameraUiTimer = new System.Windows.Forms.Timer { Interval = 100 };
                _cameraUiTimer.Tick += (s, ev) =>
                {

                    using (var frame = _faceAiService.GetFrame())
                    {

                        if (frame != null && !frame.IsEmpty)
                        {
                            var old = _picCameraFeed.Image;
                            _picCameraFeed.Image = frame.ToBitmap();
                            old?.Dispose();
                        }
                    }
                };
                _cameraUiTimer.Start();

                RestoreAnswersLocal();
                DisplayQuestion(0);
                DisplayQuestionList();
                _timerCountdown.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khởi tạo bài thi: " + ex.Message);
                this.Close();
            }
        }

        private void UpdateMonitoringLabel()
        {
            _lblMonitoringStatus.Text = "🔴  ĐANG GIÁM SÁT";
            _lblMonitoringStatus.ForeColor = Color.FromArgb(198, 40, 40);
            _lblMonitoringStatus.BackColor = Color.FromArgb(255, 235, 235);
            lblViolationBar.Text = $"Vi phạm: {_violationCount} / 3";
            lblViolationBar.ForeColor = _violationCount >= 2
                ? Color.FromArgb(198, 40, 40)
                : Color.FromArgb(80, 80, 80);
        }

        // =================================================================
        //  VIOLATION HANDLER
        // =================================================================
        private void HandleViolationAlert(string violationMsg)
        {
            if (_isForceSubmitting || _antiCheatService.IsShowingAlert) return;

            try
            {
                _antiCheatService.IsShowingAlert = true;
                _antiCheatTimer?.Stop();

                _violationCount++;
                _lblMonitoringStatus.Text = $"⚠️  CẢNH BÁO ({_violationCount}/3)";
                _lblMonitoringStatus.ForeColor = Color.FromArgb(230, 100, 0);
                _lblMonitoringStatus.BackColor = Color.FromArgb(255, 245, 225);
                lblViolationBar.Text = $"Vi phạm: {_violationCount} / 3";
                lblViolationBar.ForeColor = Color.FromArgb(198, 40, 40);
                System.Media.SystemSounds.Beep.Play();

                if (_violationCount >= 3)
                {
                    _isForceSubmitting = true;
                    CleanupResources();
                    MessageBox.Show(this,
                        $"BẠN ĐÃ VI PHẠM QUY CHẾ THI 3 LẦN.\nBài thi sẽ được tự động nộp!",
                        "THU BÀI TỰ ĐỘNG", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    SaveCurrentAnswer();
                    SubmitExamAsync();
                    return;
                }

                MessageBox.Show(this,
                    $"HỆ THỐNG PHÁT HIỆN: {violationMsg}.\nĐây là lần cảnh báo thứ {_violationCount}/3.",
                    "CẢNH BÁO GIAN LẬN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                if (!_isForceSubmitting)
                {
                    _antiCheatService.IsShowingAlert = false;
                    _antiCheatTimer?.Start();
                    UpdateMonitoringLabel();
                }
            }
        }

        // =================================================================
        //  DISPLAY QUESTION
        // =================================================================
        private void DisplayQuestion(int index)
        {
            if (index < 0 || index >= _questions.Count) return;

            _currentQuestionIndex = index;
            var q = _questions[index];

            lblProgress.Text = $"Câu {index + 1} / {_questions.Count}";
            lblQuestionNumber.Text = $"CÂU {index + 1}";
            lblQuestion.Text = q.Content;

            rbA.Text = "A.  " + (q.OptionA ?? "");
            rbB.Text = "B.  " + (q.OptionB ?? "");
            rbC.Text = "C.  " + (q.OptionC ?? "");
            rbD.Text = "D.  " + (q.OptionD ?? "");

            // Reset styles first
            foreach (var rb in new[] { rbA, rbB, rbC, rbD })
            {
                rb.Checked = false;
                rb.BackColor = Color.White;
                rb.ForeColor = Color.FromArgb(30, 30, 30);
            }

            if (_answers.TryGetValue(q.QuestionId, out string saved))
            {
                var target = saved == "A" ? rbA : saved == "B" ? rbB : saved == "C" ? rbC : rbD;
                target.Checked = true;
            }

            btnPrevious.Enabled = index > 0;
            btnNext.Enabled = index < _questions.Count - 1;

            // Highlight active question button in navigator
            DisplayQuestionList();
        }

        // =================================================================
        //  QUESTION NAVIGATOR
        // =================================================================
        private void DisplayQuestionList()
        {
            flpQuestions.Controls.Clear();
            for (int i = 0; i < _questions.Count; i++)
            {
                bool answered = _answers.ContainsKey(_questions[i].QuestionId);
                bool isCurrent = i == _currentQuestionIndex;

                var btn = new Button
                {
                    Text = (i + 1).ToString(),
                    Width = 38,
                    Height = 38,
                    Margin = new Padding(3),
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI Semibold", 9),
                    Cursor = Cursors.Hand,
                    BackColor = isCurrent
                        ? Color.FromArgb(21, 101, 192)
                        : answered
                            ? Color.FromArgb(56, 142, 60)
                            : Color.FromArgb(240, 242, 245),
                    ForeColor = (isCurrent || answered) ? Color.White : Color.FromArgb(60, 60, 60),
                    FlatAppearance = { BorderSize = 0 }
                };

                int qi = i;
                btn.Click += (s, e) =>
                {
                    SaveCurrentAnswer();
                    DisplayQuestion(qi);
                };
                flpQuestions.Controls.Add(btn);
            }
        }

        // =================================================================
        //  NAVIGATION BUTTONS
        // =================================================================
        private void BtnPrevious_Click(object sender, EventArgs e)
        {
            SaveCurrentAnswer();
            if (_currentQuestionIndex > 0)
                DisplayQuestion(_currentQuestionIndex - 1);
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            SaveCurrentAnswer();
            if (_currentQuestionIndex < _questions.Count - 1)
                DisplayQuestion(_currentQuestionIndex + 1);
        }

        private void SaveCurrentAnswer()
        {
            if (_questions == null || _currentQuestionIndex >= _questions.Count) return;
            var q = _questions[_currentQuestionIndex];

            if (rbA.Checked) _answers[q.QuestionId] = "A";
            else if (rbB.Checked) _answers[q.QuestionId] = "B";
            else if (rbC.Checked) _answers[q.QuestionId] = "C";
            else if (rbD.Checked) _answers[q.QuestionId] = "D";
            else _answers.Remove(q.QuestionId);

            BackupAnswersLocal();
        }

        // =================================================================
        //  SUBMIT
        // =================================================================
        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            SaveCurrentAnswer();
            int answered = _answers.Count;
            int total = _questions?.Count ?? 0;

            var result = MessageBox.Show(
                $"Bạn đã trả lời {answered}/{total} câu.\n\nBạn chắc chắn muốn nộp bài?",
                "Xác nhận nộp bài", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
                SubmitExamAsync();
        }

        private void CleanupResources()
        {
            _timerCountdown?.Stop();
            _antiCheatTimer?.Stop();
            _cameraUiTimer?.Stop();
            _antiCheatService?.StopMonitoring();

            if (_faceAiService != null)
            {
                _faceAiService.StopCamera();
                System.Threading.Thread.Sleep(100);
            }

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
                _isSubmitted = true; // FIX: đánh dấu đã nộp
                btnSubmit.Enabled = false;
                _timerCountdown?.Stop();
                CleanupResources();

                int correctCount = 0;
                foreach (var answer in _answers)
                {
                    var q = _questions.FirstOrDefault(x => x.QuestionId == answer.Key);
                    if (q != null && q.CorrectOption == answer.Value) correctCount++;
                }

                float score = (_questions.Count > 0) ? (correctCount * 10f / _questions.Count) : 0;

                _examResult.Score = score;
                _examResult.CompletedAt = DateTime.Now;
                _examResult.AnswersData = JsonSerializer.Serialize(_answers);

                await _examService.UpdateExamResultAsync(_examResult);

                string backupPath = GetBackupFilePath();
                if (File.Exists(backupPath)) File.Delete(backupPath);

                MessageBox.Show(
                    $"Nộp bài thành công!\n\nKết quả: {correctCount}/{_questions.Count} câu đúng\nĐiểm: {score:F1}/10",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nộp bài: " + ex.Message);
                btnSubmit.Enabled = true;
                _isSubmitted = false;
            }
        }

        // =================================================================
        //  TIMER
        // =================================================================
        private void TimerCountdown_Tick(object sender, EventArgs e)
        {
            if (_isForceSubmitting) return;
            _remainingSeconds--;
            int m = _remainingSeconds / 60;
            int s = _remainingSeconds % 60;
            lblTimer.Text = $"⏱  {m:D2}:{s:D2}";

            // Turn red in last 5 minutes
            lblTimer.ForeColor = _remainingSeconds <= 300
                ? Color.FromArgb(255, 82, 82)
                : Color.FromArgb(255, 235, 59);

            if (_remainingSeconds <= 0)
            {
                _timerCountdown.Stop();
                MessageBox.Show("Hết thời gian! Bài làm của bạn sẽ được nộp.", "Hết giờ",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SaveCurrentAnswer();
                SubmitExamAsync();
            }
        }

        // =================================================================
        //  FORM CLOSING — FIX BUG ĐÓNG FORM GIỮA CHỪNG
        // =================================================================
        private void FrmTakeExam_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_isSubmitted || _isForceSubmitting) return;
            if (_questions == null || _questions.Count == 0) return;

            // Chặn đóng, hỏi người dùng
            e.Cancel = true;

            // Dùng BeginInvoke để tránh deadlock với form closing event
            this.BeginInvoke(new Action(() =>
            {
                var choice = MessageBox.Show(this,
                    "Bài thi đang diễn ra!\n\n" +
                    "• Chọn YES để thoát và LƯU NHÁP bài làm.\n" +
                    "• Chọn NO để tiếp tục làm bài.",
                    "Xác nhận thoát bài thi",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (choice == DialogResult.No) return;

                // Lưu nháp
                SaveCurrentAnswer();
                BackupAnswersLocal();

                // Dọn tài nguyên
                _timerCountdown?.Stop();
                _antiCheatTimer?.Stop();
                _cameraUiTimer?.Stop();
                _antiCheatService?.StopMonitoring();
                _faceAiService?.StopCamera();

                if (_picCameraFeed?.Image != null)
                {
                    var img = _picCameraFeed.Image;
                    _picCameraFeed.Image = null;
                    img?.Dispose();
                }

                // Đánh dấu đã xử lý rồi đóng
                _isSubmitted = true;
                this.Close();
            }));
        }

        // =================================================================
        //  LOCAL BACKUP
        // =================================================================
        private string GetBackupFilePath() =>
            Path.Combine(Path.GetTempPath(), $"OmniSight_Exam_{_exam.ExamId}_Backup.json");

        private void BackupAnswersLocal()
        {
            try { File.WriteAllText(GetBackupFilePath(), JsonSerializer.Serialize(_answers)); }
            catch { }
        }

        private void RestoreAnswersLocal()
        {
            try
            {
                string path = GetBackupFilePath();
                if (!File.Exists(path)) return;
                var saved = JsonSerializer.Deserialize<Dictionary<int, string>>(File.ReadAllText(path));
                if (saved != null && saved.Count > 0)
                {
                    _answers = saved;
                    MessageBox.Show("Phát hiện bài làm bị gián đoạn trước đó. Đã khôi phục các câu trả lời!",
                        "Khôi phục thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch { }
        }
    }
}