using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using OmniSight.Core.Entities;
using OmniSight.Services;

namespace OmniSight.UI.Forms
{
    // 1. Đổi kế thừa từ Form sang MaterialForm
    public partial class FrmQuestionEditor : MaterialForm
    {
        private readonly ExamService _examService;
        private readonly int _examId;
        private int? _questionId;
        private Question _question;

        // Đổi txtContent thành dạng gõ nhiều dòng (vì câu hỏi thường khá dài)
        private MaterialMultiLineTextBox2 txtContent;
        private MaterialTextBox txtOptionA;
        private MaterialTextBox txtOptionB;
        private MaterialTextBox txtOptionC;
        private MaterialTextBox txtOptionD;

        // Đổi ComboBox thường thành MaterialComboBox cho đồng bộ giao diện
        private MaterialComboBox cbCorrectOption;
        private MaterialButton btnSave;
        private MaterialButton btnCancel;

        public FrmQuestionEditor(ExamService examService, int examId, int? questionId = null)
        {
            _examService = examService;
            _examId = examId;
            _questionId = questionId;
            InitializeComponent();

            // 2. Thêm Form này vào trình quản lý giao diện để nó tự động "ăn" theo màu sắc và Dark Mode của MainForm
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
        }

        private void InitializeComponent()
        {
            this.Text = _questionId.HasValue ? "SỬA CÂU HỎI" : "THÊM CÂU HỎI MỚI";
            this.Width = 600;
            this.Height = 650; // Tăng chiều cao lên một chút cho thoáng
            this.StartPosition = FormStartPosition.CenterParent;
            this.Sizable = false; // Tắt tính năng kéo dãn form
            this.MaximizeBox = false;

            // 3. Tọa độ Y bắt đầu từ 80 để tránh bị thanh tiêu đề (Title bar) che mất
            // Content
            txtContent = new MaterialMultiLineTextBox2
            {
                Location = new System.Drawing.Point(20, 80),
                Width = 560,
                Height = 100,
                Hint = "Nhập nội dung câu hỏi..."
            };

            // Options (Lồng luôn nhãn A, B, C, D vào Hint cho gọn gàng)
            txtOptionA = new MaterialTextBox { Location = new System.Drawing.Point(20, 200), Width = 560, Hint = "A. Nhập đáp án A" };
            txtOptionB = new MaterialTextBox { Location = new System.Drawing.Point(20, 260), Width = 560, Hint = "B. Nhập đáp án B" };
            txtOptionC = new MaterialTextBox { Location = new System.Drawing.Point(20, 320), Width = 560, Hint = "C. Nhập đáp án C" };
            txtOptionD = new MaterialTextBox { Location = new System.Drawing.Point(20, 380), Width = 560, Hint = "D. Nhập đáp án D" };

            // Correct Option
            var lblCorrectOption = new MaterialLabel { Text = "Đáp Án Đúng:", Location = new System.Drawing.Point(20, 460), AutoSize = true };
            cbCorrectOption = new MaterialComboBox
            {
                Location = new System.Drawing.Point(150, 445),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cbCorrectOption.Items.AddRange(new object[] { "A", "B", "C", "D" });
            cbCorrectOption.SelectedIndex = 0;

            // Buttons
            btnSave = new MaterialButton
            {
                Text = "LƯU CÂU HỎI",
                Location = new System.Drawing.Point(20, 530),
                AutoSize = true,
                UseAccentColor = true, // Dùng màu nhấn cho nổi bật
                Type = MaterialButton.MaterialButtonType.Contained
            };
            btnSave.Click += BtnSave_Click;

            btnCancel = new MaterialButton
            {
                Text = "HỦY",
                Location = new System.Drawing.Point(180, 530),
                AutoSize = true,
                Type = MaterialButton.MaterialButtonType.Outlined // Nút hủy để dạng viền cho đỡ vướng mắt
            };
            btnCancel.Click += (s, e) => this.Close();

            // Đưa tất cả control thẳng vào form
            this.Controls.AddRange(new Control[]
            {
                txtContent,
                txtOptionA, txtOptionB, txtOptionC, txtOptionD,
                lblCorrectOption, cbCorrectOption,
                btnSave, btnCancel
            });

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