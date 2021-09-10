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
            startServer();

        }

        private static void startServer()
        {
            //var serverIpAddress = SettingsMgr.ReadSetting(ServerConfig.ServerIpConfigKey); TODO
            var serverIpAddress = "127.0.0.1";
            //var serverPort = SettingsMgr.ReadSetting(ServerConfig.SeverPortConfigKey);
            var serverPort = "6000";

            Console.WriteLine($"Server is starting in address {serverIpAddress} and port {serverPort}");

            var ipEndPoint = new IPEndPoint(IPAddress.Parse(serverIpAddress), int.Parse(serverPort));
            var tcpListener = new TcpListener(ipEndPoint);

            int maxClientsInQ = 100;
            tcpListener.Start(maxClientsInQ);
            Console.WriteLine("Server will start accepting connections from the clients");

            while (true) // This while (true) should only be valid for examples TODO SACAR
            {
                var acceptedTcpClient = tcpListener.AcceptTcpClient(); // Gets the first client in the queue
                Console.WriteLine("Accepted new client connection");
                Thread clientThread = new Thread( () => new ClientHandler(acceptedTcpClient).StartHandling()) ;
                clientThread.Start();
            }
        }
    }
}
