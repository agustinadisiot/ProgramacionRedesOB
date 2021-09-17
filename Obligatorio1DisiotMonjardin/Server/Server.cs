using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    public class Server
    {
        private readonly TcpListener tcpListener;
        public const int  maxClientsInQ = 100;
        public Server(string serverIpAddress, string serverPort) {
            
            var ipEndPoint = new IPEndPoint(IPAddress.Parse(serverIpAddress), int.Parse(serverPort));
            tcpListener = new TcpListener(ipEndPoint);

        }

        public  void StartReceivingConnections()
        {
            tcpListener.Start(maxClientsInQ);

            while (true) // This while (true) should only be valid for examples TODO SACAR
            {
                var acceptedTcpClient = tcpListener.AcceptTcpClient(); // Gets the first client in the queue
                Console.WriteLine("Accepted new client connection");
                Thread clientThread = new Thread(() => new ClientHandler(acceptedTcpClient).StartHandling());
                clientThread.Start();
            }
        }
    }


}
