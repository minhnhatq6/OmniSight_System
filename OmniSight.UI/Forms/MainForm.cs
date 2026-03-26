using MaterialSkin;
using MaterialSkin.Controls;
using Microsoft.Extensions.DependencyInjection;
using OmniSight.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace OmniSight.UI.Forms
{
    public partial class MainForm : MaterialForm
    {
        private readonly AuthService _authService;
        private readonly IUserService _userService;
        private readonly IServiceProvider _serviceProvider;
        private bool _isLoggingOut = false;

        public MainForm(AuthService authService, IUserService userService, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _authService = authService;
            _userService = userService;
            _serviceProvider = serviceProvider;

            // Cấu hình Theme
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
            // Kích hoạt Drawer (Menu bên trái)
            this.DrawerTabControl = this.materialTabControl1;

            var user = _authService.CurrentUser;
            if (user != null)
            {
                lblHomeWelcome.Text = $"Chào mừng {user.FullName} đến với OmniSight!";
                btnUserAccount.Text = user.FullName.ToUpper();

                txtFullName.Text = user.FullName ?? "";
                txtPhone.Text = user.Phone ?? "";
                switchStudent.Checked = user.IsStudent;
                switchTeacher.Checked = user.IsTeacher;
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
                // 1. Xóa session (xóa file session.json)
                _authService.Logout();

                // 2. Khởi động lại toàn bộ ứng dụng
                // Lệnh này cực kỳ an toàn vì nó sẽ chạy lại từ Program.cs
                Application.Restart();

                // 3. Thoát khỏi môi trường hiện tại
                Environment.Exit(0);
            }
        }

        private async void btnSaveProfile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text)) return;

            var userId = _authService.CurrentUser.UserId;
            bool success = await _userService.UpdateProfileAsync(userId, txtFullName.Text, txtPhone.Text, switchStudent.Checked, switchTeacher.Checked);

            if (success)
            {
                _authService.CurrentUser.FullName = txtFullName.Text;
                btnUserAccount.Text = txtFullName.Text.ToUpper();
                lblHomeWelcome.Text = $"Chào mừng {txtFullName.Text} đến với OmniSight!";
                MessageBox.Show("Cập nhật thành công!");
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!_isLoggingOut) Application.Exit();
        }
    }
}