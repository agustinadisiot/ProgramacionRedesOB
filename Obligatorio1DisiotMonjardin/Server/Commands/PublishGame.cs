using Common.Domain;
using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Commands
{
    public class PublishGame : TextCommand
    {
        public PublishGame(INetworkStreamHandler nwsh) : base(nwsh) { }
        public override void ParsedRequestHandler(string[] req)
        {
            Game newGame = new Game
            {
                Title = req[0],
                Synopsis = req[1],
                ESRBRating = (Common.ESRBRating)int.Parse(req[2]),
                Genre = req[3]
            };

            string coverPath = fileNetworkStreamHandler.ReceiveFile(ServerConfig.GameCoverPath); // TODO ver donde si dejamos el serverConfig aca
            newGame.CoverFilePath = coverPath;
            Steam SteamInstance = Steam.GetInstance();
            SteamInstance.PublishGame(newGame, networkStreamHandler);
            string message = "Game added succesfully"; // TODO agregar catch para cuando tira error
            Respond(message);
        }

        private void Respond(string message)
        {

            networkStreamHandler.WriteString(Specification.responseHeader);
            networkStreamHandler.WriteCommand(Command.PUBLISH_GAME);
            networkStreamHandler.WriteInt(message.Length);
            networkStreamHandler.WriteString(message);

        }

    }
}
