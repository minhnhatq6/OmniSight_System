using MaterialSkin.Controls;
using OmniSight.Core.Entities;
using OmniSight.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace OmniSight.UI.Forms
{
    public class FrmExamResultDetail : Form
    {
        private readonly ExamService _examService;
        private readonly Exam _exam;
        private readonly User _student;
        private ExamResult _examResult;
        private List<Question> _questions;
        private Dictionary<string, string> _studentAnswers;

        private MaterialLabel lblStudentName;
        private MaterialLabel lblScore;
        private MaterialLabel lblExamName;
        private MaterialLabel lblTimeInfo;
        private DataGridView dgvAnswers;
        private MaterialButton btnExportPDF;
        private MaterialButton btnExportExcel;
        private MaterialButton btnClose;
        private DataGridView dgvViolations;

        public FrmExamResultDetail(ExamService examService, Exam exam, ExamResult examResult, User student)
        {
            _examService = examService;
            _exam = exam;
            _examResult = examResult;
            _student = student;
            _questions = new List<Question>();
            _studentAnswers = new Dictionary<string, string>();
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
    new DataGridViewTextBoxColumn { Name = "QuestionNum", DataPropertyName = "QuestionNum", HeaderText = "Câu", Width = 50 },
    new DataGridViewTextBoxColumn { Name = "Content", DataPropertyName = "Content", HeaderText = "Nội Dung Câu Hỏi", Width = 300 },

    // Sửa DataPropertyName cho khớp với logic bên dưới
    new DataGridViewTextBoxColumn { Name = "StudentAnswer", DataPropertyName = "StudentAnswer", HeaderText = "Đáp Án của HS", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter, Font = new Font("Roboto", 10, FontStyle.Bold), ForeColor = Color.Blue } },
    new DataGridViewTextBoxColumn { Name = "CorrectAnswer", DataPropertyName = "CorrectAnswer", HeaderText = "Đáp Án Đúng", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter, Font = new Font("Roboto", 10, FontStyle.Bold), ForeColor = Color.Green } },
    new DataGridViewTextBoxColumn { Name = "Status", DataPropertyName = "Status", HeaderText = "Kết Quả", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } }
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
            var lblViolationTitle = new Label { Text = "📝 Nhật Ký Vi Phạm", Font = new Font("Roboto", 12, FontStyle.Bold), Location = new Point(10, 400), AutoSize = true };

            dgvViolations = new DataGridView
            {
                Location = new Point(10, 430),
                Width = 870,
                Height = 300,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true
            };

            dgvViolations.Columns.AddRange(
                new DataGridViewTextBoxColumn { Name = "Timestamp", DataPropertyName = "Timestamp", HeaderText = "Thời gian", DefaultCellStyle = new DataGridViewCellStyle { Format = "HH:mm:ss dd/MM" }, FillWeight = 20 },
                new DataGridViewTextBoxColumn { Name = "ViolationType", DataPropertyName = "ViolationType", HeaderText = "Hành Vi", FillWeight = 50 },
                new DataGridViewLinkColumn { Name = "ImageUrl", DataPropertyName = "ImageUrl", HeaderText = "Bằng Chứng", FillWeight = 30, LinkColor = Color.Blue, TrackVisitedState = false }
            );

            // Thêm sự kiện để bấm vào link mở ảnh
            dgvViolations.CellContentClick += DgvViolations_CellContentClick;

            pnlButtons.Controls.AddRange(new Control[] { btnExportExcel, btnExportPDF, btnClose });

            this.Controls.Add(dgvAnswers);
            this.Controls.Add(lblViolationTitle); // Thêm tiêu đề
            this.Controls.Add(dgvViolations);   // Thêm bảng vi phạm
            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlButtons);
        }
        private void DgvViolations_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Chỉ xử lý khi bấm vào cột "Bằng Chứng" và không phải dòng header
            if (e.RowIndex >= 0 && e.ColumnIndex == dgvViolations.Columns["ImageUrl"].Index)
            {
                string url = dgvViolations.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();

                // Mở link bằng trình duyệt mặc định
                if (!string.IsNullOrEmpty(url) && url.StartsWith("http"))
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Không thể mở link bằng chứng: " + ex.Message);
                    }
                }
            }
        }
        private async void LoadData()
        {
            try
            {
                // Load questions
                _questions = await _examService.GetQuestionsByExamIdAsync(_exam.ExamId);

                // ===== ĐOẠN CODE ĐỌC ĐÁP ÁN ĐÃ ĐƯỢC SỬA =====
                if (!string.IsNullOrEmpty(_examResult.AnswersData))
                {
                    try
                    {
                        // Sửa int thành string để khớp với JSON
                        _studentAnswers = JsonSerializer.Deserialize<Dictionary<string, string>>(_examResult.AnswersData);
                    }
                    catch (JsonException) // Bắt lỗi cụ thể để biết lỗi do JSON
                    {
                        // Nếu JSON bị lỗi, coi như học sinh không trả lời câu nào
                        _studentAnswers = new Dictionary<string, string>();
                    }
                }
                else
                {
                    _studentAnswers = new Dictionary<string, string>();
                }
                // Đổ dữ liệu vi phạm vào bảng dgvViolations
                if (_examResult.ViolationLogs != null && _examResult.ViolationLogs.Any())
                {
                    var violationData = _examResult.ViolationLogs.Select(v => new
                    {
                        v.Timestamp,
                        v.ViolationType,
                        ImageUrl = string.IsNullOrEmpty(v.ImageUrl) ? "(không có)" : v.ImageUrl
                    }).OrderBy(v => v.Timestamp).ToList();

                    dgvViolations.DataSource = violationData;
                }
                // ===========================================

                RefreshAnswersGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }
       
        private void RefreshAnswersGrid()
        {
            // Đảm bảo dgv không bị kẹt dữ liệu cũ
            dgvAnswers.DataSource = null;

            if (_questions == null) return;

            var data = new List<dynamic>();
            for (int i = 0; i < _questions.Count; i++)
            {
                var question = _questions[i];
                string studentAnswer = "Không trả lời";
                string status = "❌ Sai";

                // Kiểm tra an toàn: studentAnswers không null và có chứa ID (dạng chuỗi)
                if (_studentAnswers != null && _studentAnswers.ContainsKey(question.QuestionId.ToString()))
                {
                    studentAnswer = _studentAnswers[question.QuestionId.ToString()];

                    // So sánh đáp án (nên Trim để tránh khoảng trắng thừa)
                    if (string.Equals(studentAnswer?.Trim(), question.CorrectOption?.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        status = "✅ Đúng";
                    }
                }

                // Tạo đối tượng dynamic với tên thuộc tính KHỚP HOÀN TOÀN với DataPropertyName ở Bước 1
                data.Add(new
                {
                    QuestionId = question.QuestionId,
                    QuestionNum = i + 1,
                    Content = question.Content,
                    StudentAnswer = studentAnswer,
                    CorrectAnswer = question.CorrectOption, // Tên này phải khớp DataPropertyName
                    Status = status
                });
            }

            dgvAnswers.DataSource = data;

            // Tô màu cột Kết quả
            foreach (DataGridViewRow row in dgvAnswers.Rows)
            {
                var statusCell = row.Cells["Status"];
                if (statusCell.Value != null)
                {
                    statusCell.Style.ForeColor = statusCell.Value.ToString().Contains("Đúng") ? Color.Green : Color.Red;
                }
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

                    // Thêm .ToString() để chuyển ID từ số sang chữ
                    if (_studentAnswers.ContainsKey(question.QuestionId.ToString()))
                    {
                        // Thêm .ToString() ở đây nữa
                        studentAnswer = _studentAnswers[question.QuestionId.ToString()];
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
