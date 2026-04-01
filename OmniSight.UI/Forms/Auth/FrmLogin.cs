using MaterialSkin.Controls;
using Microsoft.Extensions.DependencyInjection;
using OmniSight.Services;
using OmniSight.UI.Forms;

namespace OmniSight.UI.Forms.Auth
{
    public partial class FrmLogin : MaterialForm
    {
        private readonly AuthService _authService;
        private readonly IServiceProvider _serviceProvider;

        public FrmLogin(AuthService authService, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _authService = authService;
            _serviceProvider = serviceProvider;
        }

        private async void btnLoginGoogle_Click(object sender, EventArgs e)
        {
            bool success = await _authService.LoginWithGoogleAsync();
            if (success)
            {
                var user = _authService.CurrentUser;

                // KIỂM TRA: Nếu user chưa có mật khẩu (do mới Login Google lần đầu)
                if (user == null || string.IsNullOrEmpty(user.PasswordHash) || user.PasswordHash == "GOOGLE_AUTH")
                {
                    using (var setPassForm = _serviceProvider.GetRequiredService<FrmSetPassword>())
                    {
                        if (setPassForm.ShowDialog() == DialogResult.OK)
                        {
                            GoToMainForm(); // Xong thì vào Main
                        }
                        else
                        {
                            // Nếu tắt form set pass giữa chừng thì đăng xuất luôn cho an toàn
                            _authService.Logout();
                        }
                    }
                }
                else
                {
                    // Nếu đã có mật khẩu rồi thì vào thẳng
                    GoToMainForm();
                }
            }
        }

        // SỰ KIỆN NÚT ĐĂNG NHẬP THƯỜNG
        private async void btnLoginEmail_Click(object sender, EventArgs e)
        {
            // SỬA LẠI CÁCH VIẾT NÀY ĐỂ TRÁNH LỖI CS8130
            var result = await _authService.LoginWithEmailAsync(txtEmail.Text, txtPassword.Text);
            if (result.success)
            {
                GoToMainForm();
            }
            else
            {
                MessageBox.Show(result.message, "Lỗi Đăng nhập");
            }
        }

        // SỰ KIỆN LINK SANG TRANG ĐĂNG KÝ
        private void lnkRegister_Click(object sender, EventArgs e)
        {
            var registerForm = _serviceProvider.GetRequiredService<FrmRegister>();
            registerForm.Show();
            this.Hide();
        }
        // ĐĂNG NHẬP BẰNG KHUÔN MẶT
        private void btnLoginFace_Click(object sender, EventArgs e)
        {
            // Mở form quét mặt (FrmFaceLogin bạn cần tạo ở bước trước)
            using (var faceLoginForm = _serviceProvider.GetRequiredService<FrmFaceLogin>())
            {
                if (faceLoginForm.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show($"Xác thực khuôn mặt thành công! Chào mừng {_authService.CurrentUser?.FullName}.", "Thành công");
                    GoToMainForm();
                }
            }
        }

        // Hàm chung để chuyển form, tránh lặp code
        private void GoToMainForm()
        {
            var mainForm = _serviceProvider.GetRequiredService<MainForm>();
            mainForm.Show();
            this.Hide();
        }

    }
}