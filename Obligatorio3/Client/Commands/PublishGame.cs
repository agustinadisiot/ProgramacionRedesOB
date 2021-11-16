using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System.Threading.Tasks;

namespace Client.Commands
{
    public class PublishGame : TextCommand
    {
        public PublishGame(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.PUBLISH_GAME;

        public async Task<string> SendRequest(Game newGame)
        {
            await SendHeader();

            string data = "";
            data += newGame.Title;
            data += Specification.FIRST_DELIMITER;
            data += newGame.Synopsis;
            data += Specification.FIRST_DELIMITER;
            data += (int)newGame.ESRBRating;
            data += Specification.FIRST_DELIMITER;
            data += newGame.Genre;

            await SendData(data);
            await fileNetworkStreamHandler.SendFile(newGame.CoverFilePath);
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
