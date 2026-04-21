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
using Xceed.Words.NET;

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
        private List<ResultRowViewModel> _resultRows = new List<ResultRowViewModel>();

        // UI Components
        private MaterialTabControl tabControlMain;
        private MaterialTabSelector tabSelector;
        private TabPage tabExamList, tabExamDetail, tabResults;

        // Tab 1: Danh sách bài thi
        private DataGridView dgvExams;
        private MaterialButton btnCreateExam, btnEditExam, btnDeleteExam, btnRefresh, btnImportWord;

        // Tab 2: Chi tiết bài thi
        private MaterialTextBox txtExamTitle;
        private NumericUpDown nudDuration;
        private ComboBox cbClasses;
        private DateTimePicker dtpStartTime, dtpEndTime;
        private DataGridView dgvQuestions;
        private MaterialButton btnAddQuestion, btnEditQuestion, btnDeleteQuestion, btnSaveExam, btnCancelExam;
        private MaterialLabel lblExamInfo;

        // Tab 3: Kết quả
        private DataGridView dgvResults;
        private MaterialButton btnViewDetail, btnExportExcel;
        private ComboBox cbExamFilter;
        private TextBox txtResultSearch;

        private sealed class ResultRowViewModel
        {
            public string StudentName { get; set; }
            public float? Score { get; set; }
            public int ViolationCount { get; set; }
            public DateTime? StartedAt { get; set; }
            public DateTime? CompletedAt { get; set; }
            public string RetakeStatusDisplay { get; set; }
            public string ActionText { get; set; }
        }

        public FrmExamManagement(ExamService examService, ClassService classService, int teacherId)
        {
            _examService = examService;
            _classService = classService;
            _teacherId = teacherId;

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);

            InitializeCustomComponents();
            ApplyThemeToStandardControls(); // Ép màu Theme ngay khi khởi tạo
            LoadExamsAsync();
        }

        private void InitializeCustomComponents()
        {
            this.Text = "QUẢN LÝ BÀI THI & KẾT QUẢ";
            this.Size = new Size(1200, 750);
            this.StartPosition = FormStartPosition.CenterParent;

            tabControlMain = new MaterialTabControl { Dock = DockStyle.Fill };
            tabExamList = new TabPage { Text = "Danh Sách Bài Thi" };
            tabExamDetail = new TabPage { Text = "Chi Tiết & Câu Hỏi" };
            tabResults = new TabPage { Text = "Kết Quả Học Sinh" };

            tabSelector = new MaterialTabSelector
            {
                BaseTabControl = tabControlMain,
                Dock = DockStyle.Top,
                Height = 48
            };

            SetupExamListTab();
            SetupExamDetailTab();
            SetupResultsTab();

            tabControlMain.TabPages.Add(tabExamList);
            tabControlMain.TabPages.Add(tabExamDetail);
            tabControlMain.TabPages.Add(tabResults);

            this.Controls.Add(tabControlMain);
            this.Controls.Add(tabSelector);
        }

        #region SETUP GIAO DIỆN

        private void SetupExamListTab()
        {
            var panel = new Panel { Dock = DockStyle.Fill };

            var toolbar = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(15, 12, 15, 10),
                WrapContents = false
            };

            btnCreateExam = new MaterialButton { Text = "TẠO BÀI THI", AutoSize = true };
            btnCreateExam.Click += BtnCreateExam_Click;

            btnEditExam = new MaterialButton { Text = "SỬA", AutoSize = true, Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined };
            btnEditExam.Click += BtnEditExam_Click;

            btnDeleteExam = new MaterialButton { Text = "XÓA", AutoSize = true, Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined };
            btnDeleteExam.Click += BtnDeleteExam_Click;

            btnRefresh = new MaterialButton { Text = "TẢI LẠI", AutoSize = true, Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined };
            btnRefresh.Click += (s, e) => LoadExamsAsync();

            btnImportWord = new MaterialButton { Text = "NHẬP TỪ WORD", AutoSize = true, Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined };
            btnImportWord.Click += BtnImportWord_Click;

            toolbar.Controls.AddRange(new Control[] { btnCreateExam, btnEditExam, btnDeleteExam, btnRefresh, btnImportWord });

            var gridWrapper = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15) };

            dgvExams = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowTemplate = { Height = 45 },
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 50,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
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

            gridWrapper.Controls.Add(dgvExams);
            panel.Controls.Add(gridWrapper);
            panel.Controls.Add(toolbar);
            tabExamList.Controls.Add(panel);
        }

        private void SetupExamDetailTab()
        {
            var mainPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };

            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 180 };
            var lblTitle = new MaterialLabel { Text = "Tên Bài Thi:", Location = new Point(0, 15), AutoSize = true };
            txtExamTitle = new MaterialTextBox { Location = new Point(130, 0), Width = 800, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, Hint = "Nhập tên bài thi..." };

            var lblClass = new MaterialLabel { Text = "Lớp Học:", Location = new Point(0, 75), AutoSize = true };
            cbClasses = new ComboBox { Location = new Point(130, 75), Width = 300, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 12) };

            var lblDuration = new MaterialLabel { Text = "Thời Gian (phút):", Location = new Point(480, 75), AutoSize = true };
            nudDuration = new NumericUpDown { Location = new Point(640, 75), Width = 100, Minimum = 1, Maximum = 180, Value = 60, Font = new Font("Segoe UI", 12) };

            var lblStart = new MaterialLabel { Text = "Mở đề lúc:", Location = new Point(0, 130), AutoSize = true };
            dtpStartTime = new DateTimePicker { Location = new Point(130, 126), Width = 200, Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm", Font = new Font("Segoe UI", 11) };

            var lblEnd = new MaterialLabel { Text = "Đóng đề lúc:", Location = new Point(360, 130), AutoSize = true };
            dtpEndTime = new DateTimePicker { Location = new Point(480, 126), Width = 200, Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm", Font = new Font("Segoe UI", 11) };

            pnlTop.Controls.AddRange(new Control[] { lblTitle, txtExamTitle, lblClass, cbClasses, lblDuration, nudDuration, lblStart, dtpStartTime, lblEnd, dtpEndTime });

            var pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 70 };
            btnSaveExam = new MaterialButton { Text = "LƯU BÀI THI", Location = new Point(0, 15), AutoSize = true, Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained, UseAccentColor = true };
            btnSaveExam.Click += BtnSaveExam_Click;

            btnCancelExam = new MaterialButton { Text = "HỦY BỎ", Location = new Point(160, 15), AutoSize = true, Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined };
            btnCancelExam.Click += BtnCancelExam_Click;

            lblExamInfo = new MaterialLabel { Location = new Point(280, 25), AutoSize = true, Text = "Trạng thái: Chọn bài thi để xem chi tiết" };
            pnlBottom.Controls.AddRange(new Control[] { btnSaveExam, btnCancelExam, lblExamInfo });

            var pnlCenter = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 10, 0, 10) };
            var questionToolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(10, 8, 10, 5), BorderStyle = BorderStyle.FixedSingle };
            btnAddQuestion = new MaterialButton { Text = "THÊM CÂU", AutoSize = true }; btnAddQuestion.Click += BtnAddQuestion_Click;
            btnEditQuestion = new MaterialButton { Text = "SỬA CÂU", AutoSize = true, Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined }; btnEditQuestion.Click += BtnEditQuestion_Click;
            btnDeleteQuestion = new MaterialButton { Text = "XÓA CÂU", AutoSize = true, Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined }; btnDeleteQuestion.Click += BtnDeleteQuestion_Click;
            questionToolbar.Controls.AddRange(new Control[] { btnAddQuestion, btnEditQuestion, btnDeleteQuestion });

            dgvQuestions = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                RowTemplate = { Height = 40 },
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 45,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            };

            dgvQuestions.Columns.AddRange(
                new DataGridViewTextBoxColumn { Name = "QuestionId", DataPropertyName = "QuestionId", HeaderText = "ID", Visible = false },
                new DataGridViewTextBoxColumn { Name = "Content", DataPropertyName = "Content", HeaderText = "Nội Dung Câu Hỏi" },
                new DataGridViewTextBoxColumn { Name = "CorrectOption", DataPropertyName = "CorrectOption", HeaderText = "Đáp Án", Width = 150 }
            );

            pnlCenter.Controls.Add(dgvQuestions);
            pnlCenter.Controls.Add(questionToolbar);

            mainPanel.Controls.Add(pnlCenter);
            mainPanel.Controls.Add(pnlBottom);
            mainPanel.Controls.Add(pnlTop);
            tabExamDetail.Controls.Add(mainPanel);
        }

        private void SetupResultsTab()
        {
            var panel = new Panel { Dock = DockStyle.Fill };

            Panel pnlHeader = new Panel { Dock = DockStyle.Top, Height = 110, Padding = new Padding(15) };

            MaterialLabel lblFilter = new MaterialLabel { Text = "Chọn bài thi:", Location = new Point(15, 24), AutoSize = true };
            cbExamFilter = new ComboBox { Location = new Point(130, 20), Width = 350, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 12) };
            cbExamFilter.SelectedIndexChanged += CbExamFilter_SelectedIndexChanged;

            txtResultSearch = new TextBox { Location = new Point(130, 58), Width = 350, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle, PlaceholderText = "Tìm học sinh theo tên..." };
            txtResultSearch.TextChanged += (s, e) => ApplyResultSearchFilter();

            FlowLayoutPanel actionFlow = new FlowLayoutPanel { Location = new Point(500, 15), AutoSize = true, WrapContents = false };
            btnViewDetail = new MaterialButton { Text = "XEM CHI TIẾT BÀI LÀM", AutoSize = true, Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained, UseAccentColor = true };
            btnViewDetail.Click += BtnViewDetail_Click;
            btnExportExcel = new MaterialButton { Text = "XUẤT EXCEL", AutoSize = true, Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined };
            btnExportExcel.Click += BtnExportExcel_Click;

            actionFlow.Controls.Add(btnViewDetail);
            actionFlow.Controls.Add(btnExportExcel);
            pnlHeader.Controls.AddRange(new Control[] { lblFilter, cbExamFilter, txtResultSearch, actionFlow });

            var gridWrapper = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15) };

            dgvResults = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                RowTemplate = { Height = 45 },
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 50,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            };

            dgvResults.Columns.AddRange(
                new DataGridViewTextBoxColumn { Name = "StudentName", DataPropertyName = "StudentName", HeaderText = "Học Sinh" },
                new DataGridViewTextBoxColumn { Name = "Score", DataPropertyName = "Score", HeaderText = "Điểm", DefaultCellStyle = new DataGridViewCellStyle { Format = "F1" } },
                new DataGridViewTextBoxColumn { Name = "ViolationCount", DataPropertyName = "ViolationCount", HeaderText = "Lỗi" },
                new DataGridViewTextBoxColumn { Name = "StartedAt", DataPropertyName = "StartedAt", HeaderText = "Bắt Đầu", DefaultCellStyle = new DataGridViewCellStyle { Format = "HH:mm dd/MM" } },
                new DataGridViewTextBoxColumn { Name = "CompletedAt", DataPropertyName = "CompletedAt", HeaderText = "Kết Thúc", DefaultCellStyle = new DataGridViewCellStyle { Format = "HH:mm dd/MM" } },
                new DataGridViewTextBoxColumn { Name = "RetakeStatusDisplay", DataPropertyName = "RetakeStatusDisplay", HeaderText = "Trạng Thái" },
                new DataGridViewButtonColumn { Name = "ActionColumn", DataPropertyName = "ActionText", HeaderText = "Hành Động", UseColumnTextForButtonValue = false, FlatStyle = FlatStyle.Flat }
            );

            dgvResults.CellFormatting += DgvResults_CellFormatting;
            dgvResults.CellContentClick += DgvResults_CellContentClick;

            gridWrapper.Controls.Add(dgvResults);
            panel.Controls.Add(gridWrapper);
            panel.Controls.Add(pnlHeader);
            tabResults.Controls.Add(panel);
        }

        // ===================================================================================
        // HÀM ÉP MÀU THEO CHẾ ĐỘ SÁNG/TỐI (CHẮC CHẮN HOẠT ĐỘNG TRÊN MỌI PHIÊN BẢN)
        // ===================================================================================
        private void ApplyThemeToStandardControls()
        {
            var msm = MaterialSkinManager.Instance;
            bool isDark = msm.Theme == MaterialSkinManager.Themes.DARK;

            // Xác định màu nền và màu chữ dựa vào cờ isDark
            Color backColor = isDark ? Color.FromArgb(50, 50, 50) : Color.White;
            Color textColor = isDark ? Color.White : Color.Black;
            Color headerColor = isDark ? Color.FromArgb(40, 40, 40) : Color.FromArgb(240, 240, 240);
            Color gridLineColor = isDark ? Color.FromArgb(80, 80, 80) : Color.FromArgb(225, 225, 225);

            // 1. Quét và ép màu cho các Panel ẩn chứa DataGridView
            foreach (TabPage tab in tabControlMain.TabPages)
            {
                tab.BackColor = backColor;
                UpdateAllChildPanels(tab, backColor);
            }

            // 2. Ép màu cho các DataGridView
            var grids = new[] { dgvExams, dgvQuestions, dgvResults };
            foreach (var dgv in grids)
            {
                if (dgv == null) continue;

                dgv.BackgroundColor = backColor;
                dgv.GridColor = gridLineColor;

                dgv.DefaultCellStyle.BackColor = backColor;
                dgv.DefaultCellStyle.ForeColor = textColor;
                dgv.DefaultCellStyle.SelectionBackColor = msm.ColorScheme.AccentColor;
                dgv.DefaultCellStyle.SelectionForeColor = Color.White;

                dgv.ColumnHeadersDefaultCellStyle.BackColor = headerColor;
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = textColor;
                dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = headerColor;

                dgv.Invalidate();
            }

            // 3. Ép màu cho các Control nhập liệu thuần WinForms
            var inputs = new Control[] { cbClasses, cbExamFilter, txtResultSearch, nudDuration, dtpStartTime, dtpEndTime };
            foreach (var ctrl in inputs)
            {
                if (ctrl == null) continue;
                ctrl.BackColor = backColor;
                ctrl.ForeColor = textColor;
            }
        }

        // Đệ quy quét các panel con
        private void UpdateAllChildPanels(Control parent, Color color)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is Panel || c is FlowLayoutPanel)
                {
                    c.BackColor = color;
                    UpdateAllChildPanels(c, color);
                }
            }
        }

        #endregion

        #region LOGIC XỬ LÝ DỮ LIỆU VÀ SỰ KIỆN

        private async void BtnImportWord_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog { Filter = "Word Document|*.docx", Title = "Chọn file đề thi Word" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var (title, duration, classId) = await PromptForImportDetails();
                    if (string.IsNullOrEmpty(title)) return;

                    try
                    {
                        btnImportWord.Text = "ĐANG XỬ LÝ...";
                        btnImportWord.Enabled = false;

                        var newExam = await _examService.ImportExamFromWordAsync(classId, title, duration, ofd.FileName);

                        _selectedExam = newExam;
                        txtExamTitle.Text = _selectedExam.Title;
                        nudDuration.Value = _selectedExam.DurationMinutes;
                        await LoadClassesAsync();
                        cbClasses.SelectedValue = _selectedExam.ClassId;
                        dgvQuestions.DataSource = _selectedExam.Questions.ToList();

                        tabControlMain.SelectedTab = tabExamDetail;
                        lblExamInfo.Text = $"Trạng thái: Vừa nhập đề từ Word. Hãy kiểm tra lại câu hỏi.";

                        this.DialogResult = DialogResult.OK;
                        MessageBox.Show($"Nhập thành công {newExam.Questions.Count} câu hỏi!", "Thành công");

                        ApplyThemeToStandardControls(); // Cập nhật màu sau khi nạp data
                    }
                    catch (Exception ex) { MessageBox.Show("Lỗi bóc tách file: " + ex.Message); }
                    finally
                    {
                        btnImportWord.Text = "NHẬP TỪ WORD";
                        btnImportWord.Enabled = true;
                    }
                }
            }
        }

        private async Task<(string Title, int Duration, int ClassId)> PromptForImportDetails()
        {
            using (Form prompt = new Form { Width = 400, Height = 300, Text = "Thông tin đề nhập từ Word", StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog })
            {
                Label lbl1 = new Label { Left = 20, Top = 20, Text = "Tên đề thi:", Width = 350 };
                TextBox txtTitle = new TextBox { Left = 20, Top = 45, Width = 340 };
                Label lbl2 = new Label { Left = 20, Top = 80, Text = "Thời gian (phút):", Width = 150 };
                NumericUpDown numDur = new NumericUpDown { Left = 180, Top = 78, Width = 80, Minimum = 5, Value = 45 };
                Label lbl3 = new Label { Left = 20, Top = 120, Text = "Áp dụng cho lớp:", Width = 350 };
                ComboBox cb = new ComboBox { Left = 20, Top = 145, Width = 340, DropDownStyle = ComboBoxStyle.DropDownList };

                var classes = await _classService.GetOwnedClassesAsync(_teacherId);
                cb.DataSource = classes;
                cb.DisplayMember = "ClassName";
                cb.ValueMember = "ClassId";

                Button btnOk = new Button { Text = "Xác nhận nhập", Left = 240, Width = 120, Top = 200, DialogResult = DialogResult.OK };
                prompt.Controls.AddRange(new Control[] { lbl1, txtTitle, lbl2, numDur, lbl3, cb, btnOk });
                prompt.AcceptButton = btnOk;

                if (prompt.ShowDialog(this.ParentForm) == DialogResult.OK && cb.SelectedValue != null)
                    return (txtTitle.Text.Trim(), (int)numDur.Value, (int)cb.SelectedValue);

                return (null, 0, 0);
            }
        }

        private async Task LoadExamsAsync()
        {
            try
            {
                _exams = await _examService.GetExamsForTeacherAsync(_teacherId);

                var displayData = _exams.Select(e => new
                {
                    e.ExamId,
                    e.Title,
                    ClassName = e.Class?.ClassName ?? "N/A",
                    e.DurationMinutes,
                    Questions = e.Questions?.Count ?? 0,
                    Submissions = e.ExamResults?.Count ?? 0,
                    e.CreatedAt
                }).ToList();

                dgvExams.DataSource = null;
                dgvExams.DataSource = displayData;

                cbExamFilter.DataSource = null;
                cbExamFilter.DataSource = _exams;
                cbExamFilter.DisplayMember = "Title";
                cbExamFilter.ValueMember = "ExamId";

                ApplyThemeToStandardControls(); // Cập nhật màu lưới
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải dữ liệu bài thi: " + ex.Message); }
        }

        private async Task LoadClassesAsync()
        {
            try
            {
                var classes = await _classService.GetOwnedClassesAsync(_teacherId);
                cbClasses.DataSource = classes;
                cbClasses.DisplayMember = "ClassName";
                cbClasses.ValueMember = "ClassId";
                ApplyThemeToStandardControls();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải danh sách lớp: " + ex.Message); }
        }

        private async void BtnCreateExam_Click(object sender, EventArgs e)
        {
            _selectedExam = null;
            ClearExamDetail();
            await LoadClassesAsync();
            tabControlMain.SelectedTab = tabExamDetail;
            lblExamInfo.Text = "Trạng thái: Đang tạo bài thi mới";
        }

        private async void BtnEditExam_Click(object sender, EventArgs e)
        {
            if (dgvExams.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn bài thi để sửa"); return;
            }

            var examId = (int)dgvExams.SelectedRows[0].Cells["ExamId"].Value;
            _selectedExam = _exams.FirstOrDefault(ex => ex.ExamId == examId);

            if (_selectedExam != null)
            {
                txtExamTitle.Text = _selectedExam.Title;
                nudDuration.Value = _selectedExam.DurationMinutes;
                await LoadClassesAsync();
                cbClasses.SelectedValue = _selectedExam.ClassId;

                if (_selectedExam.StartTime != null && _selectedExam.StartTime > DateTime.MinValue)
                    dtpStartTime.Value = (DateTime)_selectedExam.StartTime;

                if (_selectedExam.EndTime != null && _selectedExam.EndTime > DateTime.MinValue)
                    dtpEndTime.Value = (DateTime)_selectedExam.EndTime;

                var questions = await _examService.GetQuestionsByExamIdAsync(examId);
                dgvQuestions.DataSource = questions;

                tabControlMain.SelectedTab = tabExamDetail;
                lblExamInfo.Text = $"Trạng thái: Đang sửa bài thi '{_selectedExam.Title}'";
                ApplyThemeToStandardControls();
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
                MessageBox.Show("Vui lòng chọn bài thi để xóa"); return;
            }

            if (MessageBox.Show("Bạn chắc chắn muốn xóa bài thi này?\n(Tất cả kết quả sẽ bị xóa)", "Xác Nhận Xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                var examId = (int)dgvExams.SelectedRows[0].Cells["ExamId"].Value;
                try
                {
                    await _examService.DeleteExamAsync(examId);
                    this.DialogResult = DialogResult.OK;
                    MessageBox.Show("Xóa bài thi thành công");
                    await LoadExamsAsync();
                }
                catch (Exception ex) { MessageBox.Show("Lỗi xóa bài thi: " + ex.Message); }
            }
        }

        private async void BtnSaveExam_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtExamTitle.Text))
            {
                MessageBox.Show("Vui lòng nhập tên bài thi!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtExamTitle.Focus(); return;
            }

            if (cbClasses.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn lớp học!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra giá trị text trực tiếp để bắt lỗi gõ tay (Bỏ qua làm tròn của control)
            if (!int.TryParse(nudDuration.Text, out int actualInput) || actualInput < 5)
            {
                MessageBox.Show("Thời gian làm bài phải tối thiểu là 5 phút!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nudDuration.Value = 5;
                nudDuration.Focus();
                return;
            }

            try
            {
                btnSaveExam.Enabled = false;

                if (_selectedExam == null)
                {
                    var classId = (int)cbClasses.SelectedValue;
                    var newExam = await _examService.CreateExamAsync(classId, txtExamTitle.Text, (int)nudDuration.Value);

                    newExam.StartTime = dtpStartTime.Value;
                    newExam.EndTime = dtpEndTime.Value;
                    await _examService.UpdateExamAsync(newExam);

                    MessageBox.Show("Tạo bài thi thành công!");
                }
                else
                {
                    _selectedExam.Title = txtExamTitle.Text;
                    _selectedExam.DurationMinutes = (int)nudDuration.Value;
                    _selectedExam.ClassId = (int)cbClasses.SelectedValue;
                    _selectedExam.StartTime = dtpStartTime.Value;
                    _selectedExam.EndTime = dtpEndTime.Value;

                    await _examService.UpdateExamAsync(_selectedExam);
                    MessageBox.Show("Cập nhật bài thi thành công!");
                }

                // KHÔNG GỌI LoadExamsAsync Ở ĐÂY NỮA ĐỂ TRÁNH LỖI A Second Operation...
                this.DialogResult = DialogResult.OK;
                this.Close(); // Đóng form luôn
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { btnSaveExam.Enabled = true; }
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
            dtpStartTime.Value = DateTime.Now;
            dtpEndTime.Value = DateTime.Now.AddDays(1);
            dgvQuestions.DataSource = null;
            _selectedExam = null;
            ApplyThemeToStandardControls();
        }

        private async void BtnAddQuestion_Click(object sender, EventArgs e)
        {
            if (_selectedExam == null)
            {
                MessageBox.Show("Vui lòng tạo hoặc chọn bài thi trước khi thêm câu hỏi!"); return;
            }

            using (var frm = new FrmQuestionEditor(_examService, _selectedExam.ExamId))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    dgvQuestions.DataSource = await _examService.GetQuestionsByExamIdAsync(_selectedExam.ExamId);
                    ApplyThemeToStandardControls();
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
                    dgvQuestions.DataSource = await _examService.GetQuestionsByExamIdAsync(_selectedExam.ExamId);
                    ApplyThemeToStandardControls();
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
                    dgvQuestions.DataSource = await _examService.GetQuestionsByExamIdAsync(_selectedExam.ExamId);
                    ApplyThemeToStandardControls();
                }
                catch (Exception ex) { MessageBox.Show("Lỗi xóa câu hỏi: " + ex.Message); }
            }
        }

        private async void CbExamFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbExamFilter.SelectedItem == null) return;
            var selectedExam = cbExamFilter.SelectedItem as Exam;
            if (selectedExam == null) return;

            try
            {
                _currentExamResults = await _examService.GetExamResultsAsync(selectedExam.ExamId);

                _resultRows = _currentExamResults.Select(r => new ResultRowViewModel
                {
                    StudentName = r.Student?.FullName ?? "N/A",
                    Score = r.Score,
                    ViolationCount = r.ViolationLogs != null ? r.ViolationLogs.Count : 0,
                    StartedAt = r.StartedAt,
                    CompletedAt = r.CompletedAt,
                    RetakeStatusDisplay = r.RetakeStatus == "Pending" ? "Đang xin làm lại" :
                                          r.RetakeStatus == "Approved" ? "Đã cho phép" :
                                          r.RetakeStatus == "Retaken" ? "Đang thi lại" : "",
                    ActionText = r.RetakeStatus == "Pending" ? "✅ Duyệt" : ""
                }).ToList();

                ApplyResultSearchFilter();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải kết quả: " + ex.Message); }
        }

        private void ApplyResultSearchFilter()
        {
            if (dgvResults == null) return;

            var keyword = txtResultSearch?.Text?.Trim();
            IEnumerable<ResultRowViewModel> query = _resultRows;

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(r => (r.StudentName ?? string.Empty)
                    .Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }

            dgvResults.DataSource = query.ToList();
            ApplyThemeToStandardControls(); // Ép màu lại sau khi filter
        }

        private void DgvResults_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == dgvResults.Columns["ViolationCount"].Index && e.Value != null)
            {
                if (int.TryParse(e.Value.ToString(), out int violationCount) && violationCount > 0)
                {
                    e.CellStyle.ForeColor = Color.Tomato;
                    e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Bold);
                }
            }
        }

        private async void DgvResults_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvResults.Columns[e.ColumnIndex].Name == "ActionColumn")
            {
                var result = _currentExamResults[e.RowIndex];

                if (result.RetakeStatus == "Pending")
                {
                    var confirm = MessageBox.Show($"Cho phép học sinh {result.Student?.FullName} làm lại?\n(Điểm cũ sẽ bị xóa khi học sinh bắt đầu thi lại)",
                                                  "Xác nhận duyệt", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (confirm == DialogResult.Yes)
                    {
                        try
                        {
                            await _examService.ApproveRetakeAsync(result.ResultId, true);
                            MessageBox.Show("Đã duyệt thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            CbExamFilter_SelectedIndexChanged(null, null);
                        }
                        catch (Exception ex) { MessageBox.Show("Lỗi khi duyệt: " + ex.Message); }
                    }
                }
            }
        }

        private async void BtnViewDetail_Click(object sender, EventArgs e)
        {
            if (dgvResults.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một học sinh trong bảng kết quả!"); return;
            }

            try
            {
                int rowIndex = dgvResults.SelectedRows[0].Index;
                if (_currentExamResults == null || rowIndex >= _currentExamResults.Count) return;

                var selectedResultSummary = _currentExamResults[rowIndex];
                var selectedExam = cbExamFilter.SelectedItem as Exam;

                if (selectedResultSummary == null || selectedExam == null) return;

                var fullExamResult = await _examService.GetExamResultByIdAsync(selectedResultSummary.ResultId);
                if (fullExamResult == null)
                {
                    MessageBox.Show("Không tìm thấy kết quả chi tiết trong cơ sở dữ liệu."); return;
                }

                using (var frm = new FrmExamResultDetail(_examService, selectedExam, fullExamResult, fullExamResult.Student))
                {
                    frm.ShowDialog(this);
                }
            }
            catch (Exception ex) { MessageBox.Show("Không thể xem chi tiết: " + ex.Message); }
        }

        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            if (dgvResults.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedExam = cbExamFilter.SelectedItem as Exam;
            string defaultFileName = selectedExam != null ? $"KetQua_{selectedExam.Title}.xlsx" : "KetQuaBaiThi.xlsx";

            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel Workbook|*.xlsx", FileName = defaultFileName })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var workbook = new ClosedXML.Excel.XLWorkbook())
                        {
                            var worksheet = workbook.Worksheets.Add("Kết Quả");

                            for (int i = 0; i < dgvResults.Columns.Count; i++)
                            {
                                worksheet.Cell(1, i + 1).Value = dgvResults.Columns[i].HeaderText;
                                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                                worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGray;
                            }

                            for (int i = 0; i < dgvResults.Rows.Count; i++)
                            {
                                for (int j = 0; j < dgvResults.Columns.Count; j++)
                                {
                                    var cellValue = dgvResults.Rows[i].Cells[j].Value;
                                    worksheet.Cell(i + 2, j + 1).Value = cellValue != null ? cellValue.ToString() : "";
                                }
                            }
                            worksheet.Columns().AdjustToContents();
                            workbook.SaveAs(sfd.FileName);
                        }

                        MessageBox.Show("Xuất file Excel thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex) { MessageBox.Show("Lỗi xuất file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            }
        }
        #endregion
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }
        private System.ComponentModel.IContainer components = null;
    }

}