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
    // FrmSetPassword.cs - Thêm property PasswordSaved, bỏ DialogResult
    public partial class FrmSetPassword : Form
    {
        private readonly AuthService _authService;
        private readonly IUserService _userService;

        public bool PasswordSaved { get; private set; } = false;
        private bool _isSaving = false; // Cờ theo dõi trạng thái lưu

        public FrmSetPassword(AuthService authService, IUserService userService)
        {
            InitializeComponent();
            _authService = authService;
            _userService = userService;
        }

        // FrmSetPassword.cs - Viết lại btnSave_Click không dùng async void
        private void btnSave_Click(object sender, EventArgs e)
        {
            string passwordToSave = txtNewPassword.Text;
            if (string.IsNullOrWhiteSpace(passwordToSave) || passwordToSave.Length < 6)
            {
                MessageBox.Show("Mật khẩu phải từ 6 ký tự trở lên!");
                return;
            }

            var user = _authService.CurrentUser;
            if (user == null)
            {
                MessageBox.Show("Lỗi: Không tìm thấy user!");
                return;
            }

            try
            {
                btnSave.Enabled = false;
                btnSave.Text = "ĐANG LƯU...";

                // Hash password đồng bộ - không cần async
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordToSave);

                // Chạy async task trên thread pool, chờ kết quả ĐỒNG BỘ
                // Tránh dùng async void gây nuốt exception
                System.Diagnostics.Debug.WriteLine(">> [SET_PASS] B1: Bắt đầu lưu...");

                Task.Run(async () => await _authService.UpdateUserFaceDataAsync(user))
                    .GetAwaiter()
                    .GetResult();

                System.Diagnostics.Debug.WriteLine(">> [SET_PASS] B2: Lưu DB xong");

                // Thay thế this.Close() bằng:
                this.PasswordSaved = true;
                System.Diagnostics.Debug.WriteLine(">> [SET_PASS] B3: PasswordSaved = true, set DialogResult...");

                // Gán DialogResult sẽ tự động đóng form VÀ giải phóng ShowDialog()
                this.DialogResult = DialogResult.OK;

                System.Diagnostics.Debug.WriteLine(">> [SET_PASS] B4: DialogResult đã set");
                // KHÔNG gọi this.Close() nữa — DialogResult tự đóng
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(">> [SET_PASS] LỖI: " + ex.Message + "\n" + ex.StackTrace);
                MessageBox.Show("Lỗi lưu mật khẩu: " + ex.Message);
                btnSave.Enabled = true;
                btnSave.Text = "XÁC NHẬN";
            }
        }

       
    }
}
