using Common.FileHandler;
using Common.FileHandler.Interfaces;
using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class ClientHandler // TODO no se si se deberia llamar asi
    {
        private readonly TcpClient _tcpClient;
        private readonly IFileStreamHandler _fileStreamHandler;
        private INetworkStreamHandler _networkStreamHandler;

        public ClientHandler()
        {
            _tcpClient = new TcpClient(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6001)); // TODO cambiar esto 
            _fileStreamHandler = new FileStreamHandler();
        }

        public void StartClient()
        {
            _tcpClient.Connect(IPAddress.Parse("127.0.0.1"), 6000);
            _networkStreamHandler = new NetworkStreamHandler(_tcpClient.GetStream());
        }

    }
}
