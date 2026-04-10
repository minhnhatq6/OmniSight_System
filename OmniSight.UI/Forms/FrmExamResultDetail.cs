using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin.Controls;
using OmniSight.Core.Entities;
using OmniSight.Services;

namespace OmniSight.UI.Forms
{
    public class FrmExamResultDetail : Form
    {
        private readonly ExamService _examService;
        private readonly Exam _exam;
        private readonly User _student;
        private ExamResult _examResult;
        private List<Question> _questions;
        private Dictionary<int, string> _studentAnswers;

        private MaterialLabel lblStudentName;
        private MaterialLabel lblScore;
        private MaterialLabel lblExamName;
        private MaterialLabel lblTimeInfo;
        private DataGridView dgvAnswers;
        private MaterialButton btnExportPDF;
        private MaterialButton btnExportExcel;
        private MaterialButton btnClose;

        public FrmExamResultDetail(ExamService examService, Exam exam, ExamResult examResult, User student)
        {
            _examService = examService;
            _exam = exam;
            _examResult = examResult;
            _student = student;
            _questions = new List<Question>();
            _studentAnswers = new Dictionary<int, string>();
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Chi Tiết Kết Quả Bài Thi";
            this.Width = 900;
            this.Height = 650;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;
            this.Font = new Font("Roboto", 10);

            // Header Panel
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = Color.FromArgb(33, 150, 243),
                Padding = new Padding(15)
            };

            lblExamName = new MaterialLabel
            {
                Text = $"📋 {_exam.Title}",
                Font = new Font("Roboto", 14, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(10, 10)
            };

            lblStudentName = new MaterialLabel
            {
                Text = $"👤 Học Sinh: {_student?.FullName ?? "Unknown"}",
                Font = new Font("Roboto", 11),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(10, 40)
            };

            lblScore = new MaterialLabel
            {
                Text = $"🎯 Điểm: {_examResult?.Score:F1}/10",
                Font = new Font("Roboto", 12, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(10, 70)
            };

            lblTimeInfo = new MaterialLabel
            {
                Text = $"⏱️ Thời gian: {_examResult?.StartedAt:dd/MM/yyyy HH:mm} → {_examResult?.CompletedAt:dd/MM/yyyy HH:mm}",
                Font = new Font("Roboto", 10),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(400, 40)
            };

            pnlHeader.Controls.AddRange(new Control[] { lblExamName, lblStudentName, lblScore, lblTimeInfo });

            // DataGridView for answers
            dgvAnswers = new DataGridView
            {
                Location = new Point(10, 130),
                Width = 870,
                Height = 400,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false,
                GridColor = Color.FromArgb(200, 200, 200)
            };

            dgvAnswers.Columns.AddRange(
                new DataGridViewTextBoxColumn { Name = "QuestionId", DataPropertyName = "QuestionId", HeaderText = "ID", Visible = false },
                new DataGridViewTextBoxColumn { Name = "QuestionNum", HeaderText = "Câu", Width = 40 },
                new DataGridViewTextBoxColumn { Name = "Content", DataPropertyName = "Content", HeaderText = "Nội Dung Câu Hỏi", Width = 250 },
                new DataGridViewTextBoxColumn { Name = "StudentAnswer", HeaderText = "Đáp Án của Học Sinh", Width = 80, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter, Font = new Font("Roboto", 10, FontStyle.Bold), ForeColor = Color.Blue } },
                new DataGridViewTextBoxColumn { Name = "CorrectAnswer", DataPropertyName = "CorrectOption", HeaderText = "Đáp Án Đúng", Width = 80, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter, Font = new Font("Roboto", 10, FontStyle.Bold), ForeColor = Color.Green } },
                new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Kết Quả", Width = 80, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } }
            );

            // Buttons Panel
            var pnlButtons = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = Color.FromArgb(240, 240, 240),
                Padding = new Padding(10)
            };

            btnExportExcel = new MaterialButton
            {
                Text = "📊 Xuất Excel",
                Location = new Point(10, 10),
                Width = 120,
                Height = 35
            };
            btnExportExcel.Click += BtnExportExcel_Click;

            btnExportPDF = new MaterialButton
            {
                Text = "📄 Xuất PDF",
                Location = new Point(140, 10),
                Width = 120,
                Height = 35
            };
            btnExportPDF.Click += BtnExportPDF_Click;

            btnClose = new MaterialButton
            {
                Text = "❌ Đóng",
                Location = new Point(750, 10),
                Width = 100,
                Height = 35
            };
            btnClose.Click += (s, e) => this.Close();

            pnlButtons.Controls.AddRange(new Control[] { btnExportExcel, btnExportPDF, btnClose });

            this.Controls.Add(dgvAnswers);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlButtons);
        }

        private async void LoadData()
        {
            try
            {
                // Load questions
                _questions = await _examService.GetQuestionsByExamIdAsync(_exam.ExamId);

                // Load student answers from database (placeholder - need to extend ExamResult to store answers)
                // For now, we'll display the questions and mark as correct/incorrect based on score
                RefreshAnswersGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }

        private void RefreshAnswersGrid()
        {
            dgvAnswers.DataSource = null;

            var data = new List<dynamic>();
            for (int i = 0; i < _questions.Count; i++)
            {
                var question = _questions[i];
                string studentAnswer = "Không trả lời";
                string status = "❌ Sai";
                Color statusColor = Color.Red;

                // In real implementation, get student's actual answer from database
                // For now, this is a placeholder
                if (_studentAnswers.ContainsKey(question.QuestionId))
                {
                    studentAnswer = _studentAnswers[question.QuestionId];
                    if (studentAnswer == question.CorrectOption)
                    {
                        status = "✅ Đúng";
                        statusColor = Color.Green;
                    }
                }

                data.Add(new
                {
                    QuestionId = question.QuestionId,
                    QuestionNum = i + 1,
                    Content = question.Content,
                    StudentAnswer = studentAnswer,
                    CorrectAnswer = question.CorrectOption,
                    Status = status
                });
            }

            dgvAnswers.DataSource = data;

            // Color code the status column
            foreach (DataGridViewRow row in dgvAnswers.Rows)
            {
                string status = row.Cells["Status"].Value?.ToString() ?? "";
                if (status.Contains("Đúng"))
                    row.Cells["Status"].Style.ForeColor = Color.Green;
                else
                    row.Cells["Status"].Style.ForeColor = Color.Red;
            }
        }

        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx",
                    FileName = $"KetQua_{_student.FullName}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToExcel(saveDialog.FileName);
                    MessageBox.Show("Xuất file Excel thành công!", "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = saveDialog.FileName,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xuất file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnExportPDF_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Tính năng xuất PDF sẽ được triển khai trong phiên bản tiếp theo.", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ExportToExcel(string filePath)
        {
            // Simple CSV export (can be enhanced with EPPLUS library)
            using (StreamWriter sw = new StreamWriter(filePath, false, System.Text.Encoding.UTF8))
            {
                // Header
                sw.WriteLine("Kết Quả Bài Thi");
                sw.WriteLine($"Bài Thi: {_exam.Title}");
                sw.WriteLine($"Học Sinh: {_student.FullName}");
                sw.WriteLine($"Điểm: {_examResult.Score:F1}/10");
                sw.WriteLine($"Thời gian bắt đầu: {_examResult.StartedAt:dd/MM/yyyy HH:mm}");
                sw.WriteLine($"Thời gian kết thúc: {_examResult.CompletedAt:dd/MM/yyyy HH:mm}");
                sw.WriteLine();

                // Questions and Answers
                sw.WriteLine("Câu,Nội Dung,Đáp Án Của Học Sinh,Đáp Án Đúng,Kết Quả");

                for (int i = 0; i < _questions.Count; i++)
                {
                    var question = _questions[i];
                    string studentAnswer = "Không trả lời";
                    string status = "Sai";

                    if (_studentAnswers.ContainsKey(question.QuestionId))
                    {
                        studentAnswer = _studentAnswers[question.QuestionId];
                        if (studentAnswer == question.CorrectOption)
                            status = "Đúng";
                    }

                    string line = $"\"{i + 1}\",\"{question.Content}\",\"{studentAnswer}\",\"{question.CorrectOption}\",\"{status}\"";
                    sw.WriteLine(line);
                }
            }
        }
    }
}
