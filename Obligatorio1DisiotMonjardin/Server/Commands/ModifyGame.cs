using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;

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
            int Id = int.Parse(req[0]);
            Game modifiedGame = new Game
            {
                Title = req[1],
                Synopsis = req[2],
                ESRBRating = (Common.ESRBRating)int.Parse(req[3]),
                Genre = req[4]
            };

            if(req[5] == "1")
            {
                string coverPath = fileNetworkStreamHandler.ReceiveFile(ServerConfig.GameCoverPath);
                modifiedGame.CoverFilePath = coverPath;
            }
            Steam SteamInstance = Steam.GetInstance();
            SteamInstance.ModifyGame(Id, modifiedGame);
            string message = "Juego modificado exitosamente"; 
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