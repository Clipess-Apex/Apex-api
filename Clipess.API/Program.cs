using Microsoft.AspNetCore.Hosting;
using QuestPDF.Infrastructure;
using System.Reflection;

namespace Clipess.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Build and run the host
            QuestPDF.Settings.License = LicenseType.Community;
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });

    }
}