using Common;
using Common.Domain;
using Common.Interfaces;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using Server.BusinessLogic;
using System;
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
            int Id = int.Parse(req[0]);
            Game modifiedGame = new Game
            {
                Title = req[1],
                Synopsis = req[2],
                Genre = req[4]
            };

            if (req[5] == "1")
            {
                ISettingsManager SettingsMgr = new SettingsManager();
                string folderPath = SettingsMgr.ReadSetting(ServerConfig.GameCoverPathKey);
                string coverPath = fileNetworkStreamHandler.ReceiveFile(folderPath);
                modifiedGame.CoverFilePath = coverPath;
            }

            BusinessLogicGameCUD GameCUD = BusinessLogicGameCUD.GetInstance();
            string message;
            try
            {
                int gameId = parseInt(req[0]); // can thow serverError
                modifiedGame.ESRBRating = (Common.ESRBRating)parseInt(req[3]);
                message = GameCUD.ModifyGame(gameId, modifiedGame);
            }
            catch (Exception e) when (e is TitleAlreadyExistsException || e is ServerError)
            {
                message = e.Message;
                if (req[5] == "1")
                    File.Delete(modifiedGame.CoverFilePath);
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