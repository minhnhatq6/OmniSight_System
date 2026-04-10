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

        // BẮT BUỘC LÀ "void Main" ĐỂ APP KHÔNG BỊ ĐỨNG KHI MỞ HỘP THOẠI CHỌN FILE
        [STAThread]
        static void Main(string[] args)
        {
            // 1. Đăng ký Protocol với Windows
            RegisterCustomProtocol();
            ApplicationConfiguration.Initialize();

            // 2. Khởi tạo Host và Services (Giữ nguyên toàn bộ cấu hình của bạn)
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

                    // Face AI Service (Khuyên dùng Singleton để camera không bị đụng độ giữa các form)
                    services.AddSingleton<FaceAiService>();

                    // Đăng ký các Form
                    services.AddTransient<FrmLogin>();
                    services.AddTransient<MainForm>();
                    services.AddTransient<FrmRegister>();
                    services.AddTransient<FrmSetPassword>();
                    services.AddTransient<FrmFaceLogin>(); // <-- Dòng này giúp Face Login của bạn hoạt động
                })
                .Build();

            ServiceProvider = host.Services;

            // --- SEED DATA (KHỞI TẠO MÔN HỌC MẪU NẾU TRỐNG) ---
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

            // 3. XỬ LÝ NẾU MỞ TỪ LINK (DEEP LINKING) - Chạy đồng bộ để giữ luồng UI
            if (args.Length > 0 && args[0].StartsWith("omnisight://"))
            {
                HandleDeepLink(args[0], authService).GetAwaiter().GetResult();
            }

            // 4. LOGIC KHỞI ĐỘNG BÌNH THƯỜNG - Chạy đồng bộ để giữ luồng UI
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

        // Hàm xử lý bóc tách link và xác thực
        private static async Task HandleDeepLink(string url, AuthService authService)
        {
            try
            {
                var uri = new Uri(url.Replace("omnisight://", "http://"));
                var query = HttpUtility.ParseQueryString(uri.Query);

                string? token = query["token"];
                string? email = query["email"];
                string path = uri.Host;

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