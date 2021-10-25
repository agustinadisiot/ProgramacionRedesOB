using Common;
using Common.Interfaces;
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
            var serverIpAddress = SettingsMgr.ReadSetting(ServerConfig.ServerIpConfigKey);
            var serverPort = SettingsMgr.ReadSetting(ServerConfig.SeverPortConfigKey);
            Console.WriteLine($"Server is starting in address {serverIpAddress} and port {serverPort}");

            Server server = new Server(serverIpAddress, serverPort);
            await StartServer(server);
            await Task.Run(() => server.ExitPrompt());
        }

        public static async Task StartServer(Server server)
        {
            Console.WriteLine("Server will start accepting connections from the clients");

            await Task.Run(() => server.StartReceivingConnections());
        }

    }
}
