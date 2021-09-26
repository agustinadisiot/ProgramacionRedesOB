using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
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

            Game modifiedGame = new Game
            {
                Title = req[1],
                Synopsis = req[2],
                Genre = req[4]
            };

            if (req[5] == "1")
            {
                string coverPath = fileNetworkStreamHandler.ReceiveFile(ServerConfig.GameCoverPath);
                modifiedGame.CoverFilePath = coverPath;
            }

            Steam SteamInstance = Steam.GetInstance();
            string message;
            try
            {
                int id = parseInt(req[0]); // can thow serverError
                modifiedGame.ESRBRating = (Common.ESRBRating)parseInt(req[3]);
                message = SteamInstance.ModifyGame(id, modifiedGame);
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
            networkStreamHandler.WriteString(Specification.responseHeader);
            networkStreamHandler.WriteCommand(Command.MODIFY_GAME);
            networkStreamHandler.WriteInt(message.Length);
            networkStreamHandler.WriteString(message);

        }
    }
}