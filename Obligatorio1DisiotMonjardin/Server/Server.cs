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
        public bool acceptingConnections;
        public List<TcpClient> tcpClients;
        public Server(string serverIpAddress, string serverPort) {

            tcpClients = new List<TcpClient>();
            var ipEndPoint = new IPEndPoint(IPAddress.Parse(serverIpAddress), int.Parse(serverPort));
            tcpListener = new TcpListener(ipEndPoint);
            acceptingConnections = true;
        }

        public  void StartReceivingConnections()
        {
            tcpListener.Start(maxClientsInQ);

            while (acceptingConnections) // This while (true) should only be valid for examples TODO SACAR
            {
                var acceptedTcpClient = tcpListener.AcceptTcpClient(); // Gets the first client in the queue
                tcpClients.Add(acceptedTcpClient);
                Console.WriteLine("Accepted new client connection");
                Thread clientThread = new Thread(() => new ClientHandler(acceptedTcpClient).StartHandling());
                clientThread.Start();
            }
            Console.WriteLine("Server closing");
            foreach(TcpClient tcpclient in tcpClients)
            {
                tcpclient.Close();
            }
        }
    }


}
