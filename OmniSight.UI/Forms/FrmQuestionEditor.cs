using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin.Controls;
using OmniSight.Core.Entities;
using OmniSight.Services;

namespace OmniSight.UI.Forms
{
    public partial class FrmQuestionEditor : Form
    {
        private readonly ExamService _examService;
        private readonly int _examId;
        private int? _questionId;
        private Question _question;

        private MaterialTextBox txtContent;
        private MaterialTextBox txtOptionA;
        private MaterialTextBox txtOptionB;
        private MaterialTextBox txtOptionC;
        private MaterialTextBox txtOptionD;
        private ComboBox cbCorrectOption;
        private MaterialButton btnSave;
        private MaterialButton btnCancel;

        public FrmQuestionEditor(ExamService examService, int examId, int? questionId = null)
        {
            _examService = examService;
            _examId = examId;
            _questionId = questionId;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = _questionId.HasValue ? "Sửa Câu Hỏi" : "Thêm Câu Hỏi";
            this.Width = 600;
            this.Height = 500;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = System.Drawing.Color.White;

            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15), AutoScroll = true };

            // Content
            var lblContent = new MaterialLabel { Text = "Nội Dung Câu Hỏi:", Location = new System.Drawing.Point(0, 0), AutoSize = true };
            txtContent = new MaterialTextBox { Location = new System.Drawing.Point(0, 30), Width = 550, Hint = "Nhập nội dung câu hỏi" };

            // Options
            var lblOptionA = new MaterialLabel { Text = "Đáp án A:", Location = new System.Drawing.Point(0, 90), AutoSize = true };
            txtOptionA = new MaterialTextBox { Location = new System.Drawing.Point(0, 120), Width = 550, Hint = "Nhập đáp án A" };

            var lblOptionB = new MaterialLabel { Text = "Đáp án B:", Location = new System.Drawing.Point(0, 180), AutoSize = true };
            txtOptionB = new MaterialTextBox { Location = new System.Drawing.Point(0, 210), Width = 550, Hint = "Nhập đáp án B" };

            var lblOptionC = new MaterialLabel { Text = "Đáp án C:", Location = new System.Drawing.Point(0, 270), AutoSize = true };
            txtOptionC = new MaterialTextBox { Location = new System.Drawing.Point(0, 300), Width = 550, Hint = "Nhập đáp án C" };

            var lblOptionD = new MaterialLabel { Text = "Đáp án D:", Location = new System.Drawing.Point(0, 360), AutoSize = true };
            txtOptionD = new MaterialTextBox { Location = new System.Drawing.Point(0, 390), Width = 550, Hint = "Nhập đáp án D" };

            // Correct Option
            var lblCorrectOption = new MaterialLabel { Text = "Đáp Án Đúng:", Location = new System.Drawing.Point(0, 450), AutoSize = true };
            cbCorrectOption = new ComboBox 
            { 
                Location = new System.Drawing.Point(0, 480), 
                Width = 100, 
                DropDownStyle = ComboBoxStyle.DropDownList 
            };
            cbCorrectOption.Items.AddRange(new object[] { "A", "B", "C", "D" });
            cbCorrectOption.SelectedIndex = 0;

            // Buttons
            btnSave = new MaterialButton { Text = "💾 Lưu", Location = new System.Drawing.Point(0, 520), Width = 100 };
            btnSave.Click += BtnSave_Click;
            btnCancel = new MaterialButton { Text = "❌ Hủy", Location = new System.Drawing.Point(110, 520), Width = 100 };
            btnCancel.Click += (s, e) => this.Close();

            panel.Controls.AddRange(new Control[] 
            { 
                lblContent, txtContent,
                lblOptionA, txtOptionA,
                lblOptionB, txtOptionB,
                lblOptionC, txtOptionC,
                lblOptionD, txtOptionD,
                lblCorrectOption, cbCorrectOption,
                btnSave, btnCancel
            });

            this.Controls.Add(panel);

            if (_questionId.HasValue)
            {
                LoadQuestionAsync();
            }
        }

        private async void LoadQuestionAsync()
        {
            try
            {
                var questions = await _examService.GetQuestionsByExamIdAsync(_examId);
                _question = questions.Find(q => q.QuestionId == _questionId.Value);

                if (_question != null)
                {
                    txtContent.Text = _question.Content;
                    txtOptionA.Text = _question.OptionA;
                    txtOptionB.Text = _question.OptionB;
                    txtOptionC.Text = _question.OptionC;
                    txtOptionD.Text = _question.OptionD;
                    cbCorrectOption.SelectedItem = _question.CorrectOption;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải câu hỏi: " + ex.Message);
            }
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtContent.Text))
            {
                MessageBox.Show("Vui lòng nhập nội dung câu hỏi");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtOptionA.Text) || string.IsNullOrWhiteSpace(txtOptionB.Text) ||
                string.IsNullOrWhiteSpace(txtOptionC.Text) || string.IsNullOrWhiteSpace(txtOptionD.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ 4 đáp án");
                return;
            }

            try
            {
                if (_questionId.HasValue)
                {
                    // Update existing question
                    _question.Content = txtContent.Text;
                    _question.OptionA = txtOptionA.Text;
                    _question.OptionB = txtOptionB.Text;
                    _question.OptionC = txtOptionC.Text;
                    _question.OptionD = txtOptionD.Text;
                    _question.CorrectOption = cbCorrectOption.SelectedItem?.ToString();

                    await _examService.UpdateQuestionAsync(_question);
                    MessageBox.Show("Cập nhật câu hỏi thành công");
                }
                else
                {
                    // Create new question
                    var newQuestion = new Question
                    {
                        ExamId = _examId,
                        Content = txtContent.Text,
                        OptionA = txtOptionA.Text,
                        OptionB = txtOptionB.Text,
                        OptionC = txtOptionC.Text,
                        OptionD = txtOptionD.Text,
                        CorrectOption = cbCorrectOption.SelectedItem?.ToString()
                    };

                    await _examService.CreateQuestionAsync(newQuestion);
                    MessageBox.Show("Thêm câu hỏi thành công");
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu câu hỏi: " + ex.Message);
            }
        }
    }
}
