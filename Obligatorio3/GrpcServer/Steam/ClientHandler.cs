using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Server
{
    public class ClientHandler
    {
        private readonly Socket acceptedSocketClient;
        private INetworkStreamHandler networkStreamHandler;
        private bool isClientConnected;

        public ClientHandler(Socket newAcceptedSocketClient)
        {
            acceptedSocketClient = newAcceptedSocketClient;
        }

        public async Task StartHandling()
        {
            isClientConnected = true;
            try
            {
                networkStreamHandler = new NetworkStreamHandler(new NetworkStream(acceptedSocketClient));

                while (isClientConnected)
                {

                    string header = await networkStreamHandler.ReadString(Specification.HEADER_LENGTH);
                    Command cmd = await networkStreamHandler.ReadCommand();
                    if (cmd == Command.EXIT)
                    {
                        isClientConnected = false;
                        Console.WriteLine("Client is leaving");
                    }
                    else
                    {
                        CommandHandler commandHandler = CommandFactory.GetCommandHandler(cmd, networkStreamHandler);
                        await commandHandler.HandleRequest();
                    }
                }
            }
            catch (Exception e) when (e is SocketException || e is System.IO.IOException)
            {
                Console.WriteLine($"The connection was closed");
            }
        }

        public void StopHandling()
        {
            if (isClientConnected)
            {
                isClientConnected = false;
                acceptedSocketClient.Shutdown(SocketShutdown.Both);
                acceptedSocketClient.Close();
            }
        }

    }

}

