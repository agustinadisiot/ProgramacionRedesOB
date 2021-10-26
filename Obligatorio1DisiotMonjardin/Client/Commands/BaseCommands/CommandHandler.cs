using Common.FileHandler.Interfaces;
using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System.Threading.Tasks;

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

        protected async Task SendHeader()
        {
            try
            {
                await networkStreamHandler.WriteString(Specification.REQUEST_HEADER);
                await networkStreamHandler.WriteCommand(cmd);
            }
            catch (System.IO.IOException)
            {
                throw new ServerShutDownException();
            }
        }


        protected async Task ReadHeader()
        {
            await networkStreamHandler.ReadString(Specification.HEADER_LENGTH);
        }

        protected async Task ReadCommand()
        {
            Command readCommand = await networkStreamHandler.ReadCommand();
            if (readCommand == Command.ERROR)
                await HandleError();
        }

        private async Task HandleError()
        {
            int messageErrorLength = await networkStreamHandler.ReadInt(Specification.DATA_SIZE_LENGTH);
            string errorMessage = await networkStreamHandler.ReadString(messageErrorLength);
            throw new ServerError(errorMessage);
        }
    }
}
