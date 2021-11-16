using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System.Threading.Tasks;

namespace Client
{
    public class DeleteGame : TextCommand
    {
        public DeleteGame(INetworkStreamHandler nwsh) : base(nwsh)
        {
        }

        public override Command cmd => Command.DELETE_GAME;

        public async Task<string> SendRequest(int gameId)
        {
            await SendHeader();

            string data = gameId.ToString();

            await SendData(data);
            return await ResponseHandler();
        }
        private async Task<string> ResponseHandler()
        {
            string[] data = await GetData();
            string message = data[0];
            return message;

        }
    }
}