using Common.NetworkUtils.Interfaces;
using Common.Protocol;

namespace Server
{
    internal class DownloadCover : TextCommand
    {
        public DownloadCover(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.DOWNLOAD_COVER;

        public override void ParsedRequestHandler(string[] req)
        {
            int gameID  = parseInt(req[0]);
            Steam SteamInstance = Steam.GetInstance();
            string coverPath = SteamInstance.GetCoverPath(gameID);
            Respond(coverPath);
        }

        private void Respond(string coverPath)
        {
            SendResponseHeader();
            fileNetworkStreamHandler.SendFile(coverPath);

        }
    }
}