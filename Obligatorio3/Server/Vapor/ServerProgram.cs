using Common;
using Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public static class ServerProgram
    {

        static readonly ISettingsManager SettingsMgr = new SettingsManager();

        static async Task Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();

            var serverIpAddress = SettingsMgr.ReadSetting(ServerConfig.ServerIpConfigKey);
            var serverPort = SettingsMgr.ReadSetting(ServerConfig.SeverPortConfigKey);
            Console.WriteLine($"Server is starting in address {serverIpAddress} and port {serverPort}");

            Server server = new Server(serverIpAddress, serverPort);
            StartServer(server);
            await Task.Run(() => server.ExitPrompt());
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });


        public static async Task StartServer(Server server)
        {
            Console.WriteLine("Server will start accepting connections from the clients");

            await Task.Run(() => server.StartReceivingConnections());
        }

    }
}
