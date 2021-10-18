using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    public class Server
    {
        private readonly Socket server;
        public const int maxClientsInQ = 100;
        public bool acceptingConnections;
        public List<ClientHandler> clientHandlers;
        public Server(string serverIpAddress, string serverPort)
        {

            clientHandlers = new List<ClientHandler>();
            var ipEndPoint = new IPEndPoint(IPAddress.Parse(serverIpAddress), int.Parse(serverPort));
            server = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, 
                ProtocolType.Tcp);
            server.Bind(ipEndPoint);
            acceptingConnections = true;
        }

        public void StartReceivingConnections()
        {
            server.Listen(maxClientsInQ);

            while (acceptingConnections)
            {
                Socket acceptedClient = null;
                try
                {
                    acceptedClient = server.Accept();
                }
                catch (SocketException e)
                {
                    Console.WriteLine("Server no longer accept request");
                    acceptingConnections = false;
                }
                if (acceptingConnections)
                {
                    Console.WriteLine("Accepted new client connection");
                    ClientHandler newHandler = new ClientHandler(acceptedClient);
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

            Console.WriteLine("Server closing");
            foreach (ClientHandler clientHandler in clientHandlers)
            {
                clientHandler.StopHandling();
            }
            acceptingConnections = false;
            server.Close();
        }
    }


}
