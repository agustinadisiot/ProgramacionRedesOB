using Common.FileHandler.Interfaces;
using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;

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
            networkStreamHandler.WriteString(Specification.REQUEST_HEADER);
            networkStreamHandler.WriteCommand(cmd);
        }


        protected void ReadHeader()
        {
            networkStreamHandler.ReadString(Specification.HEADER_LENGTH);
        }

        protected void ReadCommand()
        {
            Command cmd = networkStreamHandler.ReadCommand();
            if (cmd == Command.ERROR)
                HandleError();
            if (cmd == Command.SERVER_SHUTDOWN)
                throw new ServerShutDownException();
        }

        private void HandleError()
        {
            int messageErrorLength = networkStreamHandler.ReadInt(Specification.DATA_SIZE_LENGTH);
            string errorMessage = networkStreamHandler.ReadString(messageErrorLength);
            throw new ServerError(errorMessage);
        }
    }
}
