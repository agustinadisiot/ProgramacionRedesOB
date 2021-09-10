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
        private INetworkStreamHandler _networkStreamHandler;

        public ClientHandler(TcpClient newAcceptedTcpClient)
        {
            _acceptedTcpClient = newAcceptedTcpClient;
        }

        public void StartHandling()
        {
            var isClientConnected = true;
            try
            {
                _networkStreamHandler = new NetworkStreamHandler(_acceptedTcpClient.GetStream());

                while (isClientConnected)
                {

                    byte[] header = _networkStreamHandler.Read(Specification.HeaderLength);
                    string parsedHeader = Encoding.UTF8.GetString(header);

                    byte[] cmd = _networkStreamHandler.Read(Specification.CmdLength);
                    Command parsedCmd = (Command)BitConverter.ToUInt16(cmd);

                    CommandHandler commandHandler = CommandFactory.GetCommandHandler(parsedCmd, _networkStreamHandler);

                    Console.WriteLine("Client says (header): " + parsedHeader);
                    Console.WriteLine("Client says (CMD):" + parsedCmd);

                    commandHandler.HandleRequest();


                  /*  var word = parsedData; 
                    if (word.Equals("exit"))
                    {
                        isClientConnected = false;
                        Console.WriteLine("Client is leaving");
                    }
                    else
                    {
                       
                        Console.WriteLine("Client says (length): " + parsedLength);
                        Console.WriteLine("Client says (data): " + parsedData);

                    }*/
                }
            }
            catch (SocketException se)
            {
                Console.WriteLine($"The client connection was interrupted, message {se.Message}");
            }
        }

    }

}

