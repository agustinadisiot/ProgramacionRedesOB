using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System.Threading.Tasks;

namespace Client
{
    internal class Exit : CommandHandler
    {
        public Exit(INetworkStreamHandler nwsh) : base(nwsh)
        {
        }

        public override Command cmd => Command.EXIT;

        public async Task SendRequest()
        {
            await SendHeader();
        }
    }
}