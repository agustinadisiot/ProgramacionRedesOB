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
    public class Program
    {

        static readonly ISettingsManager SettingsMgr = new SettingsManager();

        static void Main(string[] args)
        {
            startServer();

        }

        private static void startServer()
        {
            var serverIpAddress = SettingsMgr.ReadSetting(ServerConfig.ServerIpConfigKey);
            var serverPort = SettingsMgr.ReadSetting(ServerConfig.SeverPortConfigKey);

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
                new Thread(() => HandleClient(acceptedTcpClient)).Start();
            }
        }


        private static void HandleClient(TcpClient acceptedTcpClient)
        {
            var isClientConnected = true;
            try
            {
                var networkStream = acceptedTcpClient.GetStream();

                while (isClientConnected)
                {

                    byte[] header = ReadNBytes(networkStream, Specification.HeaderLength);
                    string parsedHeader = Encoding.UTF8.GetString(header);

                    byte[] cmd = ReadNBytes(networkStream, Specification.CmdLength);
                    int parsedCmd = BitConverter.ToUInt16(cmd);

                    byte[] dataLength = ReadNBytes(networkStream, Specification.dataSizeLength);
                    int parsedLength = BitConverter.ToInt32(dataLength);

                    byte[] data = ReadNBytes(networkStream, parsedLength);
                    string parsedData = Encoding.UTF8.GetString(data);

                    var word = parsedData;
                    if (word.Equals("exit"))
                    {
                        isClientConnected = false;
                        Console.WriteLine("Client is leaving");
                    }
                    else
                    {
                        Console.WriteLine("Client says: " + word);
                    }
                }
            }
            catch (SocketException se)
            {
                Console.WriteLine($"The client connection was interrupted, message {se.Message}");
            }
        }

        private static byte[] ReadNBytes(NetworkStream networkStream, int dataLength)
        {
            var dataRecieved = new byte[dataLength];
            var totalReceived = 0;
            while (totalReceived < dataLength)
            {
                var received = networkStream.Read(dataRecieved, totalReceived, dataLength - totalReceived);
                if (received == 0) // if receive 0 bytes this means that connection was interrupted between the two points
                {
                    throw new SocketException();
                }
                totalReceived += received;
            }

            return dataRecieved;
        }
    }
}

