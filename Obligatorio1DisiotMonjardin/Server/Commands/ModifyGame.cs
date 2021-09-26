using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System.IO;

namespace Server
{
    public class ModifyGame : TextCommand
    {
        public ModifyGame(INetworkStreamHandler nwsh) : base(nwsh)
        {
        }

        public override Command cmd => Command.MODIFY_GAME;

        public override void ParsedRequestHandler(string[] req)
        {
            // Game cover is downloaded first, so all request data is read before sending an error to the user
            //  in case of an incorrect parameter, so the reading stream is clear before next request
            Game modifiedGame = new Game();
            if(req[5] == "1")
            {
                string coverPath = fileNetworkStreamHandler.ReceiveFile(ServerConfig.GameCoverPath);
                modifiedGame.CoverFilePath = coverPath;
            }

            modifiedGame = new Game
            {
                Title = req[1],
                Synopsis = req[2],
                ESRBRating = (Common.ESRBRating)parseInt(req[3]),
                Genre = req[4]
            };

            int Id = parseInt(req[0]);
            Steam SteamInstance = Steam.GetInstance();
            string message;
            try
            {
                message = SteamInstance.ModifyGame(Id, modifiedGame);
            }
            catch (TitleAlreadyExistseException e) {
                message = $"Ya existe un juego con el titulo {modifiedGame.Title}";
                File.Delete(modifiedGame.CoverFilePath);
            }
            Respond(message);
        }

        private void Respond(string message)
        {
            networkStreamHandler.WriteString(Specification.responseHeader);
            networkStreamHandler.WriteCommand(Command.MODIFY_GAME);
            networkStreamHandler.WriteInt(message.Length);
            networkStreamHandler.WriteString(message);

        }
    }
}