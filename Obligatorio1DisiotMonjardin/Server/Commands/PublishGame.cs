using Common.Domain;
using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Server.Commands
{
    public class PublishGame : TextCommand
    {
        public PublishGame(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.PUBLISH_GAME;

        public override void ParsedRequestHandler(string[] req)
        {
            // Game cover is downloaded first, so all request data is read before sending an error to the user
            //   in case of an incorrect parameter, so the reading stream is clear before next request
            string coverPath = fileNetworkStreamHandler.ReceiveFile(ServerConfig.GameCoverPath); // TODO ver donde si dejamos el serverConfig aca
            Game newGame = new Game
            {
                Title = req[0],
                Synopsis = req[1],
                ESRBRating = (Common.ESRBRating)parseInt(req[2]),
                Genre = req[3]
            };

            newGame.CoverFilePath = coverPath;
            Steam SteamInstance = Steam.GetInstance();
            string message;
            try
            {
                message = SteamInstance.PublishGame(newGame, networkStreamHandler);
            }
            catch (TitleAlreadyExistseException e) {
                message = $"Ya existe un juego con el titulo {newGame.Title}";
                File.Delete(coverPath); 
            }
            Respond(message);
        }

        private void Respond(string message)
        {
            SendResponseHeader();
            SendData(message);
        }

    }
}
