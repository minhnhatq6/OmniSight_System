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
        // FrmLogin.cs - btnLoginGoogle_Click
        public bool RedirectToProfile { get; set; } = false;

        private async void btnLoginGoogle_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(">> [GOOGLE_LOGIN] B1: Bắt đầu");

            bool success = await _authService.LoginWithGoogleAsync();
            System.Diagnostics.Debug.WriteLine($">> [GOOGLE_LOGIN] B2: LoginWithGoogleAsync = {success}");
            if (!success) return;

            var user = _authService.CurrentUser;
            System.Diagnostics.Debug.WriteLine($">> [GOOGLE_LOGIN] B3: CurrentUser = {user?.FullName ?? "NULL"}, UserId = {user?.UserId}");

            bool shouldRedirect = false;
            bool needsPassword = (user == null ||
                                  string.IsNullOrEmpty(user.PasswordHash) ||
                                  user.PasswordHash == "GOOGLE_AUTH");

            System.Diagnostics.Debug.WriteLine($">> [GOOGLE_LOGIN] B4: needsPassword = {needsPassword}, PasswordHash = '{user?.PasswordHash}'");

            if (needsPassword)
            {
                System.Diagnostics.Debug.WriteLine(">> [GOOGLE_LOGIN] B5: Mở FrmSetPassword...");
                using (var setPassForm = _serviceProvider.GetRequiredService<FrmSetPassword>())
                {
                    setPassForm.ShowDialog(this);
                    shouldRedirect = setPassForm.PasswordSaved;
                    System.Diagnostics.Debug.WriteLine($">> [GOOGLE_LOGIN] B6: ShowDialog xong, PasswordSaved = {shouldRedirect}");
                }
                System.Diagnostics.Debug.WriteLine(">> [GOOGLE_LOGIN] B7: using block kết thúc, setPassForm đã Dispose");

                if (!shouldRedirect)
                {
                    System.Diagnostics.Debug.WriteLine(">> [GOOGLE_LOGIN] B8: PasswordSaved = false -> Logout");
                    _authService.Logout();
                    MessageBox.Show("Cài đặt mật khẩu chưa hoàn tất. Đăng nhập bị hủy.");
                    return;
                }
            }

            System.Diagnostics.Debug.WriteLine($">> [GOOGLE_LOGIN] B9: Chuẩn bị gọi GoToMainForm(isRedirected={shouldRedirect})");
            GoToMainForm(isRedirected: shouldRedirect);
            System.Diagnostics.Debug.WriteLine(">> [GOOGLE_LOGIN] B10: GoToMainForm đã được gọi xong");
        }

        private void GoToMainForm(bool isRedirected = false)
        {
            System.Diagnostics.Debug.WriteLine($">> [GO_TO_MAIN] B1: isRedirected = {isRedirected}");
            this.RedirectToProfile = isRedirected;

            System.Diagnostics.Debug.WriteLine($">> [GO_TO_MAIN] B2: InvokeRequired = {this.InvokeRequired}");
            System.Diagnostics.Debug.WriteLine($">> [GO_TO_MAIN] B3: IsDisposed = {this.IsDisposed}, IsHandleCreated = {this.IsHandleCreated}");

            if (this.InvokeRequired)
            {
                System.Diagnostics.Debug.WriteLine(">> [GO_TO_MAIN] B4a: Gọi Invoke Close...");
                this.Invoke(new Action(() =>
                {
                    System.Diagnostics.Debug.WriteLine(">> [GO_TO_MAIN] B5a: Đang Close() từ Invoke...");
                    this.Close();
                    System.Diagnostics.Debug.WriteLine(">> [GO_TO_MAIN] B6a: Close() xong");
                }));
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(">> [GO_TO_MAIN] B4b: Gọi Close() trực tiếp...");
                this.Close();
                System.Diagnostics.Debug.WriteLine(">> [GO_TO_MAIN] B5b: Close() xong");
            }
        }

        // --- ĐĂNG NHẬP EMAIL ---
        private async void btnLoginEmail_Click(object sender, EventArgs e)
        {
            try
            {
                string email = txtEmail.Text.Trim();
                string password = txtPassword.Text;

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                    return;
                }

                // --- 1. KIỂM TRA TÀI KHOẢN ADMIN ---
                if (email == "admin" && password == "admin123")
                {
                    var adminForm = _serviceProvider.GetRequiredService<FrmAdminDashboard>();
                    adminForm.Show();
                    this.Hide();
                    return;
                }

                // --- 2. ĐĂNG NHẬP NGƯỜI DÙNG BÌNH THƯỜNG ---
                var result = await _authService.LoginWithEmailAsync(email, password);

                if (result.success)
                {
                    // Dùng BeginInvoke để "tách" luồng mở Main ra khỏi luồng đóng Login
                    // Việc này giúp tránh lỗi "tắt app luôn" do xung đột tài nguyên
                    this.BeginInvoke(new Action(() => {
                        try
                        {
                            var mainForm = _serviceProvider.GetRequiredService<MainForm>();
                            mainForm.Show();
                            this.Hide();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi khởi động giao diện chính: " + ex.Message);
                        }
                    }));
                }
                else
                {
                    MessageBox.Show(result.message, "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hệ thống: " + ex.Message);
            }
        }

        // --- CHUYỂN TRANG ĐĂNG KÝ ---
        private void lnkRegister_Click(object sender, EventArgs e)
        {
            // XÓA LỆNH this.Hide(); Ở ĐÂY

            using (var registerForm = _serviceProvider.GetRequiredService<FrmRegister>())
            {
                registerForm.ShowDialog(this);
            }

            // XÓA LỆNH this.Show(); Ở ĐÂY
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