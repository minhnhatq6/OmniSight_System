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
                Primary.Blue200, Accent.Amber200,
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