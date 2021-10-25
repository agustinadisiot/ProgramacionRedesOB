using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System.Threading.Tasks;

namespace Client
{
    public class ModifyGame : TextCommand

    {
        public ModifyGame(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.MODIFY_GAME;

        public async Task<string> SendRequest(int gameId, Game gameToMod)
        {
            await SendHeader();

            string data = "";
            data += gameId.ToString();
            data += Specification.FIRST_DELIMITER;
            data += gameToMod.Title;
            data += Specification.FIRST_DELIMITER;
            data += gameToMod.Synopsis;
            data += Specification.FIRST_DELIMITER;
            data += (int)gameToMod.ESRBRating;
            data += Specification.FIRST_DELIMITER;
            data += gameToMod.Genre;
            data += Specification.FIRST_DELIMITER;

            if (gameToMod.CoverFilePath == "")
            {
                data += 0;
                await SendData(data);
            }
            else
            {
                data += 1;
                await SendData(data);
                await fileNetworkStreamHandler.SendFile(gameToMod.CoverFilePath);
            }

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