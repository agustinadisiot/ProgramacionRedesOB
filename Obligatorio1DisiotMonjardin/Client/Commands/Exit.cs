using Common.NetworkUtils.Interfaces;
using Common.Protocol;

namespace Client
{
    internal class Exit : CommandHandler
    {
        public Exit(INetworkStreamHandler nwsh) : base(nwsh)
        {
        }

        public override Command cmd => Command.EXIT;

        public void SendRequest()
        {
            SendHeader();
        }
    }
}