using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Win32;
using OmniSight.Core.Entities;
using OmniSight.Data;
using OmniSight.Services;
using OmniSight.UI.Forms;
using OmniSight.UI.Forms.Auth;
using System.Web; // Lưu ý: Nếu báo lỗi ở đây, hãy xem hướng dẫn phía dưới

namespace OmniSight.UI
{
    internal static class Program
    {
        public static IServiceProvider? ServiceProvider { get; private set; }

        [STAThread]
        static async Task Main(string[] args) // Đã thêm string[] args
        {
            // 1. Đăng ký Protocol với Windows (để nhận link omnisight://)
            RegisterCustomProtocol();

            ApplicationConfiguration.Initialize();

            // 2. Khởi tạo Host và Services
            var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<OmniSightDbContext>(options =>
                        options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")));

                    // Đăng ký các Service
                    services.AddScoped<AuthService>();
                    services.AddScoped<IUserService, UserService>();
                    services.AddScoped<ClassService>();
                    services.AddScoped<StreamService>();
                    // THÊM DÒNG NÀY ĐỂ FIX LỖI CRASH:
                    services.AddScoped<FaceAiService>();
                    // Đăng ký các Form
                    services.AddTransient<FrmLogin>();
                    services.AddTransient<MainForm>();
                    services.AddTransient<FrmRegister>();
                    services.AddTransient<FrmSetPassword>();
                    // Trong Program.cs -> ConfigureServices
                    services.AddTransient<FrmFaceLogin>();
                })
                .Build();

            ServiceProvider = host.Services;
            var authService = ServiceProvider.GetRequiredService<AuthService>();

            // 3. XỬ LÝ NẾU MỞ TỪ LINK (DEEP LINKING)
            if (args.Length > 0 && args[0].StartsWith("omnisight://"))
            {
                await HandleDeepLink(args[0], authService);
            }

            // 4. LOGIC KHỞI ĐỘNG BÌNH THƯỜNG
            bool isLoggedIn = await authService.TryAutoLoginAsync();

            if (isLoggedIn)
            {
                Application.Run(ServiceProvider.GetRequiredService<MainForm>());
            }
            else
            {
                Application.Run(ServiceProvider.GetRequiredService<FrmLogin>());
            }
        }

        // Hàm xử lý bóc tách link và xác thực
        private static async Task HandleDeepLink(string url, AuthService authService)
        {
            try
            {
                // Link: omnisight://verify-email?token=abc&email=test@gmail.com
                var uri = new Uri(url.Replace("omnisight://", "http://"));
                var query = HttpUtility.ParseQueryString(uri.Query);

                string? token = query["token"];
                string? email = query["email"];
                string path = uri.Host; // Host ở đây chính là "verify-email"

                if (path == "verify-email" && !string.IsNullOrEmpty(token))
                {
                    var result = await authService.VerifyTokenAsync(token, TokenType.EmailVerification);
                    if (result.success)
                    {
                        MessageBox.Show("Xác thực Email thành công! Chào mừng bạn đến với OmniSight.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Mã xác thực không hợp lệ hoặc đã hết hạn.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xử lý link: " + ex.Message);
            }
        }

        // Hàm đăng ký link "omnisight://" vào Registry Windows
        private static void RegisterCustomProtocol()
        {
            try
            {
                string appPath = Application.ExecutablePath;
                using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\omnisight"))
                {
                    key.SetValue("", "URL:OmniSight Protocol");
                    key.SetValue("URL Protocol", "");
                    using (var commandKey = key.CreateSubKey(@"shell\open\command"))
                    {
                        commandKey.SetValue("", $"\"{appPath}\" \"%1\"");
                    }
                }
            }
            catch { }
        }
    }
}