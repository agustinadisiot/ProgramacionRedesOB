using Common;
using Common.Protocol;
using Common.Interfaces;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    public static class ServerProgram
    {

        static readonly ISettingsManager SettingsMgr = new SettingsManager();

        static void Main(string[] args)
        {
            //var serverIpAddress = SettingsMgr.ReadSetting(ServerConfig.ServerIpConfigKey);// TODO
            var serverIpAddress = "127.0.0.1";
            //var serverPort = SettingsMgr.ReadSetting(ServerConfig.SeverPortConfigKey);
            var serverPort = "6000";
            Console.WriteLine($"Server is starting in address {serverIpAddress} and port {serverPort}");

            Server server = new Server(serverIpAddress, serverPort);
            Console.WriteLine("Server will start accepting connections from the clients");

            Thread clientConnectionsThread = new Thread(() => server.StartReceivingConnections());
            clientConnectionsThread.Start();

            Thread serverExitPrompt = new Thread(() => server.ExitPrompt());
            serverExitPrompt.Start();

        }


    }
}
