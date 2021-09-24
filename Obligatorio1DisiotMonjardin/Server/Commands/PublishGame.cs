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

        public override Command cmd => Command.PUBLISH_GAME;

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
            string message = "Juego agregado exitosamente"; // TODO agregar catch para cuando tira error
            // TODO borrar caratula si no se publico viene
            Respond(message);
        }

        private void Respond(string message)
        {
            SendResponseHeader();
            SendData(message);
        }

    }
}
