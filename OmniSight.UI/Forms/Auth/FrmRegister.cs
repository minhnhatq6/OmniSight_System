using MaterialSkin.Controls;
using OmniSight.Services;
using Microsoft.Extensions.DependencyInjection;

namespace OmniSight.UI.Forms.Auth
{
    public partial class FrmRegister : MaterialForm
    {
        private readonly AuthService _authService;
        private readonly IServiceProvider _serviceProvider;

        public FrmRegister(AuthService authService, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _authService = authService;
            _serviceProvider = serviceProvider;
        }

        private async void btnRegister_Click(object sender, EventArgs e)
        {
            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // SỬA LẠI CÁCH VIẾT NÀY ĐỂ TRÁNH LỖI CS8130
            var result = await _authService.RegisterAsync(txtEmail.Text, txtPassword.Text, txtFullName.Text);

            if (result.success)
            {
                MessageBox.Show("Đăng ký thành công! Vui lòng kiểm tra email để xác thực tài khoản.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lnkLogin_Click(sender, e); // Quay về trang login
            }
            else
            {
                MessageBox.Show(result.message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lnkLogin_Click(object sender, EventArgs e)
        {
            var loginForm = _serviceProvider.GetRequiredService<FrmLogin>();
            loginForm.Show();
            this.Hide(); // Dùng Hide thay Close để tránh lỗi
        }
    }
}