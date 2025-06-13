using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Werehouse.Data;
using Werehouse.Services;

namespace Werehouse
{
    internal static class Program
    {

        [STAThread]
        static void Main()
        {
            // Create the host builder
            var host = CreateHostBuilder().Build();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var mainForm = host.Services.GetRequiredService<FileUploaderForm>();
            Application.Run(mainForm);
        }

        static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<FileUploaderForm>();

                    services.AddTransient<ExcelOrderProcessorService>();
                    services.AddTransient<OrdersAdaptorService>();
                    services.AddTransient<StatusProvider>();
                });
        }
    }
}