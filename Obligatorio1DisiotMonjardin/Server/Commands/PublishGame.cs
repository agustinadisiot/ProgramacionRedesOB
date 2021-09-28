using Common.Domain;
using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using Server.BusinessLogic;
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
            Game newGame = new Game
            {
                Title = req[0],
                Synopsis = req[1],
                Genre = req[3]
            };
            string coverPath = fileNetworkStreamHandler.ReceiveFile(ServerConfig.GameCoverPath); // TODO ver donde si dejamos el serverConfig aca
            newGame.CoverFilePath = coverPath;

            BusinessLogicGameCUD GameCUD = BusinessLogicGameCUD.GetInstance();
            string message;
            try
            {
                newGame.ESRBRating = (Common.ESRBRating)parseInt(req[2]); // can thow server error if not a number
                message = GameCUD.PublishGame(newGame, networkStreamHandler);
            }
            catch (Exception e) when (e is TitleAlreadyExistsException || e is ServerError)
            {
                message = e.Message;
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
