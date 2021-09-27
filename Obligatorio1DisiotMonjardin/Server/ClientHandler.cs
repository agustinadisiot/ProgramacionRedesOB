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

        public ClientHandler(TcpClient newAcceptedTcpClient)
        {
            _acceptedTcpClient = newAcceptedTcpClient;
        }

        public void StartHandling()
        {
            var isClientConnected = true;
            try
            {
                networkStreamHandler = new NetworkStreamHandler(_acceptedTcpClient.GetStream());

                while (isClientConnected)
                {

                    string header = networkStreamHandler.ReadString(Specification.HEADER_LENGTH);
                    Command cmd = networkStreamHandler.ReadCommand();

                    CommandHandler commandHandler = CommandFactory.GetCommandHandler(cmd, networkStreamHandler);

                    //Console.WriteLine("Client says (header): " + header); TODO borrar
                    //Console.WriteLine("Client says (CMD):" + cmd);

                    commandHandler.HandleRequest();
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("Client is leaving");
            }
        }

    }

}

