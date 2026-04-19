using Emgu.CV;
using Emgu.CV.Structure;
using MaterialSkin;
using MaterialSkin.Controls;
using Microsoft.Extensions.DependencyInjection;
using OmniSight.Services;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace OmniSight.UI.Forms
{
    public partial class MainForm : MaterialForm
    {
        private readonly FaceAiService _faceAiService;
        private readonly AuthService _authService;
        private readonly IUserService _userService;
        private readonly IServiceProvider _serviceProvider;
        private bool _isLoggingOut = false;
        private float _currentZoom = 1.0f;
        private const float MIN_ZOOM = 0.7f;
        private const float MAX_ZOOM = 2.0f;
        private const float ZOOM_STEP = 0.1f;
        public bool IsFirstTimeLogin { get; set; }
        public MainForm(AuthService authService, IUserService userService, IServiceProvider serviceProvider, FaceAiService faceAiService)
        {
            InitializeComponent();
            _authService = authService;
            _userService = userService;
            _serviceProvider = serviceProvider;
            _faceAiService = faceAiService;
            LoadClassList();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Blue600, Primary.Blue700,
                Primary.Blue200, Accent.Blue400,
                TextShade.WHITE
            );
        }

        private void btnOpenExamManager_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra quyền giáo viên trước
            var user = _authService.CurrentUser;
            if (user == null) return;

            if (!user.IsTeacher)
            {
                MessageBox.Show("Chỉ giáo viên mới có quyền tạo đề thi.", "Truy cập bị từ chối",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Lấy services cần thiết
            var examService = _serviceProvider.GetRequiredService<ExamService>();
            var classService = _serviceProvider.GetRequiredService<ClassService>();

            // 3. Hiển thị Form quản lý bài thi
            using (var frm = new FrmExamManagement(examService, classService, user.UserId))
            {
                frm.ShowDialog(this);
            }
        }
        public bool StartAtProfile { get; set; } = false;
        private void MainForm_Load(object sender, EventArgs e)
        {
            this.DrawerTabControl = this.materialTabControl1;
            this.KeyPreview = true;
            this.KeyDown += MainForm_KeyDown;

            var user = _authService.CurrentUser;
            if (StartAtProfile)
            {
                // 1. Chuyển ngay sang tab Profile (thường là Index 2 tùy thiết kế của bạn)
                materialTabControl1.SelectedTab = tabProfile;

                // 2. Hiện thông báo nhắc nhở "Đẹp mắt"
                BeginInvoke(new Action(() => {
                    MessageBox.Show(this,
                        "Chào mừng thành viên mới!\n\nBạn hãy dành ít giây để:\n" +
                        "1. Chọn Vai trò (Học sinh hoặc Giáo viên).\n" +
                        "2. Cài đặt Face ID để bảo mật tài khoản.\n\nChúc bạn có trải nghiệm tốt!",
                        "Hoàn tất hồ sơ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }));
            }
            if (user != null)
            {
                lblHomeWelcome.Text = $"Chào mừng {user.FullName} đến với OmniSight!";
                btnUserAccount.Text = user.FullName.ToUpper();
                // --- BỔ SUNG CÁC DÒNG NÀY ---
                txtFullName.Text = user.FullName ?? "";
                txtPhone.Text = user.Phone ?? "";         // Load số điện thoại
                SetupHomeTab();
                SetupSettingsTab();
                _ = LoadDashboardDataAsync();
                switchStudent.Checked = user.IsStudent;   // Load trạng thái học sinh
                switchTeacher.Checked = user.IsTeacher;   // Load trạng thái giáo viên
            }

            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string detector = Path.Combine(baseDir, "Models", "face_detection_yunet_2023mar.onnx");
                string recognizer = Path.Combine(baseDir, "Models", "face_recognition_sface_2021dec.onnx");
                _faceAiService.InitializeModels(detector, recognizer);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khởi tạo AI: " + ex.Message);
            }
        }

        private void btnUserAccount_Click(object sender, EventArgs e)
        {
            cmsUserMenu.Show(btnUserAccount, new Point(0, btnUserAccount.Height));
        }

        private void tsmLogout_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                _isLoggingOut = true;
                _authService.Logout();
                Application.Restart();
                Environment.Exit(0);
            }
        }

        private async void btnSaveProfile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text)) return;
            var user = _authService.CurrentUser;
            if (user == null) return;

            bool success = await _userService.UpdateProfileAsync(user.UserId, txtFullName.Text, txtPhone.Text, switchStudent.Checked, switchTeacher.Checked);

            if (success)
            {
                // Reload user from DB to ensure switches and other fields reflect persisted state
                var updatedUser = await _userService.GetUserByIdAsync(user.UserId);
                if (updatedUser != null)
                {
                    _authService.SetCurrentUser(updatedUser);
                    btnUserAccount.Text = (updatedUser.FullName ?? string.Empty).ToUpper();

                    // Update UI fields to reflect saved state
                    txtFullName.Text = updatedUser.FullName ?? string.Empty;
                    txtPhone.Text = updatedUser.Phone ?? string.Empty;
                    switchStudent.Checked = updatedUser.IsStudent;
                    switchTeacher.Checked = updatedUser.IsTeacher;

                    MessageBox.Show("Cập nhật thành công!");
                }
                else
                {
                    // Fallback: update minimal UI
                    user.FullName = txtFullName.Text;
                    btnUserAccount.Text = txtFullName.Text.ToUpper();
                    MessageBox.Show("Cập nhật thành công! (Không thể tải lại dữ liệu người dùng)");
                }
            }
        }

        private void btnStartCamera_Click(object sender, EventArgs e)
        {
            if (_faceAiService.StartCamera(0))
            {
                timerCamera.Start();
                btnStartCamera.Enabled = false;
            }
            else MessageBox.Show("Không thể mở Camera!");
        }

        private void timerCamera_Tick(object sender, EventArgs e)
        {
            using (var frame = _faceAiService.GetFrame())
            {
                if (frame != null && !frame.IsEmpty)
                {
                    var oldImage = picCamera.Image;
                    picCamera.Image = frame.ToBitmap();
                    oldImage?.Dispose();
                }
            }
        }

        private async void btnCaptureFace_Click(object sender, EventArgs e)
        {
            using (var frame = _faceAiService.GetFrame())
            {
                if (frame == null || frame.IsEmpty) return;

                var embedding = _faceAiService.ExtractEmbedding(frame);
                if (embedding != null)
                {
                    string embStr = string.Join(";", embedding.Select(f => f.ToString(CultureInfo.InvariantCulture)));
                    var user = _authService.CurrentUser;
                    if (user != null)
                    {
                        await _userService.UpdateFaceEmbeddingAsync(user.UserId, embStr);
                        user.FaceEmbedding = embStr;
                        MessageBox.Show("Thiết lập Face ID thành công!");
                    }

                    timerCamera.Stop();
                    _faceAiService.StopCamera();
                    picCamera.Image?.Dispose();
                    picCamera.Image = null;
                    btnStartCamera.Enabled = true;
                }
                else MessageBox.Show("Không nhận diện được khuôn mặt!");
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _faceAiService.StopCamera();
            if (!_isLoggingOut) Application.Exit();
        }

        private void btnOpenCreateClass_Click(object sender, EventArgs e)
        {
            var classService = _serviceProvider.GetRequiredService<ClassService>();
            int currentTeacherId = _authService.CurrentUser?.UserId ?? 0;

            using (var frmCreate = new FrmCreateClass(classService, currentTeacherId))
            {
                var result = frmCreate.ShowDialog();
                if (result == DialogResult.OK)
                {
                    LoadClassList();
                }
            }
        }

        private async void LoadClassList()
        {
            lvwClasses.Items.Clear();
            var _currentUser = _authService.CurrentUser;
            if (_currentUser == null) return;

            var _classService = _serviceProvider.GetRequiredService<ClassService>();

            // 1. Dành cho Giáo viên
            if (_currentUser.IsTeacher)
            {
                var ownedClasses = await _classService.GetOwnedClassesAsync(_currentUser.UserId);
                foreach (var c in ownedClasses)
                {
                    // NỐI CHUỖI TẠI ĐÂY: Tên lớp - Tên môn học
                    string fullDisplayName = $"{c.ClassName} - {c.Subject?.SubjectName ?? "N/A"}";

                    var item = new ListViewItem(fullDisplayName);
                    item.SubItems.Add(c.JoinCode);
                    item.Tag = c.ClassId;
                    lvwClasses.Items.Add(item);
                }
            }

            // 2. Dành cho Học sinh
            if (_currentUser.IsStudent)
            {
                var joinedClasses = await _classService.GetJoinedClassesAsync(_currentUser.UserId);
                foreach (var c in joinedClasses)
                {
                    if (lvwClasses.Items.Cast<ListViewItem>().Any(i => (int)i.Tag == c.ClassId)) continue;

                    // NỐI CHUỖI TẠI ĐÂY: Tên lớp - Tên môn học
                    string fullDisplayName = $"{c.ClassName} - {c.Subject?.SubjectName ?? "N/A"}";

                    var item = new ListViewItem(fullDisplayName);
                    item.SubItems.Add("******");
                    item.Tag = c.ClassId;
                    lvwClasses.Items.Add(item);
                }
            }
        }

        private void btnOpenJoinClass_Click(object sender, EventArgs e)
        {
            var classService = _serviceProvider.GetRequiredService<ClassService>();
            int currentUserId = _authService.CurrentUser?.UserId ?? 0;

            using (var frmJoin = new FrmJoinClass(classService, currentUserId))
            {
                if (frmJoin.ShowDialog() == DialogResult.OK)
                {
                    LoadClassList();
                }
            }
        }

        // Double-click on class list
        private async void lvwClasses_MouseDoubleClick(object sender, EventArgs e)
        {
            var _currentUser = _authService.CurrentUser;
            if (lvwClasses.SelectedItems.Count == 0) return;

            var selectedItem = lvwClasses.SelectedItems[0];
            string className = selectedItem.Text;
            int classId = (int)(selectedItem.Tag ?? 0); 

            var streamService = _serviceProvider.GetRequiredService<StreamService>();
            var classService = _serviceProvider.GetRequiredService<ClassService>();
            var assignmentService = _serviceProvider.GetRequiredService<AssignmentService>();
            var examService = _serviceProvider.GetRequiredService<ExamService>();
            int currentUserId = _currentUser?.UserId ?? 0;
            // KIỂM TRA XEM CÓ PHẢI GIÁO VIÊN CỦA LỚP NÀY KHÔNG
            bool isTeacherOfThisClass = await classService.IsTeacherOfClassAsync(classId, currentUserId);

            // Open class detail tab and pass examService
            // Truyền thêm biến isTeacherOfThisClass vào cuối (đối số thứ 8)
            OpenClassDetailTab(classId, className, streamService, classService, assignmentService, examService, currentUserId, isTeacherOfThisClass);
        }

        private void OpenClassDetailTab(int classId, string className, StreamService streamService, ClassService classService, AssignmentService assignmentService, ExamService examService, int currentUserId, bool isTeacherOfThisClass)
        {
            string tabName = "tabClass_" + classId;
            TabPage tabDetail = materialTabControl1.TabPages[tabName];

            if (tabDetail == null)
            {
                tabDetail = new TabPage();
                tabDetail.Name = tabName;
                tabDetail.Text = "Lớp: " + className;
                tabDetail.BackColor = Color.White;

                var ucDetail = new UcClassDetail(streamService, classService, _authService, currentUserId, classId, isTeacherOfThisClass);
                ucDetail.SetAssignmentService(assignmentService);
                ucDetail.SetExamService(examService);
                ucDetail.Dock = DockStyle.Fill;

                tabDetail.Controls.Add(ucDetail);
                materialTabControl1.TabPages.Add(tabDetail);
            }

            materialTabControl1.SelectedTab = tabDetail;
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            // Ctrl + Scroll wheel simulation with + and - keys
            if (e.Control)
            {
                if (e.KeyCode == Keys.Oemplus || e.KeyCode == Keys.Add)
                {
                    ZoomIn();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.OemMinus || e.KeyCode == Keys.Subtract)
                {
                    ZoomOut();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.D0)
                {
                    ResetZoom();
                    e.Handled = true;
                }
            }
        }

        // 1. Khai báo khung chứa các thẻ thống kê
        private FlowLayoutPanel flpDashboardCards;

        // 2. Hàm Setup giao diện Trang Chủ
        private void SetupHomeTab()
        {
            // Thêm chữ "Tổng quan hoạt động" dưới dòng Chào mừng
            MaterialLabel lblSubtitle = new MaterialLabel
            {
                Text = "Tổng quan hoạt động của bạn",
                FontType = MaterialSkinManager.fontType.Subtitle1,
                Location = new Point(35, 75),
                AutoSize = true,
                ForeColor = Color.Gray
            };
            tabHome.Controls.Add(lblSubtitle);

            // Khung chứa các thẻ màu sắc
            flpDashboardCards = new FlowLayoutPanel
            {
                Location = new Point(35, 120),
                Width = tabHome.Width - 70,
                Height = 180, // TĂNG LÊN 180 ĐỂ CÁC THẺ KHÔNG BỊ CHÈN ÉP
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                WrapContents = true
            };
            tabHome.Controls.Add(flpDashboardCards);

            // Tiêu đề Hành động nhanh
            MaterialLabel lblQuickActions = new MaterialLabel
            {
                Text = "⚡ Hành động nhanh",
                FontType = MaterialSkinManager.fontType.H6,
                Location = new Point(35, 300),
                AutoSize = true
            };
            tabHome.Controls.Add(lblQuickActions);

            // Khung chứa các nút bấm
            FlowLayoutPanel flpActions = new FlowLayoutPanel
            {
                Location = new Point(35, 340),
                Width = tabHome.Width - 70,
                Height = 100,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            MaterialButton btnGoToClasses = new MaterialButton { Text = "📚 MỞ DANH SÁCH LỚP", AutoSize = true, UseAccentColor = true };
            btnGoToClasses.Click += (s, e) => materialTabControl1.SelectedTab = tabClasses;

            MaterialButton btnGoToProfile = new MaterialButton { Text = "👤 CẬP NHẬT HỒ SƠ", AutoSize = true, Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined };
            btnGoToProfile.Click += (s, e) => materialTabControl1.SelectedTab = tabProfile;

            flpActions.Controls.Add(btnGoToClasses);
            flpActions.Controls.Add(btnGoToProfile);

            // Nếu là giáo viên, thêm nút Quản lý đề thi
            var user = _authService.CurrentUser;
            if (user != null && user.IsTeacher)
            {
                MaterialButton btnGoToExams = new MaterialButton { Text = "📋 QUẢN LÝ ĐỀ THI", AutoSize = true, Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined };
                btnGoToExams.Click += btnOpenExamManager_Click;
                flpActions.Controls.Add(btnGoToExams);
            }

            tabHome.Controls.Add(flpActions);
        }

        // 3. Hàm vẽ ra từng thẻ màu sắc (Card)
        private Panel CreateDashboardCard(string title, string value, Color bgColor, string iconEmoji)
        {
            Panel card = new Panel
            {
                Width = 300,
                Height = 130,
                Margin = new Padding(0, 0, 20, 20),
                BackColor = bgColor,
                BorderStyle = BorderStyle.None
            };

            card.Paint += (s, e) => { e.Graphics.Clear(bgColor); };

            Label lblTitle = new Label { Text = title.ToUpper(), Font = new Font("Roboto", 11, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(20, 20) };
            Label lblValue = new Label { Text = value, Font = new Font("Roboto", 32, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(20, 50) };

            // Dùng Icon bằng Emoji cho nhẹ và hiện đại
            Label lblIcon = new Label { Text = iconEmoji, Font = new Font("Segoe UI Emoji", 45), ForeColor = Color.FromArgb(80, 255, 255, 255), AutoSize = true, Location = new Point(210, 30) };

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblValue);
            card.Controls.Add(lblIcon);

            return card;
        }

        // 4. Hàm nạp dữ liệu thật vào các thẻ
        private async Task LoadDashboardDataAsync()
        {
            try
            {
                var user = _authService.CurrentUser;
                if (user == null || flpDashboardCards == null) return;

                int classCount = 0;
                int pendingActionCount = 0; // Biến lưu số lượng bài tập cần xử lý

                // Mở làn đường riêng cho Database
                using (var scope = _serviceProvider.CreateScope())
                {
                    var classService = scope.ServiceProvider.GetRequiredService<ClassService>();
                    var assignmentService = scope.ServiceProvider.GetRequiredService<AssignmentService>();

                    if (user.IsTeacher)
                    {
                        var classes = await classService.GetOwnedClassesAsync(user.UserId);
                        classCount = classes.Count;

                        // GIÁO VIÊN: Đi từng lớp -> Đi từng bài tập -> Đếm số bài nộp chưa chấm điểm
                        foreach (var c in classes)
                        {
                            var assignments = await assignmentService.GetAssignmentsByClassIdAsync(c.ClassId);
                            foreach (var a in assignments)
                            {
                                if (a.Submissions != null)
                                {
                                    // Đếm số bài nộp có Score = null
                                    pendingActionCount += a.Submissions.Count(s => s.Score == null);
                                }
                            }
                        }
                    }
                    else // HỌC SINH
                    {
                        var classes = await classService.GetJoinedClassesAsync(user.UserId);
                        classCount = classes.Count;

                        // HỌC SINH: Đi từng lớp -> Đi từng bài tập -> Đếm bài mình chưa nộp
                        foreach (var c in classes)
                        {
                            var assignments = await assignmentService.GetAssignmentsByClassIdAsync(c.ClassId);
                            foreach (var a in assignments)
                            {
                                // Kiểm tra xem trong danh sách nộp bài có mặt mình chưa
                                bool isSubmitted = a.Submissions != null && a.Submissions.Any(s => s.StudentId == user.UserId);
                                if (!isSubmitted)
                                {
                                    pendingActionCount++;
                                }
                            }
                        }
                    }
                } // Đóng đường ống Database

                // Ép vẽ giao diện
                this.Invoke(new MethodInvoker(() =>
                {
                    flpDashboardCards.Controls.Clear();

                    if (user.IsTeacher)
                    {
                        flpDashboardCards.Controls.Add(CreateDashboardCard("LỚP ĐANG DẠY", $"{classCount} lớp", Color.FromArgb(33, 150, 243), "👨‍🏫"));
                        // Thay chữ "Cập nhật..." thành số lượng bài
                        flpDashboardCards.Controls.Add(CreateDashboardCard("BÀI NỘP CHỜ CHẤM", $"{pendingActionCount} bài", Color.FromArgb(255, 152, 0), "📝"));
                    }
                    else
                    {
                        flpDashboardCards.Controls.Add(CreateDashboardCard("LỚP ĐANG HỌC", $"{classCount} lớp", Color.FromArgb(76, 175, 80), "🎒"));
                        // Thay chữ "Cập nhật..." thành số lượng bài
                        flpDashboardCards.Controls.Add(CreateDashboardCard("BÀI TẬP CHỜ NỘP", $"{pendingActionCount} bài", Color.FromArgb(244, 67, 54), "⏳"));
                    }

                    flpDashboardCards.Controls.Add(CreateDashboardCard("VAI TRÒ TÀI KHOẢN", user.IsTeacher ? "Giáo Viên" : "Học Sinh", Color.FromArgb(156, 39, 176), "🛡️"));
                }));
            }
            catch (Exception ex)
            {
                this.Invoke(new MethodInvoker(() => {
                    MessageBox.Show("Lỗi tải thống kê Dashboard: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
            }
        }

        // 1. Hàm thiết lập giao diện trang Cài đặt
        private void SetupSettingsTab()
        {
            // Xóa sạch các nút cũ bị thừa (bao gồm cái nút Quản lý đề thi lơ lửng)
            tabSettings.Controls.Clear();
            tabSettings.BackColor = Color.White;

            // Tiêu đề
            MaterialLabel lblTitle = new MaterialLabel
            {
                Text = "⚙️ CÀI ĐẶT GIAO DIỆN ỨNG DỤNG",
                FontType = MaterialSkinManager.fontType.H6,
                Location = new Point(35, 30),
                AutoSize = true
            };

            // --- CÔNG TẮC DARK MODE ---
            MaterialSwitch switchDarkMode = new MaterialSwitch
            {
                Text = "🌙 Chế độ nền tối (Dark Mode)",
                Location = new Point(35, 80),
                AutoSize = true,
                Checked = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK
            };
            switchDarkMode.CheckedChanged += (s, e) =>
            {
                // Bật tắt Dark Mode ngay lập tức
                MaterialSkinManager.Instance.Theme = switchDarkMode.Checked ?
                    MaterialSkinManager.Themes.DARK : MaterialSkinManager.Themes.LIGHT;

                _ = LoadDashboardDataAsync();
            };

            // --- CHỌN MÀU CHỦ ĐẠO ---
            MaterialLabel lblTheme = new MaterialLabel
            {
                Text = "🎨 Chọn màu chủ đạo:",
                FontType = MaterialSkinManager.fontType.Subtitle1,
                Location = new Point(35, 160),
                AutoSize = true,
                ForeColor = Color.Gray
            };

            FlowLayoutPanel flpColors = new FlowLayoutPanel
            {
                Location = new Point(35, 200),
                Width = 800,
                Height = 100,
                WrapContents = true
            };

            // Thêm các lựa chọn màu sắc
            flpColors.Controls.Add(CreateColorOption("Xanh Dương", Color.FromArgb(30, 136, 229), Primary.Blue600, Primary.Blue700, Primary.Blue200, Accent.LightBlue200));
            flpColors.Controls.Add(CreateColorOption("Xanh Lá", Color.FromArgb(67, 160, 71), Primary.Green600, Primary.Green700, Primary.Green200, Accent.LightGreen200));
            flpColors.Controls.Add(CreateColorOption("Cam", Color.FromArgb(251, 140, 0), Primary.Orange600, Primary.Orange700, Primary.Orange200, Accent.DeepOrange200));
            flpColors.Controls.Add(CreateColorOption("Tím", Color.FromArgb(142, 36, 170), Primary.Purple600, Primary.Purple700, Primary.Purple200, Accent.DeepPurple200));
            flpColors.Controls.Add(CreateColorOption("Hồng", Color.FromArgb(216, 27, 96), Primary.Pink600, Primary.Pink700, Primary.Pink200, Accent.Pink200));

            tabSettings.Controls.Add(lblTitle);
            tabSettings.Controls.Add(switchDarkMode);
            tabSettings.Controls.Add(lblTheme);
            tabSettings.Controls.Add(flpColors);
        }

        // 2. Hàm hỗ trợ tạo ra các nút chọn màu
        private Button CreateColorOption(string name, Color bgColor, Primary p, Primary d, Primary l, Accent a)
        {
            Button btn = new Button
            {
                Text = name,
                Size = new Size(130, 45),
                BackColor = bgColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Roboto", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 15, 15)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += (s, e) =>
            {
                // Đổi màu toàn bộ ứng dụng
                MaterialSkinManager.Instance.ColorScheme = new ColorScheme(p, d, l, a, TextShade.WHITE);
                this.Refresh(); // Cập nhật lại giao diện ngay lập tức

                _ = LoadDashboardDataAsync();
            };
            return btn;
        }

        private void ZoomIn()
        {
            if (_currentZoom < MAX_ZOOM)
            {
                _currentZoom = Math.Min(_currentZoom + ZOOM_STEP, MAX_ZOOM);
                ApplyZoom();
            }
        }

        private void ZoomOut()
        {
            if (_currentZoom > MIN_ZOOM)
            {
                _currentZoom = Math.Max(_currentZoom - ZOOM_STEP, MIN_ZOOM);
                ApplyZoom();
            }
        }

        private void ResetZoom()
        {
            _currentZoom = 1.0f;
            ApplyZoom();
        }

        private void ApplyZoom()
        {
            // Áp dụng zoom cho tất cả các tab
            foreach (TabPage tab in materialTabControl1.TabPages)
            {
                foreach (Control ctrl in tab.Controls)
                {
                    if (ctrl is UcClassDetail ucDetail)
                    {
                        // Phóng to/thu nhỏ font và size của các control
                        ScaleControl(ucDetail, _currentZoom);
                    }
                }
            }

            // Phóng to/thu nhỏ form chính
            this.Font = new Font(this.Font.FontFamily, 8 * _currentZoom, this.Font.Style);

            // Update title bar to show zoom level
            this.Text = $"OmniSight Dashboard ({(int)(_currentZoom * 100)}%)";
        }

        private void ScaleControl(Control control, float zoomLevel)
        {
            if (control == null) return;

            try
            {
                // Scale font
                float newFontSize = 8 * zoomLevel;
                control.Font = new Font(control.Font?.FontFamily ?? new FontFamily("Roboto"), newFontSize, control.Font?.Style ?? FontStyle.Regular);

                // Scale child controls
                foreach (Control child in control.Controls)
                {
                    ScaleControl(child, zoomLevel);
                }
            }
            catch
            {
                // Ignore errors for controls that don't support scaling
            }
        }
    }
}