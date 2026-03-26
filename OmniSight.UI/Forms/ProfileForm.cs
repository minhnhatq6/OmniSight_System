using MaterialSkin.Controls;
using OmniSight.Services;
using System;
using System.Windows.Forms;

namespace OmniSight.UI.Forms
{
    public partial class ProfileForm : MaterialForm
    {
        private readonly IUserService _userService;
        private readonly AuthService _authService; // Để lấy CurrentUser

        // Dùng Dependency Injection để gọi Service
        public ProfileForm(IUserService userService, AuthService authService)
        {
            InitializeComponent();
            _userService = userService;
            _authService = authService;

            // Load theme MaterialSkin (Bạn có thể copy đoạn cấu hình Theme từ Form1 sang đây)
        }

        private void ProfileForm_Load(object sender, EventArgs e)
        {
            var currentUser = _authService.CurrentUser;
            if (currentUser != null)
            {
                // Đổ dữ liệu cũ lên giao diện
                txtFullName.Text = currentUser.FullName;
                txtPhone.Text = currentUser.Phone;
                switchStudent.Checked = currentUser.IsStudent;
                switchTeacher.Checked = currentUser.IsTeacher;
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Họ tên không được để trống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!switchStudent.Checked && !switchTeacher.Checked)
            {
                MessageBox.Show("Bạn phải chọn ít nhất một vai trò!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int userId = _authService.CurrentUser.UserId;

            // Gọi hàm Update
            bool success = await _userService.UpdateProfileAsync(
                userId,
                txtFullName.Text,
                txtPhone.Text,
                switchStudent.Checked,
                switchTeacher.Checked
            );

            if (success)
            {
                // Cập nhật lại session hiện tại trong app
                _authService.CurrentUser.FullName = txtFullName.Text;
                _authService.CurrentUser.Phone = txtPhone.Text;
                _authService.CurrentUser.IsStudent = switchStudent.Checked;
                _authService.CurrentUser.IsTeacher = switchTeacher.Checked;

                MessageBox.Show("Cập nhật hồ sơ thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra khi lưu Database.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}