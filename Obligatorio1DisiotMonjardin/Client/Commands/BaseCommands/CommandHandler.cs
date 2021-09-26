using Common.FileHandler.Interfaces;
using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;

namespace Client
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

        protected void SendHeader()
        {
            networkStreamHandler.WriteString(Specification.requestHeader);
            networkStreamHandler.WriteCommand(cmd);
        }


        protected void ReadHeader()
        {
            networkStreamHandler.ReadString(Specification.HeaderLength);
        }

        protected void ReadCommand()
        {
            Command cmd = networkStreamHandler.ReadCommand();
            if (cmd == Command.ERROR)
                HandleError();
        }

        private void HandleError()
        {
            int messageErrorLength = networkStreamHandler.ReadInt(Specification.dataSizeLength);
            string errorMessage = networkStreamHandler.ReadString(messageErrorLength);
            throw new ServerError(errorMessage);
        }
    }
}
