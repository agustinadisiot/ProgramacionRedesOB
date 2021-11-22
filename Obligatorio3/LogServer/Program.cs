using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
namespace LogServer
{
    public class Program
    {
        private static LogsServerLogic logsClient;
        public static void Main(string[] args)
        {
            logsClient = new LogsServerLogic();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
