using Common;
using Common.Domain;
using Common.Interfaces;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using Server.BusinessLogic;
using System;
using System.IO;

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
            ISettingsManager SettingsMgr = new SettingsManager();
            string folderPath = SettingsMgr.ReadSetting(ServerConfig.GameCoverPathKey);
            string coverPath = fileNetworkStreamHandler.ReceiveFile(folderPath);
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
