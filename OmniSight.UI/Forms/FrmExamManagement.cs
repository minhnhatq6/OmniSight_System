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
    public partial class FrmExamManagement : Form
    {
        private readonly ExamService _examService;
        private readonly ClassService _classService;
        private readonly int _teacherId;
        private List<Exam> _exams;
        private Exam _selectedExam;

        private MaterialTabControl tabControl;
        private TabPage tabExamList;
        private TabPage tabExamDetail;
        private TabPage tabResults;

        // Tab 1: Danh sách bài thi
        private DataGridView dgvExams;
        private MaterialButton btnCreateExam;
        private MaterialButton btnEditExam;
        private MaterialButton btnDeleteExam;
        private MaterialButton btnRefresh;

        // Tab 2: Chi tiết bài thi
        private MaterialTextBox txtExamTitle;
        private NumericUpDown nudDuration;
        private ComboBox cbClasses;
        private DataGridView dgvQuestions;
        private MaterialButton btnAddQuestion;
        private MaterialButton btnEditQuestion;
        private MaterialButton btnDeleteQuestion;
        private MaterialButton btnSaveExam;
        private MaterialButton btnCancelExam;
        private MaterialLabel lblExamInfo;

        // Tab 3: Kết quả
        private DataGridView dgvResults;
        private MaterialButton btnViewDetail;
        private ComboBox cbExamFilter;

        public FrmExamManagement(ExamService examService, ClassService classService, int teacherId)
        {
            _examService = examService;
            _classService = classService;
            _teacherId = teacherId;
            _exams = new List<Exam>();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Quản lý Bài Thi";
            this.Width = 1200;
            this.Height = 700;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;

            // Tab Control
            tabControl = new MaterialTabControl
            {
                Dock = DockStyle.Fill,
                Name = "tabControl"
            };

            // Tab 1: Danh sách bài thi
            tabExamList = new TabPage { Text = "Danh Sách Bài Thi", Name = "tabExamList" };
            InitializeExamListTab();
            tabControl.TabPages.Add(tabExamList);

            // Tab 2: Chi tiết bài thi
            tabExamDetail = new TabPage { Text = "Chi Tiết Bài Thi", Name = "tabExamDetail" };
            InitializeExamDetailTab();
            tabControl.TabPages.Add(tabExamDetail);

            // Tab 3: Kết quả học sinh
            tabResults = new TabPage { Text = "Kết Quả Học Sinh", Name = "tabResults" };
            InitializeResultsTab();
            tabControl.TabPages.Add(tabResults);

            this.Controls.Add(tabControl);

            LoadExamsAsync();
        }

        private void InitializeExamListTab()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };

            // Toolbar
            var toolbar = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.FromArgb(245, 245, 245) };
            btnCreateExam = new MaterialButton { Text = "➕ Tạo Bài Thi", Location = new Point(10, 10), Width = 120 };
            btnCreateExam.Click += BtnCreateExam_Click;
            btnEditExam = new MaterialButton { Text = "✏️ Sửa", Location = new Point(140, 10), Width = 100 };
            btnEditExam.Click += BtnEditExam_Click;
            btnDeleteExam = new MaterialButton { Text = "🗑️ Xóa", Location = new Point(250, 10), Width = 100 };
            btnDeleteExam.Click += BtnDeleteExam_Click;
            btnRefresh = new MaterialButton { Text = "🔄 Tải Lại", Location = new Point(360, 10), Width = 100 };
            btnRefresh.Click += BtnRefresh_Click;

            toolbar.Controls.AddRange(new Control[] { btnCreateExam, btnEditExam, btnDeleteExam, btnRefresh });

            // DataGrid
            dgvExams = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                GridColor = Color.FromArgb(200, 200, 200)
            };

            dgvExams.Columns.AddRange(
                new DataGridViewTextBoxColumn { Name = "ExamId", DataPropertyName = "ExamId", HeaderText = "ID", Visible = false },
                new DataGridViewTextBoxColumn { Name = "Title", DataPropertyName = "Title", HeaderText = "Tên Bài Thi", Width = 300 },
                new DataGridViewTextBoxColumn { Name = "ClassName", HeaderText = "Lớp Học", Width = 150 },
                new DataGridViewTextBoxColumn { Name = "DurationMinutes", DataPropertyName = "DurationMinutes", HeaderText = "Thời Gian (phút)", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "Questions", HeaderText = "Số Câu", Width = 80 },
                new DataGridViewTextBoxColumn { Name = "Submissions", HeaderText = "Số Bài Nộp", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "CreatedAt", DataPropertyName = "CreatedAt", HeaderText = "Ngày Tạo", Width = 150, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" } }
            );

            dgvExams.CellClick += DgvExams_CellClick;
            dgvExams.CellDoubleClick += DgvExams_CellDoubleClick;

            panel.Controls.Add(dgvExams);
            panel.Controls.Add(toolbar);
            tabExamList.Controls.Add(panel);
        }

        private void InitializeExamDetailTab()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(10) };

            // Exam Info
            var lblTitle = new MaterialLabel { Text = "Tên Bài Thi:", Location = new Point(10, 10), AutoSize = true };
            txtExamTitle = new MaterialTextBox { Location = new Point(150, 10), Width = 300, Hint = "Nhập tên bài thi" };

            var lblClass = new MaterialLabel { Text = "Lớp Học:", Location = new Point(10, 60), AutoSize = true };
            cbClasses = new ComboBox { Location = new Point(150, 60), Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };

            var lblDuration = new MaterialLabel { Text = "Thời Gian (phút):", Location = new Point(10, 110), AutoSize = true };
            nudDuration = new NumericUpDown { Location = new Point(150, 110), Width = 100, Minimum = 5, Maximum = 180, Value = 60 };

            // Questions
            var lblQuestions = new MaterialLabel { Text = "📋 Câu Hỏi:", Location = new Point(10, 160), AutoSize = true, Font = new Font("Roboto", 11, FontStyle.Bold) };

            dgvQuestions = new DataGridView
            {
                Location = new Point(10, 190),
                Width = 760,
                Height = 250,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            dgvQuestions.Columns.AddRange(
                new DataGridViewTextBoxColumn { Name = "QuestionId", DataPropertyName = "QuestionId", HeaderText = "ID", Visible = false },
                new DataGridViewTextBoxColumn { Name = "Content", DataPropertyName = "Content", HeaderText = "Nội Dung", Width = 350 },
                new DataGridViewTextBoxColumn { Name = "CorrectOption", DataPropertyName = "CorrectOption", HeaderText = "Đáp Án", Width = 60 }
            );

            // Question Toolbar
            var questionToolbar = new Panel { Location = new Point(10, 450), Width = 760, Height = 40, BackColor = Color.FromArgb(245, 245, 245) };
            btnAddQuestion = new MaterialButton { Text = "➕ Thêm Câu", Location = new Point(10, 5), Width = 110 };
            btnAddQuestion.Click += BtnAddQuestion_Click;
            btnEditQuestion = new MaterialButton { Text = "✏️ Sửa Câu", Location = new Point(130, 5), Width = 110 };
            btnEditQuestion.Click += BtnEditQuestion_Click;
            btnDeleteQuestion = new MaterialButton { Text = "🗑️ Xóa Câu", Location = new Point(250, 5), Width = 110 };
            btnDeleteQuestion.Click += BtnDeleteQuestion_Click;

            questionToolbar.Controls.AddRange(new Control[] { btnAddQuestion, btnEditQuestion, btnDeleteQuestion });

            // Save/Cancel
            btnSaveExam = new MaterialButton { Text = "💾 Lưu Bài Thi", Location = new Point(10, 510), Width = 120, BackColor = Color.FromArgb(76, 175, 80), ForeColor = Color.White };
            btnSaveExam.Click += BtnSaveExam_Click;
            btnCancelExam = new MaterialButton { Text = "❌ Hủy", Location = new Point(140, 510), Width = 100 };
            btnCancelExam.Click += BtnCancelExam_Click;

            lblExamInfo = new MaterialLabel { Location = new Point(10, 540), AutoSize = true, Text = "Chọn bài thi để xem chi tiết", ForeColor = Color.Gray };

            panel.Controls.AddRange(new Control[] { lblTitle, txtExamTitle, lblClass, cbClasses, lblDuration, nudDuration, lblQuestions, dgvQuestions, questionToolbar, btnSaveExam, btnCancelExam, lblExamInfo });
            tabExamDetail.Controls.Add(panel);
        }

        private void InitializeResultsTab()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(10) };

            // Filter
            var lblFilter = new MaterialLabel { Text = "Bài Thi:", Location = new Point(10, 10), AutoSize = true };
            cbExamFilter = new ComboBox { Location = new Point(80, 10), Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
            cbExamFilter.SelectedIndexChanged += CbExamFilter_SelectedIndexChanged;

            // Results Grid
            dgvResults = new DataGridView
            {
                Location = new Point(10, 50),
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false
            };

            dgvResults.Columns.AddRange(
                new DataGridViewTextBoxColumn { Name = "StudentName", HeaderText = "Học Sinh", Width = 200 },
                new DataGridViewTextBoxColumn { Name = "Score", HeaderText = "Điểm", Width = 80, DefaultCellStyle = new DataGridViewCellStyle { Format = "F1" } },
                new DataGridViewTextBoxColumn { Name = "StartedAt", HeaderText = "Bắt Đầu", Width = 150, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" } },
                new DataGridViewTextBoxColumn { Name = "CompletedAt", HeaderText = "Kết Thúc", Width = 150, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" } },
                new DataGridViewTextBoxColumn { Name = "Duration", HeaderText = "Thời Gian (phút)", Width = 120 }
            );

            btnViewDetail = new MaterialButton { Text = "👁️ Xem Chi Tiết", Location = new Point(10, 10), Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnViewDetail.Click += BtnViewDetail_Click;

            panel.Controls.Add(cbExamFilter);
            panel.Controls.Add(lblFilter);
            panel.Controls.Add(dgvResults);
            tabResults.Controls.Add(panel);
        }

        private async Task LoadExamsAsync()
        {
            try
            {
                _exams = await _examService.GetExamsForTeacherAsync(_teacherId);
                RefreshExamGrid();
                LoadExamFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải bài thi: " + ex.Message);
            }
        }

        private void RefreshExamGrid()
        {
            dgvExams.DataSource = null;
            var data = _exams.Select(e => new
            {
                e.ExamId,
                e.Title,
                ClassName = _classService.GetClassNameAsync(e.ClassId).Result ?? "N/A",
                e.DurationMinutes,
                Questions = e.Questions?.Count ?? 0,
                Submissions = e.ExamResults?.Count ?? 0,
                e.CreatedAt
            }).ToList();

            dgvExams.DataSource = data;
        }

        private void LoadExamFilter()
        {
            cbExamFilter.DataSource = _exams;
            cbExamFilter.DisplayMember = "Title";
            cbExamFilter.ValueMember = "ExamId";
        }

        private async void BtnCreateExam_Click(object sender, EventArgs e)
        {
            _selectedExam = null;
            ClearExamDetail();
            await LoadClassesAsync();
            tabControl.SelectedTab = tabExamDetail;
            lblExamInfo.Text = "Tạo bài thi mới";
        }

        private async void BtnEditExam_Click(object sender, EventArgs e)
        {
            if (dgvExams.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn bài thi để sửa");
                return;
            }

            var examId = (int)dgvExams.SelectedRows[0].Cells["ExamId"].Value;
            _selectedExam = _exams.FirstOrDefault(e => e.ExamId == examId);

            if (_selectedExam != null)
            {
                txtExamTitle.Text = _selectedExam.Title;
                nudDuration.Value = _selectedExam.DurationMinutes;
                await LoadClassesAsync();
                cbClasses.SelectedValue = _selectedExam.ClassId;

                // Load questions
                var questions = await _examService.GetQuestionsByExamIdAsync(examId);
                dgvQuestions.DataSource = questions;

                tabControl.SelectedTab = tabExamDetail;
                lblExamInfo.Text = $"Sửa bài thi: {_selectedExam.Title}";
            }
        }

        private async void BtnDeleteExam_Click(object sender, EventArgs e)
        {
            if (dgvExams.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn bài thi để xóa");
                return;
            }

            var result = MessageBox.Show("Bạn chắc chắn muốn xóa bài thi này?\n(Tất cả kết quả sẽ bị xóa)", "Xác Nhận Xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                var examId = (int)dgvExams.SelectedRows[0].Cells["ExamId"].Value;
                try
                {
                    await _examService.DeleteExamAsync(examId);
                    MessageBox.Show("Xóa bài thi thành công");
                    await LoadExamsAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xóa bài thi: " + ex.Message);
                }
            }
        }

        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            await LoadExamsAsync();
        }

        private async void BtnSaveExam_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtExamTitle.Text))
            {
                MessageBox.Show("Vui lòng nhập tên bài thi");
                return;
            }

            if (cbClasses.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn lớp học");
                return;
            }

            try
            {
                if (_selectedExam == null)
                {
                    // Create new exam
                    var classId = (int)cbClasses.SelectedValue;
                    await _examService.CreateExamAsync(classId, txtExamTitle.Text, (int)nudDuration.Value);
                    MessageBox.Show("Tạo bài thi thành công");
                }
                else
                {
                    // Update exam
                    _selectedExam.Title = txtExamTitle.Text;
                    _selectedExam.DurationMinutes = (int)nudDuration.Value;
                    await _examService.UpdateExamAsync(_selectedExam);
                    MessageBox.Show("Cập nhật bài thi thành công");
                }

                await LoadExamsAsync();
                tabControl.SelectedTab = tabExamList;
                ClearExamDetail();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu bài thi: " + ex.Message);
            }
        }

        private void BtnCancelExam_Click(object sender, EventArgs e)
        {
            ClearExamDetail();
            tabControl.SelectedTab = tabExamList;
        }

        private void ClearExamDetail()
        {
            txtExamTitle.Text = "";
            cbClasses.SelectedIndex = -1;
            nudDuration.Value = 60;
            dgvQuestions.DataSource = null;
            _selectedExam = null;
        }

        private async void BtnAddQuestion_Click(object sender, EventArgs e)
        {
            if (_selectedExam == null)
            {
                MessageBox.Show("Vui lòng tạo hoặc chọn bài thi trước");
                return;
            }

            using (var frm = new FrmQuestionEditor(_examService, _selectedExam.ExamId))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    var questions = await _examService.GetQuestionsByExamIdAsync(_selectedExam.ExamId);
                    dgvQuestions.DataSource = questions;
                }
            }
        }

        private async void BtnEditQuestion_Click(object sender, EventArgs e)
        {
            if (dgvQuestions.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn câu hỏi để sửa");
                return;
            }

            var questionId = (int)dgvQuestions.SelectedRows[0].Cells["QuestionId"].Value;
            using (var frm = new FrmQuestionEditor(_examService, _selectedExam.ExamId, questionId))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    var questions = await _examService.GetQuestionsByExamIdAsync(_selectedExam.ExamId);
                    dgvQuestions.DataSource = questions;
                }
            }
        }

        private async void BtnDeleteQuestion_Click(object sender, EventArgs e)
        {
            if (dgvQuestions.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn câu hỏi để xóa");
                return;
            }

            var result = MessageBox.Show("Xóa câu hỏi này?", "Xác Nhận", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                var questionId = (int)dgvQuestions.SelectedRows[0].Cells["QuestionId"].Value;
                try
                {
                    await _examService.DeleteQuestionAsync(questionId);
                    var questions = await _examService.GetQuestionsByExamIdAsync(_selectedExam.ExamId);
                    dgvQuestions.DataSource = questions;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xóa câu hỏi: " + ex.Message);
                }
            }
        }

        private void DgvExams_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Highlight selected row
            }
        }

        private async void DgvExams_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                BtnEditExam_Click(null, null);
            }
        }

        private async void CbExamFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbExamFilter.SelectedValue == null) return;

            var examId = (int)cbExamFilter.SelectedValue;
            try
            {
                var results = await _examService.GetExamResultsAsync(examId);
                var data = results.Select(r => new
                {
                    StudentName = r.Student?.FullName ?? "N/A",
                    r.Score,
                    r.StartedAt,
                    r.CompletedAt,
                    Duration = r.CompletedAt.HasValue && r.StartedAt.HasValue 
                        ? (int)(r.CompletedAt.Value - r.StartedAt.Value).TotalMinutes 
                        : 0
                }).ToList();

                dgvResults.DataSource = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải kết quả: " + ex.Message);
            }
        }

        private async void BtnViewDetail_Click(object sender, EventArgs e)
        {
            if (dgvResults.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn kết quả để xem chi tiết");
                return;
            }

            // Implement detailed result view
            MessageBox.Show("Tính năng xem chi tiết kết quả sẽ được triển khai");
        }

        private async Task LoadClassesAsync()
        {
            try
            {
                var classes = await _classService.GetOwnedClassesAsync(_teacherId);
                cbClasses.DataSource = classes;
                cbClasses.DisplayMember = "ClassName";
                cbClasses.ValueMember = "ClassId";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách lớp: " + ex.Message);
            }
        }
    }
}
