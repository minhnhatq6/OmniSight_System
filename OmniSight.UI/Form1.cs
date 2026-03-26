using MaterialSkin;
using MaterialSkin.Controls;
using Microsoft.Extensions.DependencyInjection;
using OmniSight.Services;
using OmniSight.UI.Forms; // Thêm dòng này để gọi được ProfileForm
using System;
using System.Windows.Forms;

namespace OmniSight.UI
{
    public partial class Form1 : MaterialForm
    {
        private readonly AuthService _authService;
        private readonly IServiceProvider _serviceProvider; // Dùng cái này để gọi form khác

        // Thêm IServiceProvider vào Constructor
        public Form1(AuthService authService, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _authService = authService;
            _serviceProvider = serviceProvider;

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Blue600, Primary.Blue700,
                Primary.Blue200, Accent.Amber200,
                TextShade.WHITE
            );
        }

        // Sự kiện khi bấm nút ĐĂNG NHẬP VỚI GOOGLE
        private async void btnLoginGoogle_Click(object sender, EventArgs e)
        {
            // (Tùy chọn) Đổi chữ trên nút để báo cho người dùng biết đang xử lý
            var btn = sender as MaterialButton;
            if (btn != null) btn.Text = "ĐANG ĐĂNG NHẬP...";

            // Gọi hàm đăng nhập
            bool success = await _authService.LoginWithGoogleAsync();

            if (success)
            {
                // 1. Tạo và mở ProfileForm thông qua DI
                var profileForm = _serviceProvider.GetRequiredService<ProfileForm>();
                profileForm.Show();

                // 2. Ẩn form đăng nhập này đi
                this.Hide();
            }
            else
            {
                MessageBox.Show("Đăng nhập thất bại hoặc bị hủy!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (btn != null) btn.Text = "ĐĂNG NHẬP VỚI GOOGLE";
            }
        }
    }
}