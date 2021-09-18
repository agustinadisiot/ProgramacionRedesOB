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
        protected INetworkStreamHandler networkStreamHandler;
        public CommandHandler(INetworkStreamHandler nwsh)
        {
            networkStreamHandler = nwsh;
        }

        protected void SendHeader()
        {
            networkStreamHandler.WriteString(Specification.responseHeader);
        }

        protected void SendCommand(Command cmd)
        {
            networkStreamHandler.WriteCommand(cmd);
        }

        protected void ReadHeader()
        {
            networkStreamHandler.ReadString(Specification.HeaderLength);
        }

        protected void ReadCommand()
        {
            networkStreamHandler.ReadCommand();
        }
    }
}
