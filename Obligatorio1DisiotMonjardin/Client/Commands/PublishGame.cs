using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;

namespace Client.Commands
{
    public class PublishGame : TextCommand
    {
        public PublishGame(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.PUBLISH_GAME;

        public string SendRequest(Game newGame)
        {
            SendHeader();

            string data = "";
            data += newGame.Title;
            data += Specification.FIRST_DELIMITER;
            data += newGame.Synopsis;
            data += Specification.FIRST_DELIMITER;
            data += (int)newGame.ESRBRating;
            data += Specification.FIRST_DELIMITER;
            data += newGame.Genre;

            SendData(data);
            fileNetworkStreamHandler.SendFile(newGame.CoverFilePath);
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
