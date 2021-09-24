using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;

namespace Client
{
    internal class DeleteGame : TextCommand
    {
        public DeleteGame(INetworkStreamHandler nwsh) : base(nwsh)
        {
        }

        public string SendRequest(int gameId)
        {
            SendHeader();
            SendCommand(Command.DELETE_GAME);

            string data = gameId.ToString();

            SendData(data);
            return ResponseHandler();
        }
        private string ResponseHandler()
        {
            string[] data = GetData();
            string message = data[0];
            return message;

        }
    }
}