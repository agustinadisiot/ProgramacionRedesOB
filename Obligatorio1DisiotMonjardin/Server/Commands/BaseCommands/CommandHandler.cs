using Common.FileHandler.Interfaces;
using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public abstract class CommandHandler
    {
        protected INetworkStreamHandler networkStreamHandler;
        protected IFileNetworkStreamHandler fileNetworkStreamHandler;
        public abstract Command cmd { get; }
        public CommandHandler(INetworkStreamHandler nwsh)
        {
            networkStreamHandler = nwsh;
            fileNetworkStreamHandler = new FileNetworkStreamHandler(nwsh);
        }
        public abstract void HandleRequest();

        protected void SendResponseHeader()
        {
            networkStreamHandler.WriteString(Specification.RESPONSE_HEADER);
            networkStreamHandler.WriteCommand(cmd);
        }

        protected void ReadHeader()
        {
            networkStreamHandler.ReadString(Specification.HEADER_LENGTH);
            networkStreamHandler.ReadCommand();
        }

        protected void SendData(string data)
        {
            int dataLengthInBytes = Encoding.UTF8.GetBytes(data).Length;
            networkStreamHandler.WriteInt(dataLengthInBytes);
            networkStreamHandler.WriteString(data);
        }
    }
}
