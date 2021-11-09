using Common.FileHandler.Interfaces;
using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System.Text;
using System.Threading.Tasks;

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
        public abstract Task HandleRequest();

        protected async Task SendResponseHeader()
        {
            await networkStreamHandler.WriteString(Specification.RESPONSE_HEADER);
            await networkStreamHandler.WriteCommand(cmd);
        }

        protected async Task ReadHeader()
        {
            await networkStreamHandler .ReadString(Specification.HEADER_LENGTH);
            await networkStreamHandler.ReadCommand();
        }

        protected async Task SendData(string data)
        {
            int dataLengthInBytes = Encoding.UTF8.GetBytes(data).Length;
            await networkStreamHandler.WriteInt(dataLengthInBytes);
            await networkStreamHandler .WriteString(data);
        }
    }
}
