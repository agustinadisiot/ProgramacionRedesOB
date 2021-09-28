using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using Server.BusinessLogic;
using System;

namespace Server
{
    public class DownloadCover : TextCommand
    {
        public DownloadCover(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.DOWNLOAD_COVER;

        public override void ParsedRequestHandler(string[] req)
        {
            int gameId = parseInt(req[0]);
            BusinessLogicGameInfo GameInfo = BusinessLogicGameInfo.GetInstance();

            bool coverIsLocked = false;
            while (!coverIsLocked)
            {
                string lockedCoverPath = GameInfo.GetCoverPath(gameId);
                // Aca se podria modificar SteamInstance.GetCoverPath(gameId);
                lock (lockedCoverPath)
                {
                    string latestCoverPath = GameInfo.GetCoverPath(gameId);
                    // chequeamos que tenemos el lock al path correcto
                    if (lockedCoverPath == latestCoverPath)
                    {
                        Respond(lockedCoverPath);
                        coverIsLocked = true;
                    }
                }
            }
        }



        private void Respond(string coverPath)
        {
            SendResponseHeader();
            fileNetworkStreamHandler.SendFile(coverPath);

        }
    }
}