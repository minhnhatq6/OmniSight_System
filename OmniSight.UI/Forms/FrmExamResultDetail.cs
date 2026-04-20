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
            this.Width = 950;
            this.Height = 750;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;
            this.Font = new Font("Roboto", 10);

            // ==========================================
            // 1. HEADER - Thông tin tổng quan
            // ==========================================
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 110,
                BackColor = Color.FromArgb(25, 118, 210),
                Padding = new Padding(20, 10, 20, 10)
            };

            lblExamName = new MaterialLabel
            {
                Text = $"📋  {_exam.Title}",
                Font = new Font("Roboto", 15, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 10)
            };

            lblStudentName = new MaterialLabel
            {
                Text = $"👤  {_student?.FullName ?? "Unknown"}",
                Font = new Font("Roboto", 11),
                ForeColor = Color.FromArgb(220, 235, 255),
                AutoSize = true,
                Location = new Point(20, 45)
            };

            lblScore = new MaterialLabel
            {
                Text = $"🎯  {_examResult?.Score:F1} / 10",
                Font = new Font("Roboto", 13, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 235, 59),
                AutoSize = true,
                Location = new Point(20, 75)
            };

            lblTimeInfo = new MaterialLabel
            {
                Text = $"⏱️  {_examResult?.StartedAt:HH:mm dd/MM/yyyy}  →  {_examResult?.CompletedAt:HH:mm dd/MM/yyyy}",
                Font = new Font("Roboto", 10),
                ForeColor = Color.FromArgb(200, 225, 255),
                AutoSize = true,
                Location = new Point(500, 75)
            };

            pnlHeader.Controls.AddRange(new Control[] { lblExamName, lblStudentName, lblScore, lblTimeInfo });

            // ==========================================
            // 2. BOTTOM BUTTONS
            // ==========================================
            var pnlButtons = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 55,
                BackColor = Color.FromArgb(245, 245, 245),
                Padding = new Padding(15, 10, 15, 10)
            };

            var line = new Panel
            {
                Dock = DockStyle.Top,
                Height = 1,
                BackColor = Color.FromArgb(220, 220, 220)
            };
            pnlButtons.Controls.Add(line);

            btnExportExcel = new MaterialButton
            {
                Text = "📊 XUẤT EXCEL",
                Location = new Point(15, 10),
                AutoSize = true,
                Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined
            };
            btnExportExcel.Click += BtnExportExcel_Click;

            btnExportPDF = new MaterialButton
            {
                Text = "📄 XUẤT PDF",
                Location = new Point(155, 10),
                AutoSize = true,
                Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined
            };
            

            btnClose = new MaterialButton
            {
                Text = "ĐÓNG",
                Location = new Point(820, 10),
                AutoSize = true,
                Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained,
                UseAccentColor = false
            };
            btnClose.Click += (s, e) => this.Close();

            pnlButtons.Controls.AddRange(new Control[] { btnExportExcel, btnClose });

            // ==========================================
            // 3. BODY - Chia đôi: Bảng câu hỏi + Vi phạm
            // ==========================================
            var pnlBody = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15, 10, 15, 5),
                BackColor = Color.White
            };

            // -- PHẦN DƯỚI: Vi phạm (chiều cao cố định) --
            var pnlViolation = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 200,
                BackColor = Color.White
            };

            var lblViolationTitle = new Label
            {
                Text = "⚠️  Nhật Ký Vi Phạm",
                Font = new Font("Roboto", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(211, 47, 47),
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(0, 5, 0, 0)
            };

            dgvViolations = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                GridColor = Color.FromArgb(230, 230, 230),
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 36,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(255, 243, 243),
                    ForeColor = Color.FromArgb(183, 28, 28),
                    Font = new Font("Roboto", 10, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(5, 0, 0, 0)
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Roboto", 10),
                    Padding = new Padding(5, 0, 0, 0),
                    SelectionBackColor = Color.FromArgb(255, 235, 235),
                    SelectionForeColor = Color.Black
                },
                RowTemplate = { Height = 34 }
            };

            dgvViolations.Columns.AddRange(
                new DataGridViewTextBoxColumn
                {
                    Name = "Timestamp",
                    DataPropertyName = "Timestamp",
                    HeaderText = "Thời Gian",
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "HH:mm:ss  dd/MM" },
                    FillWeight = 20
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "ViolationType",
                    DataPropertyName = "ViolationType",
                    HeaderText = "Hành Vi Vi Phạm",
                    FillWeight = 50
                },
                new DataGridViewLinkColumn
                {
                    Name = "ImageUrl",
                    DataPropertyName = "ImageUrl",
                    HeaderText = "Bằng Chứng",
                    FillWeight = 30,
                    LinkColor = Color.FromArgb(25, 118, 210),
                    TrackVisitedState = false
                }
            );
            dgvViolations.CellContentClick += DgvViolations_CellContentClick;

            pnlViolation.Controls.Add(dgvViolations);
            pnlViolation.Controls.Add(lblViolationTitle);

            // Đường kẻ ngăn cách 2 bảng
            var separator = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 10,
                BackColor = Color.White
            };

            // -- PHẦN TRÊN: Bảng câu hỏi (lấp đầy còn lại) --
            var lblAnswerTitle = new Label
            {
                Text = "📝  Chi Tiết Câu Trả Lời",
                Font = new Font("Roboto", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 118, 210),
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(0, 5, 0, 0)
            };

            dgvAnswers = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false,
                GridColor = Color.FromArgb(230, 230, 230),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 40,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(227, 242, 253),
                    ForeColor = Color.FromArgb(13, 71, 161),
                    Font = new Font("Roboto", 10, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(5, 0, 0, 0)
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Roboto", 10),
                    Padding = new Padding(5, 0, 0, 0),
                    SelectionBackColor = Color.FromArgb(227, 242, 253),
                    SelectionForeColor = Color.Black
                },
                RowTemplate = { Height = 40 }
            };

            dgvAnswers.Columns.AddRange(
                new DataGridViewTextBoxColumn
                {
                    Name = "QuestionId",
                    DataPropertyName = "QuestionId",
                    Visible = false
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "QuestionNum",
                    DataPropertyName = "QuestionNum",
                    HeaderText = "Câu",
                    FillWeight = 8,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Alignment = DataGridViewContentAlignment.MiddleCenter,
                        Font = new Font("Roboto", 10, FontStyle.Bold)
                    }
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "Content",
                    DataPropertyName = "Content",
                    HeaderText = "Nội Dung Câu Hỏi",
                    FillWeight = 50
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "StudentAnswer",
                    DataPropertyName = "StudentAnswer",
                    HeaderText = "Đáp Án Học Sinh",
                    FillWeight = 17,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Alignment = DataGridViewContentAlignment.MiddleCenter,
                        Font = new Font("Roboto", 11, FontStyle.Bold)
                    }
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "CorrectAnswer",
                    DataPropertyName = "CorrectAnswer",
                    HeaderText = "Đáp Án Đúng",
                    FillWeight = 17,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Alignment = DataGridViewContentAlignment.MiddleCenter,
                        Font = new Font("Roboto", 11, FontStyle.Bold),
                        ForeColor = Color.FromArgb(46, 125, 50)
                    }
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "Status",
                    DataPropertyName = "Status",
                    HeaderText = "Kết Quả",
                    FillWeight = 13,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Alignment = DataGridViewContentAlignment.MiddleCenter,
                        Font = new Font("Roboto", 10, FontStyle.Bold)
                    }
                }
            );

            pnlBody.Controls.Add(dgvAnswers);
            pnlBody.Controls.Add(lblAnswerTitle);
            pnlBody.Controls.Add(separator);
            pnlBody.Controls.Add(pnlViolation);

            // ==========================================
            // LẮP RÁP FORM
            // ==========================================
            this.Controls.Add(pnlBody);
            this.Controls.Add(pnlButtons);
            this.Controls.Add(pnlHeader);
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
