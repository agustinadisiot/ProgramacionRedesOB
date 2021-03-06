using Common;
using Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace Server
{
    public class Program
    {
        static readonly ISettingsManager SettingsMgr = new SettingsManager();

        public static async Task Main(string[] args)
        {
            var serverIpAddress = SettingsMgr.ReadSetting(ServerConfig.ServerIpConfigKey);
            var serverPort = SettingsMgr.ReadSetting(ServerConfig.SeverPortConfigKey);
            Console.WriteLine($"Server is starting in address {serverIpAddress} and port {serverPort}");

            Server server = new Server(serverIpAddress, serverPort);
            StartServer(server);
            Logger.CreateChannel();
            CreateHostBuilder(args).Build().RunAsync();
            await Task.Run(() => server.ExitPrompt());
            Logger.Log(new LogRecord { Severity = LogRecord.InfoSeverity, Message = "Se apag? el servidor" });
        }

        public static async Task StartServer(Server server)
        {
            Console.WriteLine("Server will start accepting connections from the clients");
            Logger.Log(new LogRecord { Severity = LogRecord.InfoSeverity, Message = "Se prendi? el servidor" });
            await Task.Run(() => server.StartReceivingConnections());
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
