using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Net.Sockets;

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

        public void StartHandling()
        {
            isClientConnected = true;
            try
            {
                networkStreamHandler = new NetworkStreamHandler(new NetworkStream(acceptedSocketClient));

                while (isClientConnected)
                {

                    string header = networkStreamHandler.ReadString(Specification.HEADER_LENGTH);
                    Command cmd = networkStreamHandler.ReadCommand();
                    if (cmd == Command.EXIT)
                    {
                        isClientConnected = false;
                        Console.WriteLine("Client is leaving");
                    }
                    else
                    {
                        CommandHandler commandHandler = CommandFactory.GetCommandHandler(cmd, networkStreamHandler);
                        commandHandler.HandleRequest();
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

