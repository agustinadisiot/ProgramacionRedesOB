using Common.FileHandler;
using Common.FileHandler.Interfaces;
using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class ClientHandler
    {
        private readonly TcpClient _acceptedTcpClient;
        private readonly IFileStreamHandler _fileStreamHandler;
        private INetworkStreamHandler networkStreamHandler;
        private bool isClientConnected;
        private bool isServerClosing;

        public ClientHandler(TcpClient newAcceptedTcpClient)
        {
            _acceptedTcpClient = newAcceptedTcpClient;
            isServerClosing = false;
        }

        public void StartHandling()
        {
            isClientConnected = true;
            try
            {
                networkStreamHandler = new NetworkStreamHandler(_acceptedTcpClient.GetStream());

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
            catch (SocketException se)
            {
                Console.WriteLine($"The client connection was interrupted, message {se.Message}");
            }

            if (isServerClosing)
            {
                networkStreamHandler.WriteString(Specification.REQUEST_HEADER);
                networkStreamHandler.WriteCommand(Command.SERVER_SHUTDOWN);
                Console.WriteLine("Client connection was close by the server");
            }
            _acceptedTcpClient.Close();
        }

        public void StopHandling()
        {
            if (isClientConnected)
            {
                isServerClosing = true;
                isClientConnected = false;
            }
        }

    }

}

