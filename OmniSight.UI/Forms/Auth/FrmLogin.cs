using MaterialSkin.Controls;
using OmniSight.Services;
using System;
using System.Windows.Forms;

namespace OmniSight.UI.Forms.Auth
{
    public partial class FrmLogin : MaterialForm
    {
        private readonly AuthService _authService;

        public FrmLogin(AuthService authService)
        {
            _authService = authService;
            InitializeComponent();
        }

        private async void btnLoginGoogle_Click(object sender, EventArgs e)
        {
            try
            {
                var result = await _authService.LoginWithGoogleAsync();
                if (result)
                {
                    MessageBox.Show("Đăng nhập Google thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Đăng nhập Google thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
