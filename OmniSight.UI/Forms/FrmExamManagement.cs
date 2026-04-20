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
        private MaterialButton btnImportWord;
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
        private DateTimePicker dtpStartTime, dtpEndTime;

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
            ApplyThemeToStandardControls();
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

            tabExamList = new TabPage { Text = "Danh Sách Bài Thi" };
            tabExamDetail = new TabPage { Text = "Chi Tiết & Câu Hỏi" };
            tabResults = new TabPage { Text = "Kết Quả Học Sinh" };

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
            var panel = new Panel { Dock = DockStyle.Fill };

            // 1. THANH CÔNG CỤ: Dùng FlowLayoutPanel để các nút tự động xếp hàng và cách đều nhau
            var toolbar = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 60, // Tăng chiều cao thanh công cụ lên một chút
               
                Padding = new Padding(15, 12, 15, 10), // Tạo khoảng trống lề cho đẹp
                WrapContents = false
            };

            // Đổi sang dùng AutoSize = true để nút tự co giãn theo chữ, không cần set cứng Location nữa
            btnCreateExam = new MaterialButton { Text = "➕ TẠO BÀI THI", AutoSize = true };
            btnCreateExam.Click += BtnCreateExam_Click;

            btnEditExam = new MaterialButton { Text = "✏️ SỬA", AutoSize = true, Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined };
            btnEditExam.Click += BtnEditExam_Click;

            btnDeleteExam = new MaterialButton { Text = "🗑️ XÓA", AutoSize = true, Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined };
            btnDeleteExam.Click += BtnDeleteExam_Click;

            btnRefresh = new MaterialButton { Text = "🔄 TẢI LẠI", AutoSize = true, Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined };
            btnRefresh.Click += (s, e) => LoadExamsAsync();

            // Xóa dòng bị trùng lặp cũ của bạn, chỉ thêm 1 lần sạch sẽ:
            toolbar.Controls.AddRange(new Control[] { btnCreateExam, btnEditExam, btnDeleteExam, btnRefresh });

            // 2. KHUNG CHỨA BẢNG: Tạo một Panel bọc DataGridView để tạo lề (Padding)
            var gridWrapper = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15), // Cách lề Form 15px ở mọi phía
                
            };

            // 3. NÂNG CẤP BẢNG DATA: Tăng chiều cao hàng, đổi màu font
            dgvExams = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
               
                BorderStyle = BorderStyle.FixedSingle, // Viền nhẹ
                RowHeadersVisible = false,
                GridColor = Color.FromArgb(220, 220, 220),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,

                // Làm cho các hàng cao và thoáng hơn (mặc định là 22 rất bé)
                RowTemplate = { Height = 45 },

                // Tùy chỉnh Tiêu đề cột
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 50,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(240, 240, 240),
                    ForeColor = Color.Black,
                    Font = new Font("Roboto", 11, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft
                },

                // Tùy chỉnh Các hàng dữ liệu
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Roboto", 10),
                    Padding = new Padding(5, 0, 0, 0), // Lùi chữ vào 5px cho đỡ sát viền
                    SelectionBackColor = Color.FromArgb(227, 242, 253), // Chuyển màu chọn sang xanh nhạt (giống Google)
                    SelectionForeColor = Color.Black
                }
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

            // Đưa lưới vào Wrapper, rồi đưa Wrapper vào Panel chính
            gridWrapper.Controls.Add(dgvExams);

            panel.Controls.Add(gridWrapper);
            panel.Controls.Add(toolbar);
            tabExamList.Controls.Add(panel);
        }

        private void SetupExamDetailTab()
        {
            // Panel chính bọc toàn bộ nội dung, cách lề 20px cho thoáng
            var mainPanel = new Panel { Dock = DockStyle.Fill,  Padding = new Padding(20) };

            // ==========================================
            // 1. KHU VỰC NHẬP LIỆU (Nằm trên cùng)
            // ==========================================
            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 180 };

            var lblTitle = new MaterialLabel { Text = "Tên Bài Thi:", Location = new Point(0, 15), AutoSize = true };
            // Dùng Anchor Right để ô Tên bài thi tự động kéo dài ra kịch mép phải
            txtExamTitle = new MaterialTextBox { Location = new Point(130, 0), Width = 800, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, Hint = "Nhập tên bài thi..." };

            var lblClass = new MaterialLabel { Text = "Lớp Học:", Location = new Point(0, 75), AutoSize = true };
            cbClasses = new ComboBox { Location = new Point(130, 75), Width = 300, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Roboto", 12) };

            var lblDuration = new MaterialLabel { Text = "Thời Gian (phút):", Location = new Point(480, 75), AutoSize = true };
            nudDuration = new NumericUpDown { Location = new Point(640, 75), Width = 100, Minimum = 1, Maximum = 180, Value = 60, Font = new Font("Roboto", 12) };

            var lblStart = new MaterialLabel { Text = "Mở đề lúc:", Location = new Point(0, 130), AutoSize = true };
            dtpStartTime = new DateTimePicker { Location = new Point(130, 126), Width = 200, Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm", Font = new Font("Roboto", 11) };

            var lblEnd = new MaterialLabel { Text = "Đóng đề lúc:", Location = new Point(360, 130), AutoSize = true };
            dtpEndTime = new DateTimePicker { Location = new Point(480, 126), Width = 200, Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm", Font = new Font("Roboto", 11) };

            pnlTop.Controls.AddRange(new Control[] { lblTitle, txtExamTitle, lblClass, cbClasses, lblDuration, nudDuration, lblStart, dtpStartTime, lblEnd, dtpEndTime });

            // ==========================================
            // 2. KHU VỰC NÚT LƯU / HỦY (Nằm dưới cùng)
            // ==========================================
            var pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 70 };

            btnSaveExam = new MaterialButton { Text = "💾 LƯU BÀI THI", Location = new Point(0, 15), AutoSize = true, Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained, UseAccentColor = true };
            btnSaveExam.Click += BtnSaveExam_Click;

            btnCancelExam = new MaterialButton { Text = "❌ HỦY BỎ", Location = new Point(160, 15), AutoSize = true, Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined };
            btnCancelExam.Click += BtnCancelExam_Click;

            lblExamInfo = new MaterialLabel { Location = new Point(280, 25), AutoSize = true, Text = "Trạng thái: Chọn bài thi để xem chi tiết", ForeColor = Color.Gray };

            pnlBottom.Controls.AddRange(new Control[] { btnSaveExam, btnCancelExam, lblExamInfo });

            // ==========================================
            // 3. KHU VỰC BẢNG CÂU HỎI (Lấp đầy khoảng giữa)
            // ==========================================
            var pnlCenter = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 10, 0, 10) }; // Tạo margin trên dưới

            // Thanh công cụ nằm ngay trên bảng
            var questionToolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 50, BackColor = Color.FromArgb(245, 245, 245), Padding = new Padding(10, 8, 10, 5), BorderStyle = BorderStyle.FixedSingle };
            btnAddQuestion = new MaterialButton { Text = "➕ THÊM CÂU", AutoSize = true }; btnAddQuestion.Click += BtnAddQuestion_Click;
            btnEditQuestion = new MaterialButton { Text = "✏️ SỬA CÂU", AutoSize = true, Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined }; btnEditQuestion.Click += BtnEditQuestion_Click;
            btnDeleteQuestion = new MaterialButton { Text = "🗑️ XÓA CÂU", AutoSize = true, Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined }; btnDeleteQuestion.Click += BtnDeleteQuestion_Click;

            questionToolbar.Controls.AddRange(new Control[] { btnAddQuestion, btnEditQuestion, btnDeleteQuestion });

            // Nâng cấp giao diện DataGridView câu hỏi
            dgvQuestions = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
               
                BorderStyle = BorderStyle.FixedSingle,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                RowTemplate = { Height = 40 }, // Giãn chiều cao hàng
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 45,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(240, 240, 240), Font = new Font("Roboto", 11, FontStyle.Bold) },
                DefaultCellStyle = new DataGridViewCellStyle { Font = new Font("Roboto", 10), Padding = new Padding(5, 0, 0, 0), SelectionBackColor = Color.FromArgb(227, 242, 253), SelectionForeColor = Color.Black }
            };

            dgvQuestions.Columns.AddRange(
                new DataGridViewTextBoxColumn { Name = "QuestionId", DataPropertyName = "QuestionId", HeaderText = "ID", Visible = false },
                new DataGridViewTextBoxColumn { Name = "Content", DataPropertyName = "Content", HeaderText = "Nội Dung Câu Hỏi" },
                new DataGridViewTextBoxColumn { Name = "CorrectOption", DataPropertyName = "CorrectOption", HeaderText = "Đáp Án", Width = 150 }
            );

            // Gắn Toolbar và Grid vào Center Panel
            pnlCenter.Controls.Add(dgvQuestions);
            pnlCenter.Controls.Add(questionToolbar);

            // ==========================================
            // LẮP RÁP TẤT CẢ VÀO MAIN PANEL
            // LƯU Ý THỨ TỰ ADD: Center thêm đầu tiên để Fill phần còn lại
            // ==========================================
            mainPanel.Controls.Add(pnlCenter);
            mainPanel.Controls.Add(pnlBottom);
            mainPanel.Controls.Add(pnlTop);

            tabExamDetail.Controls.Add(mainPanel);
        }

        private void SetupResultsTab()
        {
            var panel = new Panel { Dock = DockStyle.Fill };

            // ==========================================
            // 1. THANH BỘ LỌC VÀ NÚT BẤM (Phía trên)
            // ==========================================
            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(245, 245, 245),
                Padding = new Padding(15) // Cách lề 2 bên
            };

            MaterialLabel lblFilter = new MaterialLabel
            {
                Text = "Chọn bài thi:",
                Location = new Point(15, 24),
                AutoSize = true
            };

            cbExamFilter = new ComboBox
            {
                Location = new Point(130, 20),
                Width = 350,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Roboto", 12)
            };
            cbExamFilter.SelectedIndexChanged += CbExamFilter_SelectedIndexChanged;

            // Gom 2 nút bấm vào FlowLayoutPanel để chúng tự động dãn cách và canh hàng
            FlowLayoutPanel actionFlow = new FlowLayoutPanel
            {
                Location = new Point(500, 15),
                AutoSize = true,
                WrapContents = false,
                BackColor = Color.Transparent
            };

            btnViewDetail = new MaterialButton
            {
                Text = "👁️ XEM CHI TIẾT BÀI LÀM",
                AutoSize = true, // Tự co giãn theo chữ
                Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained,
                UseAccentColor = true
            };
            btnViewDetail.Click += BtnViewDetail_Click;

            btnExportExcel = new MaterialButton
            {
                Text = "📥 XUẤT EXCEL",
                AutoSize = true,
                Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined
            };
            btnExportExcel.Click += BtnExportExcel_Click;

            actionFlow.Controls.Add(btnViewDetail);
            actionFlow.Controls.Add(btnExportExcel);

            pnlHeader.Controls.AddRange(new Control[] { lblFilter, cbExamFilter, actionFlow });

            // ==========================================
            // 2. KHUNG CHỨA BẢNG KẾT QUẢ (Tạo lề đệm)
            // ==========================================
            var gridWrapper = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15), // Tạo lề 15px xung quanh bảng cho thoáng
                

            };

            // ==========================================
            // 3. BẢNG DỮ LIỆU KẾT QUẢ (Nâng cấp giao diện)
            // ==========================================
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
                GridColor = Color.FromArgb(220, 220, 220),

                // Nâng chiều cao mỗi hàng lên 45px (thoáng giống tab Danh sách)
                RowTemplate = { Height = 45 },

                // Làm đẹp tiêu đề cột
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 50,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(240, 240, 240),
                    ForeColor = Color.Black,
                    Font = new Font("Roboto", 11, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft
                },

                // Làm đẹp dữ liệu và màu khi được chọn
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Roboto", 10),
                    Padding = new Padding(5, 0, 0, 0), // Lùi chữ vào một chút
                    SelectionBackColor = Color.FromArgb(227, 242, 253), // Highlight màu xanh nhạt
                    SelectionForeColor = Color.Black
                }
            };

            dgvResults.Columns.AddRange(
                new DataGridViewTextBoxColumn { Name = "StudentName", DataPropertyName = "StudentName", HeaderText = "Học Sinh" },
                new DataGridViewTextBoxColumn { Name = "Score", DataPropertyName = "Score", HeaderText = "Điểm", DefaultCellStyle = new DataGridViewCellStyle { Format = "F1" } },
                new DataGridViewTextBoxColumn { Name = "ViolationCount", DataPropertyName = "ViolationCount", HeaderText = "Số Lần Vi Phạm" },
                new DataGridViewTextBoxColumn { Name = "StartedAt", DataPropertyName = "StartedAt", HeaderText = "Bắt Đầu", DefaultCellStyle = new DataGridViewCellStyle { Format = "HH:mm dd/MM" } },
                new DataGridViewTextBoxColumn { Name = "CompletedAt", DataPropertyName = "CompletedAt", HeaderText = "Kết Thúc", DefaultCellStyle = new DataGridViewCellStyle { Format = "HH:mm dd/MM" } },
                new DataGridViewTextBoxColumn { Name = "RetakeStatusDisplay", DataPropertyName = "RetakeStatusDisplay", HeaderText = "Trạng Thái" },

                new DataGridViewButtonColumn
                {
                    Name = "ActionColumn",
                    DataPropertyName = "ActionText",
                    HeaderText = "Hành Động",
                    UseColumnTextForButtonValue = false,
                    FlatStyle = FlatStyle.Flat
                }
            );
            dgvResults.CellFormatting += DgvResults_CellFormatting;
            dgvResults.CellContentClick += DgvResults_CellContentClick;

            // Lắp ráp vào Tab
            gridWrapper.Controls.Add(dgvResults);
            panel.Controls.Add(gridWrapper);
            panel.Controls.Add(pnlHeader);

            tabResults.Controls.Add(panel);
        }

        #endregion

        #region LOGIC XỬ LÝ DỮ LIỆU VÀ SỰ KIỆN
        private async void BtnImportWord_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog { Filter = "Word Document|*.docx", Title = "Chọn file đề thi Word" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    // 1. Hiện popup hỏi Tên đề, Thời gian và Lớp học (vì Management cần ClassId)
                    var (title, duration, classId) = await PromptForImportDetails();
                    if (string.IsNullOrEmpty(title)) return;

                    try
                    {
                        btnImportWord.Text = "ĐANG XỬ LÝ...";
                        btnImportWord.Enabled = false;

                        // 2. Gọi service bóc tách Word và lưu vào DB
                        var newExam = await _examService.ImportExamFromWordAsync(classId, title, duration, ofd.FileName);

                        // 3. THIẾT LẬP GIAO DIỆN SỬA NGAY LẬP TỨC
                        _selectedExam = newExam; // Gán đề vừa tạo vào biến đang chọn

                        // Đổ dữ liệu lên Tab 2
                        txtExamTitle.Text = _selectedExam.Title;
                        nudDuration.Value = _selectedExam.DurationMinutes;
                        await LoadClassesAsync();
                        cbClasses.SelectedValue = _selectedExam.ClassId;

                        // Load danh sách câu hỏi vừa bóc tách được
                        dgvQuestions.DataSource = _selectedExam.Questions.ToList();

                        // Chuyển sang Tab Chi tiết để giáo viên sửa
                        tabControlMain.SelectedTab = tabExamDetail;
                        lblExamInfo.Text = $"Trạng thái: Vừa nhập đề từ Word. Hãy kiểm tra lại câu hỏi.";

                        this.DialogResult = DialogResult.OK; // Báo cho Dashboard biết để load lại
                        MessageBox.Show($"Nhập thành công {newExam.Questions.Count} câu hỏi!", "Thành công");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi bóc tách file: " + ex.Message);
                    }
                    finally
                    {
                        btnImportWord.Text = "📝 Nhập từ Word";
                        btnImportWord.Enabled = true;
                    }
                }
            }
        }

        // Hàm hỗ trợ popup lấy thông tin nhanh
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

                // Load danh sách lớp vào combo
                var classes = await _classService.GetOwnedClassesAsync(_teacherId);
                cb.DataSource = classes;
                cb.DisplayMember = "ClassName";
                cb.ValueMember = "ClassId";

                Button btnOk = new Button { Text = "Xác nhận nhập", Left = 240, Width = 120, Top = 200, DialogResult = DialogResult.OK };
                prompt.Controls.AddRange(new Control[] { lbl1, txtTitle, lbl2, numDur, lbl3, cb, btnOk });
                prompt.AcceptButton = btnOk;

                if (prompt.ShowDialog() == DialogResult.OK && cb.SelectedValue != null)
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

                // === THÊM 2 DÒNG NÀY ĐỂ LOAD THỜI GIAN LÊN GIAO DIỆN ===
                // Đề phòng trường hợp thời gian trong DB bị null
                if (_selectedExam.StartTime != null && _selectedExam.StartTime > DateTime.MinValue)
                    dtpStartTime.Value = (DateTime)_selectedExam.StartTime;

                if (_selectedExam.EndTime != null && _selectedExam.EndTime > DateTime.MinValue)
                    dtpEndTime.Value = (DateTime)_selectedExam.EndTime;
                // =======================================================

                // Load questions
                var questions = await _examService.GetQuestionsByExamIdAsync(examId);
                dgvQuestions.DataSource = questions;

                tabControlMain.SelectedTab = tabExamDetail;
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
        private async void DgvResults_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra xem người dùng có click đúng vào cột "Hành Động" không
            if (e.RowIndex >= 0 && dgvResults.Columns[e.ColumnIndex].Name == "ActionColumn")
            {
                var result = _currentExamResults[e.RowIndex];

                // Chỉ xử lý nếu trạng thái đang là Pending
                if (result.RetakeStatus == "Pending")
                {
                    var confirm = MessageBox.Show($"Bạn có chắc chắn muốn cho phép học sinh {result.Student?.FullName} làm lại bài thi này không?\n\n(Điểm và bài làm cũ sẽ bị xóa khi học sinh bắt đầu làm lại)",
                                                  "Xác nhận duyệt", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (confirm == DialogResult.Yes)
                    {
                        try
                        {
                            await _examService.ApproveRetakeAsync(result.ResultId, true);
                            MessageBox.Show("Đã duyệt thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Tải lại lưới dữ liệu để cập nhật giao diện
                            CbExamFilter_SelectedIndexChanged(null, null);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi khi duyệt: " + ex.Message);
                        }
                    }
                }
            }
        }
        private async void BtnSaveExam_Click(object sender, EventArgs e)
        {
            // 1. Validate dữ liệu
            if (string.IsNullOrWhiteSpace(txtExamTitle.Text))
            {
                MessageBox.Show("Vui lòng nhập tên bài thi!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtExamTitle.Focus();
                return;
            }
            if (cbClasses.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn lớp học!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            nudDuration.Validate();
            if (nudDuration.Value < 5)
            {
                MessageBox.Show("Thời gian làm bài phải tối thiểu là 5 phút!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nudDuration.Focus();
                return;
            }

            try
            {
                btnSaveExam.Enabled = false;

                if (_selectedExam == null)
                {
                    // TẠO MỚI (Trường hợp Service không hỗ trợ truyền StartTime/EndTime)
                    var classId = (int)cbClasses.SelectedValue;
                    var newExam = await _examService.CreateExamAsync(classId, txtExamTitle.Text, (int)nudDuration.Value);

                    // Cập nhật ngay thời gian sau khi tạo
                    newExam.StartTime = dtpStartTime.Value;
                    newExam.EndTime = dtpEndTime.Value;
                    await _examService.UpdateExamAsync(newExam);

                    MessageBox.Show("Tạo bài thi thành công!");
                }
                else
                {
                    // CẬP NHẬT
                    _selectedExam.Title = txtExamTitle.Text;
                    _selectedExam.DurationMinutes = (int)nudDuration.Value;
                    _selectedExam.ClassId = (int)cbClasses.SelectedValue;

                    // QUAN TRỌNG: Gán giá trị thời gian TRƯỚC khi gọi hàm Update
                    // Mặc định dtpStartTime.Value sẽ tự lấy giờ trên control, không cần click
                    _selectedExam.StartTime = dtpStartTime.Value;
                    _selectedExam.EndTime = dtpEndTime.Value;

                    await _examService.UpdateExamAsync(_selectedExam);

                    MessageBox.Show("Cập nhật bài thi thành công!");
                }

                // Báo cho Dashboard biết để tự nó load lại dữ liệu (tuyệt đối không gọi LoadExamsAsync ở đây nữa)
                this.DialogResult = DialogResult.OK;
                this.Close(); // Đóng form an toàn
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSaveExam.Enabled = true;
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

            // === THÊM 2 DÒNG NÀY ===
            dtpStartTime.Value = DateTime.Now; // Mặc định mở ngay bây giờ
            dtpEndTime.Value = DateTime.Now.AddDays(1); // Mặc định đóng sau 1 ngày
                                                        // =======================

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
                    r.CompletedAt,

                    // Chuyển đổi trạng thái tiếng Anh sang tiếng Việt cho đẹp
                    RetakeStatusDisplay = r.RetakeStatus == "Pending" ? "Đang xin làm lại" :
                                          r.RetakeStatus == "Approved" ? "Đã cho phép" :
                                          r.RetakeStatus == "Retaken" ? "Đang thi lại" : "",

                    // Nếu đang Pending thì hiện chữ "Duyệt", không thì để trống
                    ActionText = r.RetakeStatus == "Pending" ? "✅ Duyệt" : ""
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
        private void ApplyThemeToStandardControls()
        {
            var msm = MaterialSkinManager.Instance;
            bool isDark = msm.Theme == MaterialSkinManager.Themes.DARK;

            // Lấy màu chuẩn từ Theme
            Color bgColor = isDark ? Color.FromArgb(50, 50, 50) : Color.White;
            Color textColor = isDark ? Color.White : Color.Black;
            Color gridColor = isDark ? Color.FromArgb(80, 80, 80) : Color.FromArgb(230, 230, 230);
            Color headerColor = isDark ? Color.FromArgb(40, 40, 40) : Color.FromArgb(245, 245, 245);

            var grids = new[] { dgvExams, dgvQuestions, dgvResults };
            foreach (var dgv in grids)
            {
                if (dgv == null) continue;
                dgv.BackgroundColor = bgColor;
                dgv.GridColor = gridColor;

                // QUAN TRỌNG: Ép màu chữ và nền cho các ô dữ liệu
                dgv.DefaultCellStyle.BackColor = bgColor;
                dgv.DefaultCellStyle.ForeColor = textColor;
                dgv.DefaultCellStyle.SelectionBackColor = msm.ColorScheme.AccentColor;
                dgv.DefaultCellStyle.SelectionForeColor = Color.White;

                // QUAN TRỌNG: Ép màu cho Tiêu đề cột (Header)
                dgv.ColumnHeadersDefaultCellStyle.BackColor = headerColor;
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = textColor;
                dgv.EnableHeadersVisualStyles = false;

                dgv.Invalidate(); // Ép vẽ lại ngay
            }

            // Cập nhật các control khác
            cbClasses.BackColor = bgColor; cbClasses.ForeColor = textColor;
            cbExamFilter.BackColor = bgColor; cbExamFilter.ForeColor = textColor;
            nudDuration.BackColor = bgColor; nudDuration.ForeColor = textColor;
        }
    }
}