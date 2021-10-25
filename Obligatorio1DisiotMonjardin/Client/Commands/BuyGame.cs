using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System.Threading.Tasks;

namespace Client.Commands
{
    public class BuyGame : TextCommand
    {
        public BuyGame(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.BUY_GAME;

        public async Task<string> SendRequest(int gameId)
        {
            await SendHeader();

            string data = "";
            data += gameId;

            await SendData(data);
            return await ResponseHandler ();
        }


        private async Task<string> ResponseHandler()
        {
            string[] data = await GetData();
            string message = data[0];
            return message;

        }
    }
}
