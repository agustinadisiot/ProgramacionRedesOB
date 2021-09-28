using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    public class Server
    {
        private readonly TcpListener tcpListener;
        public const int maxClientsInQ = 100;
        public bool acceptingConnections;
        public List<ClientHandler> clientHandlers;
        public Server(string serverIpAddress, string serverPort)
        {

            clientHandlers = new List<ClientHandler>();
            var ipEndPoint = new IPEndPoint(IPAddress.Parse(serverIpAddress), int.Parse(serverPort));
            tcpListener = new TcpListener(ipEndPoint);
            acceptingConnections = true;
        }

        public void StartReceivingConnections()
        {
            tcpListener.Start(maxClientsInQ);

            while (acceptingConnections)
            {
                var acceptedTcpClient = tcpListener.AcceptTcpClient(); // Gets the first client in the queue
                if (acceptingConnections)
                {
                    Console.WriteLine("Accepted new client connection");
                    ClientHandler newHandler = new ClientHandler(acceptedTcpClient);
                    clientHandlers.Add(newHandler);
                    Thread clientThread = new Thread(() => newHandler.StartHandling());
                    clientThread.Start();
                }
            }


        }

        public void ExitPrompt()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Write exit to shutdown server");
                string entry = Console.ReadLine();
                if (entry == "exit")
                {
                    exit = true;
                }
            }
            acceptingConnections = false;

            Console.WriteLine("Server closing");
            foreach (ClientHandler clientHandler in clientHandlers)
            {
                clientHandler.StopHandling();
            }
        }
    }


}
