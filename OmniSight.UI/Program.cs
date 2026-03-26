
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using OmniSight.Data;
using Microsoft.EntityFrameworkCore;
using OmniSight.Services;
using OmniSight.UI.Forms.Auth;

namespace OmniSight.UI
{
    internal static class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<OmniSightDbContext>(options =>
                        options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")));

                    services.AddScoped<AuthService>();
                    services.AddTransient<FrmLogin>();
                })
                .Build();

            ServiceProvider = host.Services;

            Application.Run(ServiceProvider.GetRequiredService<FrmLogin>());
        }
    }
}
