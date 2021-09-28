using Common.NetworkUtils.Interfaces;
using Common.Protocol;

namespace Client
{
    public class DeleteGame : TextCommand
    {
        public DeleteGame(INetworkStreamHandler nwsh) : base(nwsh)
        {
        }

        public override Command cmd => Command.DELETE_GAME;

        public string SendRequest(int gameId)
        {
            SendHeader();

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