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
            int gameId;
            bool correctID = int.TryParse(req[0], out gameId);
            // TODO ver que pasa si se parsea mal
            Steam SteamInstance = Steam.GetInstance();
            string coverPath = SteamInstance.GetCoverPath(gameId);
            Respond(coverPath);
        }

        private void Respond(string coverPath)
        {
            SendResponseHeader();
            fileNetworkStreamHandler.SendFile(coverPath);

        }
    }
}