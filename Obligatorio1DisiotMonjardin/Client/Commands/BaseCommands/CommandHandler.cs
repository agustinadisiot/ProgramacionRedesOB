using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    public abstract class CommandHandler
    {
        protected INetworkStreamHandler _networkStreamHandler;
        public CommandHandler(INetworkStreamHandler nwsh)
        {
            _networkStreamHandler = nwsh;
        }

        protected void SendHeader()
        {
            _networkStreamHandler.WriteString(Specification.responseHeader);
        }

        protected void SendCommand(Command cmd)
        {
            _networkStreamHandler.WriteCommand(cmd);
        }

        protected void ReadHeader()
        {
            _networkStreamHandler.ReadString(Specification.HeaderLength);
        }

        protected void ReadCommand()
        {
            _networkStreamHandler.ReadCommand();
        }
    }
}
