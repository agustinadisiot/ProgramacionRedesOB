using Common.NetworkUtils.Interfaces;
using Common.Protocol;
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
            Steam SteamInstance = Steam.GetInstance();

            bool coverIsLocked = false;
            while (!coverIsLocked)
            {
                string lockedCoverPath = SteamInstance.GetCoverPath(gameId);
                // Aca se podria modificar SteamInstance.GetCoverPath(gameId);
                lock (lockedCoverPath)
                {
                    string latestCoverPath = SteamInstance.GetCoverPath(gameId);
                    // chequeamos que tenemos el lock al path correcto
                    if (lockedCoverPath == latestCoverPath)
                    {
                        Respond(lockedCoverPath);
                        coverIsLocked = true;
                    }
                }
                string coverPath = SteamInstance.GetCoverPath(gameId);
                // Aca se podria modificar el coverPath, ya que ni SteamInstance ni esta clase tienen lock 
                lock (coverPath)
                {
                    Respond(coverPath);
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