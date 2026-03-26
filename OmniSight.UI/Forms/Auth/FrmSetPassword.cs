using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MaterialSkin.Controls;
using OmniSight.Services;

namespace OmniSight.UI.Forms.Auth
{
    public partial class FrmSetPassword : MaterialForm
    {
        private readonly AuthService _authService;
        private readonly IUserService _userService;

        public FrmSetPassword(AuthService authService, IUserService userService)
        {
            InitializeComponent();
            _authService = authService;
            _userService = userService;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNewPassword.Text) || txtNewPassword.Text.Length < 6)
            {
                MessageBox.Show("Mật khẩu phải từ 6 ký tự trở lên!");
                return;
            }

            // Băm mật khẩu và lưu vào DB
            var user = _authService.CurrentUser;
            if (user != null)
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(txtNewPassword.Text);
                await _authService.UpdateUserFaceDataAsync(user); // Dùng hàm update có sẵn của bạn

                MessageBox.Show("Đã thiết lập mật khẩu thành công. Email của bạn chính là tên đăng nhập.");
                this.DialogResult = DialogResult.OK; // Báo hiệu thành công để FormLogin biết
                this.Close();
            }
        }
    }
}
