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
            this.Shown += MainForm_Shown;
            // XÓA LoadClassList() khỏi constructor - gọi sau khi form load xong

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);

            // Load cài đặt từ file
            var uiSettings = LoadUiSettings();
            materialSkinManager.Theme = uiSettings.isDark ? MaterialSkinManager.Themes.DARK : MaterialSkinManager.Themes.LIGHT;
            ApplyColorByIndex(uiSettings.colorIndex);
        
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
            System.Diagnostics.Debug.WriteLine(">> [MAIN_LOAD] B1: Thiết lập UI ban đầu");

            this.DrawerTabControl = this.materialTabControl1;
            this.KeyPreview = true;
            this.KeyDown += MainForm_KeyDown;

            var user = _authService.CurrentUser;

            // 1. Nếu là đăng nhập lần đầu, chỉ chuyển Tab (không hiện MessageBox ở đây)
            if (StartAtProfile)
            {
                materialTabControl1.SelectedTab = tabProfile;
            }

            // 2. Đổ dữ liệu User lên giao diện
            if (user != null)
            {
                lblHomeWelcome.Text = $"Chào mừng {user.FullName} đến với OmniSight!";
                btnUserAccount.Text = user.FullName.ToUpper();
                txtFullName.Text = user.FullName ?? "";
                txtPhone.Text = user.Phone ?? "";

                SetupHomeTab();
                SetupSettingsTab();

                switchStudent.Checked = user.IsStudent;
                switchTeacher.Checked = user.IsTeacher;
            }

            // 3. Chạy các tác vụ ngầm (Dashboard, AI) mà không chặn UI
            BeginInvoke(new Action(() =>
            {
                _ = Task.Run(() => LoadDashboardDataAsync());

                _ = Task.Run(() =>
                {
                    try
                    {
                        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                        string detector = Path.Combine(baseDir, "Models", "face_detection_yunet_2023mar.onnx");
                        string recognizer = Path.Combine(baseDir, "Models", "face_recognition_sface_2021dec.onnx");
                        _faceAiService.InitializeModels(detector, recognizer);
                    }
                    catch (Exception ex) { System.Diagnostics.Debug.WriteLine("Lỗi AI: " + ex.Message); }
                });

                _ = LoadClassList();
            }));
        }

        // HÀM NÀY SẼ CHẠY SAU KHI FORM HIỆN LÊN HOÀN TOÀN
        private void MainForm_Shown(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(">> [MAIN_SHOWN] Form đã hiển thị xong");

            if (StartAtProfile)
            {
                // Lúc này Form đã vẽ xong hoàn toàn, gọi MessageBox sẽ không bị treo app
                MessageBox.Show(this,
                    "Chào mừng thành viên mới!\n\nBạn hãy dành ít giây để:\n1. Chọn Vai trò.\n2. Cài đặt Face ID.",
                    "Hoàn tất hồ sơ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
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
                    _=LoadClassList();
                }
            }
        }

        private async Task LoadClassList()
        {
            System.Diagnostics.Debug.WriteLine(">> [CLASSLIST] B1: Bắt đầu");
            try
            {
                lvwClasses.Items.Clear();
                var _currentUser = _authService.CurrentUser;
                if (_currentUser == null)
                {
                    System.Diagnostics.Debug.WriteLine(">> [CLASSLIST] B2: user null, return");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($">> [CLASSLIST] B3: IsTeacher={_currentUser.IsTeacher}, IsStudent={_currentUser.IsStudent}");
                var _classService = _serviceProvider.GetRequiredService<ClassService>();

                if (_currentUser.IsTeacher)
                {
                    System.Diagnostics.Debug.WriteLine(">> [CLASSLIST] B4: Lấy owned classes...");
                    var ownedClasses = await _classService.GetOwnedClassesAsync(_currentUser.UserId);
                    System.Diagnostics.Debug.WriteLine($">> [CLASSLIST] B5: Có {ownedClasses.Count} lớp dạy");
                    foreach (var c in ownedClasses)
                    {
                        string fullDisplayName = $"{c.ClassName} - {c.Subject?.SubjectName ?? "N/A"}";
                        var item = new ListViewItem(fullDisplayName);
                        item.SubItems.Add(c.JoinCode);
                        item.Tag = c.ClassId;
                        lvwClasses.Items.Add(item);
                    }
                }

                if (_currentUser.IsStudent)
                {
                    System.Diagnostics.Debug.WriteLine(">> [CLASSLIST] B6: Lấy joined classes...");
                    var joinedClasses = await _classService.GetJoinedClassesAsync(_currentUser.UserId);
                    System.Diagnostics.Debug.WriteLine($">> [CLASSLIST] B7: Có {joinedClasses.Count} lớp học");
                    foreach (var c in joinedClasses)
                    {
                        if (lvwClasses.Items.Cast<ListViewItem>().Any(i => (int)i.Tag == c.ClassId)) continue;
                        string fullDisplayName = $"{c.ClassName} - {c.Subject?.SubjectName ?? "N/A"}";
                        var item = new ListViewItem(fullDisplayName);
                        item.SubItems.Add("******");
                        item.Tag = c.ClassId;
                        lvwClasses.Items.Add(item);
                    }
                }
                System.Diagnostics.Debug.WriteLine(">> [CLASSLIST] B8: Hoàn tất");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(">> [CLASSLIST] LỖI: " + ex.Message + "\n" + ex.StackTrace);
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
                    _=LoadClassList();
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
    System.Diagnostics.Debug.WriteLine(">> [DASHBOARD] B1: Bắt đầu");
    try
    {
        var user = _authService.CurrentUser;
        if (user == null || flpDashboardCards == null)
        {
            System.Diagnostics.Debug.WriteLine(">> [DASHBOARD] B2: null, return");
            return;
        }

        int classCount = 0;
        int pendingActionCount = 0;

        using (var scope = _serviceProvider.CreateScope())
        {
            var classService = scope.ServiceProvider.GetRequiredService<ClassService>();
            var assignmentService = scope.ServiceProvider.GetRequiredService<AssignmentService>();

            if (user.IsTeacher)
            {
                var classes = await classService.GetOwnedClassesAsync(user.UserId).ConfigureAwait(false);
                classCount = classes.Count;
                foreach (var c in classes)
                {
                    var assignments = await assignmentService.GetAssignmentsByClassIdAsync(c.ClassId).ConfigureAwait(false);
                    foreach (var a in assignments)
                        if (a.Submissions != null)
                            pendingActionCount += a.Submissions.Count(s => s.Score == null);
                }
            }
            else
            {
                var classes = await classService.GetJoinedClassesAsync(user.UserId).ConfigureAwait(false);
                classCount = classes.Count;
                foreach (var c in classes)
                {
                    var assignments = await assignmentService.GetAssignmentsByClassIdAsync(c.ClassId).ConfigureAwait(false);
                    foreach (var a in assignments)
                    {
                        bool isSubmitted = a.Submissions != null && a.Submissions.Any(s => s.StudentId == user.UserId);
                        if (!isSubmitted) pendingActionCount++;
                    }
                }
            }
        }

        System.Diagnostics.Debug.WriteLine(">> [DASHBOARD] B4: Xong DB");

        if (!this.IsHandleCreated)
        {
            System.Diagnostics.Debug.WriteLine(">> [DASHBOARD] B4b: Handle chưa có, đợi...");
            await Task.Delay(300).ConfigureAwait(false);
        }

        System.Diagnostics.Debug.WriteLine(">> [DASHBOARD] B4c: Gọi BeginInvoke");
        this.BeginInvoke(new MethodInvoker(() =>
        {
            System.Diagnostics.Debug.WriteLine(">> [DASHBOARD] B5: Vẽ UI");
            flpDashboardCards.Controls.Clear();

            if (user.IsTeacher)
            {
                flpDashboardCards.Controls.Add(CreateDashboardCard("LỚP ĐANG DẠY", $"{classCount} lớp", Color.FromArgb(33, 150, 243), "👨‍🏫"));
                flpDashboardCards.Controls.Add(CreateDashboardCard("BÀI NỘP CHỜ CHẤM", $"{pendingActionCount} bài", Color.FromArgb(255, 152, 0), "📝"));
            }
            else
            {
                flpDashboardCards.Controls.Add(CreateDashboardCard("LỚP ĐANG HỌC", $"{classCount} lớp", Color.FromArgb(76, 175, 80), "🎒"));
                flpDashboardCards.Controls.Add(CreateDashboardCard("BÀI TẬP CHỜ NỘP", $"{pendingActionCount} bài", Color.FromArgb(244, 67, 54), "⏳"));
            }

            flpDashboardCards.Controls.Add(CreateDashboardCard("VAI TRÒ TÀI KHOẢN", user.IsTeacher ? "Giáo Viên" : "Học Sinh", Color.FromArgb(156, 39, 176), "🛡️"));
            System.Diagnostics.Debug.WriteLine(">> [DASHBOARD] B6: Hoàn tất");
        }));
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine(">> [DASHBOARD] LỖI: " + ex.Message + "\n" + ex.StackTrace);
        if (this.IsHandleCreated)
        {
            this.BeginInvoke(new MethodInvoker(() =>
                MessageBox.Show("Lỗi Dashboard: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error)));
        }
    }
}

        // 1. Hàm thiết lập giao diện trang Cài đặt
       // 1. Hàm thiết lập giao diện trang Cài đặt (Chuẩn MaterialSkin)
        private void SetupSettingsTab()
        {
            tabSettings.Controls.Clear();
            
            // XÓA HẾT CÁC LỆNH ÉP MÀU NỀN (BackColor = White/Transparent) ĐI
            // Hãy để MaterialSkin tự động lo màu nền cho Tab này!

            MaterialLabel lblTitle = new MaterialLabel
            {
                Text = "⚙️ CÀI ĐẶT GIAO DIỆN ỨNG DỤNG",
                FontType = MaterialSkinManager.fontType.H6,
                Location = new Point(35, 30),
                AutoSize = true
            };

            var currentSettings = LoadUiSettings();

            MaterialSwitch switchDarkMode = new MaterialSwitch
            {
                Text = "🌙 Chế độ nền tối (Dark Mode)",
                Location = new Point(35, 80),
                AutoSize = true,
                Checked = currentSettings.isDark
            };
            
            switchDarkMode.CheckedChanged += (s, e) =>
            {
                MaterialSkinManager.Instance.Theme = switchDarkMode.Checked ? MaterialSkinManager.Themes.DARK : MaterialSkinManager.Themes.LIGHT;
                
                int currentColorIndex = LoadUiSettings().colorIndex;
                SaveUiSettings(switchDarkMode.Checked, currentColorIndex);

                this.Invalidate(true);
                this.Refresh();
                _ = LoadDashboardDataAsync();
            };

            MaterialLabel lblTheme = new MaterialLabel
            {
                Text = "🎨 Chọn màu chủ đạo:",
                FontType = MaterialSkinManager.fontType.Subtitle1,
                Location = new Point(35, 160),
                AutoSize = true
            };

            FlowLayoutPanel flpColors = new FlowLayoutPanel
            {
                Location = new Point(35, 200),
                Width = 800,
                Height = 100,
                WrapContents = true
            };

            // Tạo các Ảnh màu (Dùng ID từ 0 đến 4)
            flpColors.Controls.Add(CreateColorOption("Xanh Dương", Color.FromArgb(30, 136, 229), 0));
            flpColors.Controls.Add(CreateColorOption("Xanh Lá", Color.FromArgb(67, 160, 71), 1));
            flpColors.Controls.Add(CreateColorOption("Cam", Color.FromArgb(251, 140, 0), 2));
            flpColors.Controls.Add(CreateColorOption("Tím", Color.FromArgb(142, 36, 170), 3));
            flpColors.Controls.Add(CreateColorOption("Hồng", Color.FromArgb(216, 27, 96), 4));

            tabSettings.Controls.Add(lblTitle);
            tabSettings.Controls.Add(switchDarkMode);
            tabSettings.Controls.Add(lblTheme);
            tabSettings.Controls.Add(flpColors);
        }

        // 2. TUYỆT CHIÊU: Biến cái nút thành một bức ảnh (PictureBox)
        private PictureBox CreateColorOption(string name, Color bgColor, int colorIndex)
        {
            // Tạo một bức ảnh kích thước 130x45
            Bitmap bmp = new Bitmap(130, 45);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // Tô màu nền cho bức ảnh
                g.Clear(bgColor);
                
                // Vẽ chữ màu trắng lên giữa bức ảnh
                using (Font font = new Font("Roboto", 11, FontStyle.Bold))
                using (Brush brush = new SolidBrush(Color.White))
                {
                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    g.DrawString(name, font, brush, new RectangleF(0, 0, 130, 45), sf);
                }
            }

            // Bỏ bức ảnh vào PictureBox (MaterialSkin không thể đụng vào Image)
            PictureBox pic = new PictureBox
            {
                Image = bmp,
                Size = new Size(130, 45),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 15, 15)
            };

            pic.Click += (s, e) =>
            {
                ApplyColorByIndex(colorIndex);
                
                bool isDark = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
                SaveUiSettings(isDark, colorIndex);

                this.Refresh(); 
                _ = LoadDashboardDataAsync();
            };

            return pic;
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
        // --- HỆ THỐNG LƯU TRỮ CÀI ĐẶT GIAO DIỆN ---
        private string GetUiSettingsFilePath()
        {
            // Lưu file cấu hình ui_settings.txt vào cùng chỗ với session.json
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OmniSight", "ui_settings.txt");
        }

        private void SaveUiSettings(bool isDark, int colorIndex)
        {
            try
            {
                string path = GetUiSettingsFilePath();
                File.WriteAllText(path, $"{isDark}|{colorIndex}");
            }
            catch { }
        }

        private (bool isDark, int colorIndex) LoadUiSettings()
        {
            try
            {
                string path = GetUiSettingsFilePath();
                if (File.Exists(path))
                {
                    var parts = File.ReadAllText(path).Split('|');
                    if (parts.Length == 2)
                    {
                        return (bool.Parse(parts[0]), int.Parse(parts[1]));
                    }
                }
            }
            catch { }
            return (false, 0); // Mặc định: Nền sáng, Màu Xanh dương
        }

        private void ApplyColorByIndex(int index)
        {
            var msm = MaterialSkinManager.Instance;
            switch (index)
            {
                case 1: msm.ColorScheme = new ColorScheme(Primary.Green600, Primary.Green700, Primary.Green200, Accent.LightGreen200, TextShade.WHITE); break;
                case 2: msm.ColorScheme = new ColorScheme(Primary.Orange600, Primary.Orange700, Primary.Orange200, Accent.DeepOrange200, TextShade.WHITE); break;
                case 3: msm.ColorScheme = new ColorScheme(Primary.Purple600, Primary.Purple700, Primary.Purple200, Accent.DeepPurple200, TextShade.WHITE); break;
                case 4: msm.ColorScheme = new ColorScheme(Primary.Pink600, Primary.Pink700, Primary.Pink200, Accent.Pink200, TextShade.WHITE); break;
                default: msm.ColorScheme = new ColorScheme(Primary.Blue600, Primary.Blue700, Primary.Blue200, Accent.LightBlue200, TextShade.WHITE); break;
            }
        }
    }
}