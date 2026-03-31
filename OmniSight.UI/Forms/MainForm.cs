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

        // Lưu ý: KHÔNG khai báo timerCamera ở đây nữa vì nó đã có trong Designer

        public MainForm(AuthService authService, IUserService userService, IServiceProvider serviceProvider, FaceAiService faceAiService)
        {
            InitializeComponent();
            _authService = authService;
            _userService = userService;
            _serviceProvider = serviceProvider;
            _faceAiService = faceAiService;
            LoadClassList(); // Load danh sách lớp học ngay khi mở form

            // Cấu hình Theme Material
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Blue600, Primary.Blue700,
                Primary.Blue200, Accent.Amber200,
                TextShade.WHITE
            );
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.DrawerTabControl = this.materialTabControl1;

            var user = _authService.CurrentUser;
            if (user != null)
            {
                lblHomeWelcome.Text = $"Chào mừng {user.FullName} đến với OmniSight!";
                btnUserAccount.Text = user.FullName.ToUpper();
                txtFullName.Text = user.FullName ?? "";
                // txtPhone, switchStudent... (Các control này phải khớp với Designer)
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

            // Giả sử IUserService có phương thức này
            bool success = await _userService.UpdateProfileAsync(user.UserId, txtFullName.Text, txtPhone.Text, switchStudent.Checked, switchTeacher.Checked);

            if (success)
            {
                user.FullName = txtFullName.Text;
                btnUserAccount.Text = txtFullName.Text.ToUpper();
                MessageBox.Show("Cập nhật thành công!");
            }
        }

        private void btnStartCamera_Click(object sender, EventArgs e)
        {
            if (_faceAiService.StartCamera(0))
            {
                timerCamera.Start(); // Timer này lấy từ Designer
                btnStartCamera.Enabled = false;
            }
            else MessageBox.Show("Không thể mở Camera!");
        }

        private void timerCamera_Tick(object sender, EventArgs e)
        {
            using (Mat frame = _faceAiService.GetFrame())
            {
                if (frame != null && !frame.IsEmpty)
                {
                    var oldImage = picCamera.Image;
                    // Fix lỗi ToBitmap(): Cần cài NuGet Emgu.CV.Bitmap
                    picCamera.Image = frame.ToBitmap();
                    oldImage?.Dispose();
                }
            }
        }

        private async void btnCaptureFace_Click(object sender, EventArgs e)
        {
            using (Mat frame = _faceAiService.GetFrame())
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

            // 1. Lấy ClassService ra từ Dependency Injection (bạn đã có sẵn _serviceProvider trong MainForm)
            var classService = _serviceProvider.GetRequiredService<ClassService>();

            // 2. Lấy ID của người dùng đang đăng nhập
            // (Lưu ý: Bạn hãy thay _currentUser.UserId bằng biến lưu User ID thực tế trong MainForm của bạn nhé)
            int currentTeacherId = _authService.CurrentUser?.UserId ?? 0; // Giả sử UserId là int, nếu không có user nào đăng nhập thì mặc định là 0    

            // 3. Khởi tạo và mở Form Tạo Lớp
            using (var frmCreate = new FrmCreateClass(classService, currentTeacherId))
            {
                // ShowDialog() sẽ mở form lên và bắt người dùng phải thao tác xong mới được quay lại MainForm
                var result = frmCreate.ShowDialog();

                // 4. Kiểm tra xem người dùng đã bấm "Tạo lớp" thành công hay bấm nút "X" tắt đi
                if (result == DialogResult.OK)
                {
                    // Nếu tạo thành công, gọi hàm load lại danh sách lớp học để hiển thị lên màn hình
                    LoadClassList();
                }
            }
        }

        private async void LoadClassList()
        {
            lvwClasses.Items.Clear();

            var _currentUser = _authService.CurrentUser;

            var _classService = _serviceProvider.GetRequiredService<ClassService>();

            // Giả sử bạn có biến check role (Dựa trên switchTeacher/switchStudent trong Profile)
            if (_currentUser.IsTeacher)
            {
                var classes = await _classService.GetOwnedClassesAsync(_currentUser.UserId);
                foreach (var c in classes)
                {
                    var item = new ListViewItem(c.ClassName);
                    item.SubItems.Add(c.JoinCode); // Giáo viên thì hiện Join Code
                    item.Tag = c.ClassId;
                    lvwClasses.Items.Add(item);
                }
            }
            else // Chế độ Sinh viên
            {
                var classes = await _classService.GetJoinedClassesAsync(_currentUser.UserId);
                foreach (var c in classes)
                {
                    var item = new ListViewItem(c.ClassName);
                    item.SubItems.Add("******"); // Sinh viên thì giấu Join Code đi
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
                    LoadClassList(); // Load lại danh sách lớp để thấy lớp vừa tham gia
                }
            }
        }

        private void lvwClasses_MouseDoubleClick(object sender, EventArgs e)
        {
            var _currentUser = _authService.CurrentUser; // Lấy thông tin người dùng hiện tại (nếu chưa có sẵn trong biến toàn cục)
            // 1. Kiểm tra xem người dùng có thực sự đang chọn 1 dòng nào không
            if (lvwClasses.SelectedItems.Count == 0) return;

            // 2. Lấy dữ liệu từ dòng đang được chọn
            var selectedItem = lvwClasses.SelectedItems[0];
            string className = selectedItem.Text;       // Lấy tên lớp
            int classId = (int)selectedItem.Tag;        // Lấy cái ID lớp học đã giấu lúc nãy

            // 3. Lấy Service và ID người dùng hiện tại
            var streamService = _serviceProvider.GetRequiredService<StreamService>();
            int currentUserId = _currentUser.UserId;

            // 4. Mở Form Chi tiết Lớp học
            using (var frmDetail = new FrmClassDetail(streamService, currentUserId, classId, className))
            {
                frmDetail.ShowDialog(); // Mở form lên
            }
        }
    }
}