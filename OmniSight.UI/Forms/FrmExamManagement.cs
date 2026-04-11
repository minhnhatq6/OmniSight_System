using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using OmniSight.Core.Entities;
using OmniSight.Services;

namespace OmniSight.UI.Forms
{
    public partial class FrmExamManagement : MaterialForm
    {
        private readonly ExamService _examService;
        private readonly ClassService _classService;
        private readonly int _teacherId;

        private List<Exam> _exams = new List<Exam>();
        private List<ExamResult> _currentExamResults = new List<ExamResult>();
        private Exam _selectedExam;
        private MaterialButton btnExportExcel;
        // UI Components - Main Layout
        private MaterialTabControl tabControlMain;
        private MaterialTabSelector tabSelector;
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

            // 1. Khởi tạo MaterialSkinManager (Giao diện hiện đại)
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);

            // 2. Thiết lập toàn bộ giao diện kết hợp
            InitializeCustomComponents();

            // 3. Tải dữ liệu ban đầu
            LoadExamsAsync();
        }

        private void InitializeCustomComponents()
        {
            this.Text = "QUẢN LÝ BÀI THI & KẾT QUẢ";
            this.Size = new Size(1200, 750);
            this.StartPosition = FormStartPosition.CenterParent;

            // --- TAB CONTROL ---
            tabControlMain = new MaterialTabControl { Dock = DockStyle.Fill };

            tabExamList = new TabPage { Text = "Danh Sách Bài Thi", BackColor = Color.White };
            tabExamDetail = new TabPage { Text = "Chi Tiết & Câu Hỏi", BackColor = Color.White };
            tabResults = new TabPage { Text = "Kết Quả Học Sinh", BackColor = Color.White };

            // --- TAB SELECTOR (Thanh menu ngang Material) ---
            tabSelector = new MaterialTabSelector
            {
                BaseTabControl = tabControlMain,
                Dock = DockStyle.Top,
                Height = 48
            };

            // Khởi tạo các thành phần bên trong từng Tab
            SetupExamListTab();
            SetupExamDetailTab();
            SetupResultsTab();

            // Thêm Tab vào Control
            tabControlMain.TabPages.Add(tabExamList);
            tabControlMain.TabPages.Add(tabExamDetail);
            tabControlMain.TabPages.Add(tabResults);

            this.Controls.Add(tabControlMain);
            this.Controls.Add(tabSelector);
        }

        #region UI SETUP TỪNG TAB

        private void SetupExamListTab()
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
            btnRefresh.Click += (s, e) => LoadExamsAsync();

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
                GridColor = Color.FromArgb(200, 200, 200),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            dgvExams.Columns.AddRange(
                new DataGridViewTextBoxColumn { Name = "ExamId", DataPropertyName = "ExamId", HeaderText = "ID", Visible = false },
                new DataGridViewTextBoxColumn { Name = "Title", DataPropertyName = "Title", HeaderText = "Tên Bài Thi" },
                new DataGridViewTextBoxColumn { Name = "ClassName", DataPropertyName = "ClassName", HeaderText = "Lớp Học" },
                new DataGridViewTextBoxColumn { Name = "DurationMinutes", DataPropertyName = "DurationMinutes", HeaderText = "Thời Gian (phút)" },
                new DataGridViewTextBoxColumn { Name = "Questions", DataPropertyName = "Questions", HeaderText = "Số Câu" },
                new DataGridViewTextBoxColumn { Name = "Submissions", DataPropertyName = "Submissions", HeaderText = "Số Bài Nộp" },
                new DataGridViewTextBoxColumn { Name = "CreatedAt", DataPropertyName = "CreatedAt", HeaderText = "Ngày Tạo", DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" } }
            );

            dgvExams.CellDoubleClick += DgvExams_CellDoubleClick;

            panel.Controls.Add(dgvExams);
            panel.Controls.Add(toolbar);
            tabExamList.Controls.Add(panel);
        }

        private void SetupExamDetailTab()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(10) };

            // Exam Info
            var lblTitle = new MaterialLabel { Text = "Tên Bài Thi:", Location = new Point(10, 10), AutoSize = true };
            txtExamTitle = new MaterialTextBox { Location = new Point(150, 10), Width = 300, Hint = "Nhập tên bài thi" };

            var lblClass = new MaterialLabel { Text = "Lớp Học:", Location = new Point(10, 60), AutoSize = true };
            cbClasses = new ComboBox { Location = new Point(150, 60), Width = 300, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Roboto", 11) };

            var lblDuration = new MaterialLabel { Text = "Thời Gian (phút):", Location = new Point(10, 110), AutoSize = true };
            nudDuration = new NumericUpDown { Location = new Point(150, 110), Width = 100, Minimum = 5, Maximum = 180, Value = 60, Font = new Font("Roboto", 11) };

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
                BorderStyle = BorderStyle.FixedSingle,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            dgvQuestions.Columns.AddRange(
                new DataGridViewTextBoxColumn { Name = "QuestionId", DataPropertyName = "QuestionId", HeaderText = "ID", Visible = false },
                new DataGridViewTextBoxColumn { Name = "Content", DataPropertyName = "Content", HeaderText = "Nội Dung" },
                new DataGridViewTextBoxColumn { Name = "CorrectOption", DataPropertyName = "CorrectOption", HeaderText = "Đáp Án" }
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

            lblExamInfo = new MaterialLabel { Location = new Point(10, 560), AutoSize = true, Text = "Chọn bài thi để xem chi tiết", ForeColor = Color.Gray };

            panel.Controls.AddRange(new Control[] { lblTitle, txtExamTitle, lblClass, cbClasses, lblDuration, nudDuration, lblQuestions, dgvQuestions, questionToolbar, btnSaveExam, btnCancelExam, lblExamInfo });
            tabExamDetail.Controls.Add(panel);
        }

        private void SetupResultsTab()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };

            // Panel chứa bộ lọc phía trên
            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(245, 245, 245),
                Padding = new Padding(10)
            };

            MaterialLabel lblFilter = new MaterialLabel
            {
                Text = "Chọn bài thi:",
                Location = new Point(20, 25),
                AutoSize = true
            };

            cbExamFilter = new ComboBox
            {
                Location = new Point(130, 22),
                Width = 350,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Roboto", 12)
            };
            cbExamFilter.SelectedIndexChanged += CbExamFilter_SelectedIndexChanged;

            // Nút Xem chi tiết
            btnViewDetail = new MaterialButton
            {
                Text = "👁️ XEM CHI TIẾT BÀI LÀM",
                Location = new Point(500, 18),
                Size = new Size(220, 36),
                Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained,
                UseAccentColor = true,
                AutoSize = false
            };
            btnViewDetail.Click += BtnViewDetail_Click;

            // === CODE MỚI THÊM: NÚT XUẤT EXCEL ===
            btnExportExcel = new MaterialButton
            {
                Text = "📥 XUẤT EXCEL",
                Location = new Point(730, 18), // Đặt bên cạnh nút Xem chi tiết
                Size = new Size(150, 36),
                Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined,
                AutoSize = false
            };
            btnExportExcel.Click += BtnExportExcel_Click;
            // ======================================

            pnlHeader.Controls.Add(lblFilter);
            pnlHeader.Controls.Add(cbExamFilter);
            pnlHeader.Controls.Add(btnViewDetail);
            pnlHeader.Controls.Add(btnExportExcel); // Add nút vào panel

            // Bảng kết quả (Giữ nguyên của bạn)
            dgvResults = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                GridColor = Color.FromArgb(230, 230, 230)
            };

            dgvResults.Columns.AddRange(
                new DataGridViewTextBoxColumn { Name = "StudentName", DataPropertyName = "StudentName", HeaderText = "Học Sinh" },
                new DataGridViewTextBoxColumn { Name = "Score", DataPropertyName = "Score", HeaderText = "Điểm", DefaultCellStyle = new DataGridViewCellStyle { Format = "F1" } },
                new DataGridViewTextBoxColumn { Name = "ViolationCount", DataPropertyName = "ViolationCount", HeaderText = "Số Lần Vi Phạm" },
                new DataGridViewTextBoxColumn { Name = "StartedAt", DataPropertyName = "StartedAt", HeaderText = "Bắt Đầu", DefaultCellStyle = new DataGridViewCellStyle { Format = "HH:mm dd/MM" } },
                new DataGridViewTextBoxColumn { Name = "CompletedAt", DataPropertyName = "CompletedAt", HeaderText = "Kết Thúc (Nộp Bài)", DefaultCellStyle = new DataGridViewCellStyle { Format = "HH:mm dd/MM" } }
            );
            dgvResults.CellFormatting += DgvResults_CellFormatting;
            panel.Controls.Add(dgvResults);
            panel.Controls.Add(pnlHeader);
            tabResults.Controls.Add(panel);
        }

        #endregion

        #region LOGIC XỬ LÝ DỮ LIỆU VÀ SỰ KIỆN

        private async Task LoadExamsAsync()
        {
            try
            {
                _exams = await _examService.GetExamsForTeacherAsync(_teacherId);

                var displayData = _exams.Select(e => new
                {
                    e.ExamId,
                    e.Title,
                    // Lấy trực tiếp từ Class đã được Include, không cần gọi Service nữa
                    ClassName = e.Class?.ClassName ?? "N/A",
                    e.DurationMinutes,
                    Questions = e.Questions?.Count ?? 0,    // Bây giờ sẽ hiện đúng số
                    Submissions = e.ExamResults?.Count ?? 0, // Bây giờ sẽ hiện đúng số
                    e.CreatedAt
                }).ToList();

                dgvExams.DataSource = null;
                dgvExams.DataSource = displayData;

                // Cập nhật ComboBox bộ lọc (giữ nguyên)
                cbExamFilter.DataSource = null;
                cbExamFilter.DataSource = _exams;
                cbExamFilter.DisplayMember = "Title";
                cbExamFilter.ValueMember = "ExamId";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu bài thi: " + ex.Message);
            }
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

        // --- CÁC SỰ KIỆN CỦA TAB 1 & 2 (CRUD BÀI THI) ---

        private async void BtnCreateExam_Click(object sender, EventArgs e)
        {
            _selectedExam = null;
            ClearExamDetail();
            await LoadClassesAsync();
            tabControlMain.SelectedTab = tabExamDetail; // Chuyển sang tab chi tiết
            lblExamInfo.Text = "Trạng thái: Đang tạo bài thi mới";
        }

        private async void BtnEditExam_Click(object sender, EventArgs e)
        {
            if (dgvExams.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn bài thi để sửa");
                return;
            }

            var examId = (int)dgvExams.SelectedRows[0].Cells["ExamId"].Value;
            _selectedExam = _exams.FirstOrDefault(ex => ex.ExamId == examId);

            if (_selectedExam != null)
            {
                txtExamTitle.Text = _selectedExam.Title;
                nudDuration.Value = _selectedExam.DurationMinutes;
                await LoadClassesAsync();
                cbClasses.SelectedValue = _selectedExam.ClassId;

                // Load questions
                var questions = await _examService.GetQuestionsByExamIdAsync(examId);
                dgvQuestions.DataSource = questions;

                tabControlMain.SelectedTab = tabExamDetail; // Chuyển sang tab chi tiết
                lblExamInfo.Text = $"Trạng thái: Đang sửa bài thi '{_selectedExam.Title}'";
            }
        }

        private void DgvExams_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) BtnEditExam_Click(null, null);
        }

        private async void BtnDeleteExam_Click(object sender, EventArgs e)
        {
            if (dgvExams.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn bài thi để xóa");
                return;
            }

            var result = MessageBox.Show("Bạn chắc chắn muốn xóa bài thi này?\n(Tất cả kết quả của học sinh sẽ bị xóa)", "Xác Nhận Xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                var examId = (int)dgvExams.SelectedRows[0].Cells["ExamId"].Value;
                try
                {
                    await _examService.DeleteExamAsync(examId);
                    this.DialogResult = DialogResult.OK;
                    MessageBox.Show("Xóa bài thi thành công");
                    await LoadExamsAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xóa bài thi: " + ex.Message);
                }
            }
        }

        private async void BtnSaveExam_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtExamTitle.Text))
            {
                MessageBox.Show("Vui lòng nhập tên bài thi"); return;
            }
            if (cbClasses.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn lớp học"); return;
            }

            try
            {
                if (_selectedExam == null)
                {
                    var classId = (int)cbClasses.SelectedValue;
                    await _examService.CreateExamAsync(classId, txtExamTitle.Text, (int)nudDuration.Value);
                    this.DialogResult = DialogResult.OK;
                    MessageBox.Show("Tạo bài thi thành công!");
                }
                else
                {
                    _selectedExam.Title = txtExamTitle.Text;
                    _selectedExam.DurationMinutes = (int)nudDuration.Value;
                    await _examService.UpdateExamAsync(_selectedExam);
                    this.DialogResult = DialogResult.OK;
                    MessageBox.Show("Cập nhật bài thi thành công!");
                }

                await LoadExamsAsync();
                tabControlMain.SelectedTab = tabExamList;
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
            tabControlMain.SelectedTab = tabExamList;
        }

        private void ClearExamDetail()
        {
            txtExamTitle.Text = "";
            cbClasses.SelectedIndex = -1;
            nudDuration.Value = 60;
            dgvQuestions.DataSource = null;
            _selectedExam = null;
        }

        // --- CÁC SỰ KIỆN QUẢN LÝ CÂU HỎI (TRONG TAB 2) ---

        private async void BtnAddQuestion_Click(object sender, EventArgs e)
        {
            if (_selectedExam == null)
            {
                MessageBox.Show("Vui lòng tạo hoặc chọn bài thi trước khi thêm câu hỏi!");
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
            if (dgvQuestions.SelectedRows.Count == 0) return;

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
            if (dgvQuestions.SelectedRows.Count == 0) return;

            if (MessageBox.Show("Xóa câu hỏi này?", "Xác Nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
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

        // --- CÁC SỰ KIỆN CỦA TAB 3 (KẾT QUẢ & XEM CHI TIẾT) ---

        private async void CbExamFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbExamFilter.SelectedItem == null) return;
            var selectedExam = cbExamFilter.SelectedItem as Exam;
            if (selectedExam == null) return;

            try
            {
                // Lấy danh sách kết quả lưu vào biến tạm
                _currentExamResults = await _examService.GetExamResultsAsync(selectedExam.ExamId);

                // Bind lên DataGrid
                var displayData = _currentExamResults.Select(r => new
                {
                    StudentName = r.Student?.FullName ?? "N/A",
                    r.Score,
                    ViolationCount = r.ViolationLogs != null ? r.ViolationLogs.Count : 0,
                    r.StartedAt,
                    r.CompletedAt
                }).ToList();

                dgvResults.DataSource = displayData;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải kết quả: " + ex.Message);
            }
        }
        private void DgvResults_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Tìm đến cột "Số Lần Vi Phạm"
            if (e.ColumnIndex == dgvResults.Columns["ViolationCount"].Index && e.Value != null)
            {
                if (int.TryParse(e.Value.ToString(), out int violationCount) && violationCount > 0)
                {
                    // Nếu có vi phạm, tô chữ màu đỏ và in đậm
                    e.CellStyle.ForeColor = Color.Red;
                    e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Bold);
                }
            }
        }
        private async void BtnViewDetail_Click(object sender, EventArgs e) // Nên đổi sang async
        {
            if (dgvResults.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một học sinh trong bảng kết quả!");
                return;
            }

            try
            {
                // === SỬA TỪ ĐÂY ===

                // 1. Lấy StudentId và ExamId từ dòng đang chọn
                int rowIndex = dgvResults.SelectedRows[0].Index;
                if (_currentExamResults == null || rowIndex >= _currentExamResults.Count) return;

                var selectedResultSummary = _currentExamResults[rowIndex]; // Đây là bản tóm tắt, có thể cũ
                var selectedExam = cbExamFilter.SelectedItem as Exam;

                if (selectedResultSummary == null || selectedExam == null) return;

                // 2. LẤY LẠI BẢN GHI ĐẦY ĐỦ VÀ MỚI NHẤT TỪ DATABASE
                var fullExamResult = await _examService.GetExamResultByIdAsync(selectedResultSummary.ResultId);
                if (fullExamResult == null)
                {
                    MessageBox.Show("Không tìm thấy kết quả chi tiết trong cơ sở dữ liệu.");
                    return;
                }

                // 3. Mở form với dữ liệu MỚI NHẤT
                using (var frm = new FrmExamResultDetail(_examService, selectedExam, fullExamResult, fullExamResult.Student))
                {
                    frm.ShowDialog(this);
                }

                // === ĐẾN ĐÂY ===
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể xem chi tiết: " + ex.Message);
            }
        }

        #endregion

        // Chặn lỗi gạch đỏ nếu lỡ tay xóa file Designer
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
        }
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }
        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có dữ liệu trong bảng không
            if (dgvResults.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedExam = cbExamFilter.SelectedItem as Exam;
            string defaultFileName = selectedExam != null ? $"KetQua_{selectedExam.Title}.xlsx" : "KetQuaBaiThi.xlsx";

            // Mở hộp thoại để người dùng chọn nơi lưu file
            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel Workbook|*.xlsx", FileName = defaultFileName })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Sử dụng ClosedXML để tạo file Excel
                        using (var workbook = new ClosedXML.Excel.XLWorkbook())
                        {
                            var worksheet = workbook.Worksheets.Add("Kết Quả");

                            // 1. Ghi tiêu đề các cột (dòng 1)
                            for (int i = 0; i < dgvResults.Columns.Count; i++)
                            {
                                worksheet.Cell(1, i + 1).Value = dgvResults.Columns[i].HeaderText;
                                // Format cho header in đậm và có màu nền
                                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                                worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGray;
                            }

                            // 2. Ghi dữ liệu từ DataGridView (từ dòng 2 trở đi)
                            for (int i = 0; i < dgvResults.Rows.Count; i++)
                            {
                                for (int j = 0; j < dgvResults.Columns.Count; j++)
                                {
                                    // Lấy giá trị từng ô trên màn hình
                                    var cellValue = dgvResults.Rows[i].Cells[j].Value;
                                    worksheet.Cell(i + 2, j + 1).Value = cellValue != null ? cellValue.ToString() : "";
                                }
                            }

                            // Tự động căn chỉnh độ rộng cột cho đẹp
                            worksheet.Columns().AdjustToContents();

                            // Lưu file
                            workbook.SaveAs(sfd.FileName);
                        }

                        MessageBox.Show("Xuất file Excel thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Có lỗi xảy ra khi xuất file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}