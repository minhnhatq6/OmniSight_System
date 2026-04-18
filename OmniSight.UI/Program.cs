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
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace OmniSight.UI
{
    internal static class Program
    {
        public static IServiceProvider? ServiceProvider { get; private set; }

        [STAThread]
        static void Main(string[] args)
        {
            // 1. Đăng ký Protocol với Windows
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
                    services.AddScoped<AssignmentService>();
                    services.AddScoped<ExamService>();
                    services.AddSingleton<FaceAiService>();

                    // Đăng ký các Form
                    services.AddTransient<FrmLogin>();
                    services.AddTransient<MainForm>();
                    services.AddTransient<FrmRegister>();
                    services.AddTransient<FrmSetPassword>();
                    services.AddTransient<FrmFaceLogin>();
                    services.AddScoped<AntiCheatService>();
                })
                .Build();

            ServiceProvider = host.Services;

            // --- SEED DATA ---
            using (var scope = ServiceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<OmniSightDbContext>();
                if (!db.Subjects.Any())
                {
                    db.Subjects.AddRange(
                        new Subject { SubjectName = "Toán Học" },
                        new Subject { SubjectName = "Vật Lý" },
                        new Subject { SubjectName = "Hóa Học" },
                        new Subject { SubjectName = "Tiếng Anh" },
                        new Subject { SubjectName = "Tin Học" }
                    );
                    db.SaveChanges();
                }
            }

            var authService = ServiceProvider.GetRequiredService<AuthService>();
            bool isDeepLinkLogin = false;

            // 3. XỬ LÝ NẾU MỞ TỪ LINK (DEEP LINKING)
            if (args.Length > 0 && args[0].StartsWith("omnisight://"))
            {
                // Trả về true nếu xác thực mail và đăng nhập thành công
                isDeepLinkLogin = HandleDeepLink(args[0], authService).GetAwaiter().GetResult();
            }

            // 4. LOGIC KHỞI ĐỘNG
            if (isDeepLinkLogin)
            {
                // NẾU VÀO TỪ MAIL: Mở thẳng MainForm và yêu cầu nhảy vào Profile
                var mainForm = ServiceProvider.GetRequiredService<MainForm>();
                mainForm.StartAtProfile = true; // Gán cờ để MainForm biết cần mở tab Profile
                Application.Run(mainForm);
            }
            else
            {
                // LOGIC KHỞI ĐỘNG BÌNH THƯỜNG
                bool isLoggedIn = authService.TryAutoLoginAsync().GetAwaiter().GetResult();
                if (isLoggedIn)
                {
                    Application.Run(ServiceProvider.GetRequiredService<MainForm>());
                }
                else
                {
                    Application.Run(ServiceProvider.GetRequiredService<FrmLogin>());
                }
            }
        }

        // Hàm xử lý bóc tách link và thực hiện đăng nhập tự động
        private static async Task<bool> HandleDeepLink(string url, AuthService authService)
        {
            try
            {
                // Chuẩn hóa link để dùng Uri parser (Thay omnisight:// bằng http://)
                var uriString = url.Replace("omnisight://", "http://");
                var uri = new Uri(uriString);
                var query = HttpUtility.ParseQueryString(uri.Query);

                string? token = query["token"];
                string path = uri.Host; // Sẽ là "verify" hoặc "verify-email" tùy link bạn gửi

                if (!string.IsNullOrEmpty(token))
                {
                    // Gọi hàm xác thực + tự gán CurrentUser trong AuthService
                    // Lưu ý: Đảm bảo bạn đã viết hàm VerifyEmailAndLoginAsync trong AuthService
                    var result = await authService.VerifyEmailAndLoginAsync(token);

                    if (result.success)
                    {
                        MessageBox.Show("Xác thực Email thành công! Chào mừng bạn quay trở lại.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return true; // Báo cho Main biết để mở MainForm luôn
                    }
                    else
                    {
                        MessageBox.Show("Mã xác thực không hợp lệ hoặc đã hết hạn.", "Lỗi xác thực", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi xử lý Deep Link: " + ex.Message);
            }
            return false;
        }

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