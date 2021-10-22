using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

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

        public async void StartReceivingConnections()
        {
            server.Listen(maxClientsInQ);

            while (acceptingConnections)
            {
                try
                {
                    Socket acceptedClient = await server.AcceptAsync().ConfigureAwait(false);
                    var task = Task.Run(async()=> await handleAcceptedConection(acceptedClient));
                }
                catch (SocketException e)
                {
                    Console.WriteLine("Server no longer accept request");
                    acceptingConnections = false;
                }
               
            }


        }

        public async Task handleAcceptedConection(Socket acceptedSocket)
        {
            if (acceptingConnections)
            {
                Console.WriteLine("Accepted new client connection");
                ClientHandler newHandler = new ClientHandler(acceptedSocket);
                clientHandlers.Add(newHandler);
                await Task.Run(() => newHandler.StartHandling());
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
            Console.WriteLine("Press enter to close window");
        }
    }


}
