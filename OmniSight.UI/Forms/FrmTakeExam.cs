using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin.Controls;
using OmniSight.Core.Entities;
using OmniSight.Services;

namespace OmniSight.UI.Forms
{
    public class FrmTakeExam : Form
    {
        private readonly Exam _exam;
        private readonly ExamResult _examResult;
        private readonly ExamService _examService;
        private List<Question> _questions;
        private Dictionary<int, string> _answers = new Dictionary<int, string>(); // QuestionId -> Selected Option
        private int _currentQuestionIndex = 0;
        private System.Windows.Forms.Timer _timerCountdown;
        private int _remainingSeconds;

        private MaterialLabel lblTimer;
        private MaterialLabel lblProgress;
        private MaterialLabel lblQuestion;
        private MaterialRadioButton rbOptionA, rbOptionB, rbOptionC, rbOptionD;
        private MaterialButton btnPrevious, btnNext, btnSubmit;
        private FlowLayoutPanel flpQuestions;

        public FrmTakeExam(Exam exam, ExamResult examResult, ExamService examService)
        {
            _exam = exam;
            _examResult = examResult;
            _examService = examService;
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
                // Load questions from service
                _questions = await _examService.GetQuestionsByExamIdAsync(_exam.ExamId);
                if (_questions == null || _questions.Count == 0)
                {
                    MessageBox.Show("Chưa có câu hỏi cho đề thi này.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                    return;
                }

                DisplayQuestion(0);
                DisplayQuestionList();
                _timerCountdown.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải câu hỏi: " + ex.Message);
                this.Close();
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
                btn.Click += (s, e) => DisplayQuestion(questionIndex);
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

        private async void SubmitExamAsync()
        {
            try
            {
                btnSubmit.Enabled = false;
                _timerCountdown.Stop();

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

                // Save to DB
                await _examService.UpdateExamResultAsync(_examResult);

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
            _timerCountdown?.Stop();
            _timerCountdown?.Dispose();
        }
    }
}
