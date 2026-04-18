using MaterialSkin.Controls;
using Microsoft.Extensions.DependencyInjection;
using OmniSight.Services;
using OmniSight.Core.Entities; // Thêm để dùng class User
using System;
using System.Collections.Generic;
using System.Windows.Forms;

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

        // --- ĐĂNG NHẬP GOOGLE ---
        private async void btnLoginGoogle_Click(object sender, EventArgs e)
        {
            bool success = await _authService.LoginWithGoogleAsync();
            if (success)
            {
                var user = _authService.CurrentUser;
                if (user == null || string.IsNullOrEmpty(user.PasswordHash) || user.PasswordHash == "GOOGLE_AUTH")
                {
                    using (var setPassForm = _serviceProvider.GetRequiredService<FrmSetPassword>())
                    {
                        if (setPassForm.ShowDialog() == DialogResult.OK) GoToMainForm();
                        else _authService.Logout();
                    }
                }
                else GoToMainForm();
            }
        }

        // --- ĐĂNG NHẬP EMAIL ---
        private async void btnLoginEmail_Click(object sender, EventArgs e)
        {
            var result = await _authService.LoginWithEmailAsync(txtEmail.Text, txtPassword.Text);
            if (result.success) GoToMainForm();
            else MessageBox.Show(result.message, "Lỗi Đăng nhập");
        }

        // --- CHUYỂN TRANG ĐĂNG KÝ ---
        private void lnkRegister_Click(object sender, EventArgs e)
        {
            var registerForm = _serviceProvider.GetRequiredService<FrmRegister>();
            registerForm.Show();
            this.Hide();
        }
        public string? AutoLoginToken { get; set; }

        private async void FrmLogin_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(AutoLoginToken))
            {
                // Hiển thị trạng thái đang xử lý (nếu có label)
                var result = await _authService.VerifyEmailAndLoginAsync(AutoLoginToken);
                if (result.success)
                {
                    MessageBox.Show("Xác thực email thành công! Đang tự động đăng nhập...");
                    GoToMainForm(isRedirected: true); // Chuyền thêm flag vào
                }
                else
                {
                    MessageBox.Show("Link xác thực đã hết hạn hoặc không hợp lệ.");
                }
            }
        }
        // --- ĐĂNG NHẬP KHUÔN MẶT (CÓ CHỌN ACC) ---
        private void btnLoginFace_Click(object sender, EventArgs e)
        {
            // Mở form quét mặt
            using (var faceLoginForm = _serviceProvider.GetRequiredService<FrmFaceLogin>())
            {
                if (faceLoginForm.ShowDialog() == DialogResult.OK)
                {
                    // LẤY DANH SÁCH CÁC ACC KHỚP MẶT TỪ FORM QUÉT MẶT
                    // (Bạn cần thêm property 'MatchedUsers' vào class FrmFaceLogin)
                    var matchedUsers = faceLoginForm.MatchedUsers;

                    if (matchedUsers == null || matchedUsers.Count == 0)
                    {
                        MessageBox.Show("Không tìm thấy dữ liệu khuôn mặt khớp!");
                        return;
                    }

                    if (matchedUsers.Count == 1)
                    {
                        // Nếu chỉ có 1 người, đăng nhập luôn
                        _authService.SetCurrentUser(matchedUsers[0]);
                        FinishLogin();
                    }
                    else
                    {
                        // NẾU CÓ TỪ 2 NGƯỜI TRỞ LÊN: Hiện bảng chọn
                        using (var chooseAccForm = new FrmChooseAccount(matchedUsers))
                        {
                            if (chooseAccForm.ShowDialog() == DialogResult.OK)
                            {
                                _authService.SetCurrentUser(chooseAccForm.SelectedUser);
                                FinishLogin();
                            }
                        }
                    }
                }
            }
        }

        private void FinishLogin()
        {
            MessageBox.Show($"Xác thực thành công! Chào mừng {_authService.CurrentUser?.FullName}.", "Thành công");
            GoToMainForm();
        }

        private void GoToMainForm(bool isRedirected = false)
        {
            var mainForm = _serviceProvider.GetRequiredService<MainForm>();
            if (isRedirected)
            {
                mainForm.StartAtProfile = true; // Thêm property này vào MainForm
            }
            mainForm.Show();
            this.Hide();
        }
    }

    // --- FORM CHỌN TÀI KHOẢN (Để cùng file hoặc tách file tùy bạn) ---
    public class FrmChooseAccount : Form
    {
        private ListBox listBoxUsers;
        private Button btnLogin;
        public User SelectedUser { get; private set; }

        public FrmChooseAccount(List<User> users)
        {
            this.Text = "Chọn tài khoản đăng nhập";
            this.Size = new System.Drawing.Size(350, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            Label lbl = new Label
            {
                Text = "Phát hiện nhiều tài khoản có khuôn mặt này.\nVui lòng chọn đúng tài khoản của bạn:",
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(10)
            };

            listBoxUsers = new ListBox { Dock = DockStyle.Fill, Font = new System.Drawing.Font("Roboto", 11) };
            listBoxUsers.DataSource = users;
            listBoxUsers.DisplayMember = "FullName"; // Hoặc hiện "Email" để phân biệt rõ nhất

            btnLogin = new Button
            {
                Text = "XÁC NHẬN ĐĂNG NHẬP",
                Dock = DockStyle.Bottom,
                Height = 45,
                BackColor = System.Drawing.Color.FromArgb(76, 175, 80),
                ForeColor = System.Drawing.Color.White
            };
            btnLogin.Click += btnLogin_Click;

            this.Controls.Add(listBoxUsers);
            this.Controls.Add(lbl);
            this.Controls.Add(btnLogin);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (listBoxUsers.SelectedItem != null)
            {
                SelectedUser = (User)listBoxUsers.SelectedItem;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}