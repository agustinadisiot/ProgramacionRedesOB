using Common.NetworkUtils.Interfaces;
using Common.Protocol;

namespace Server
{
    internal class DownloadCover : TextCommand
    {
        public DownloadCover(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override void ParsedRequestHandler(string[] req)
        {
            int gameID;
            bool correctID = int.TryParse(req[0], out gameID);
            // TODO ver que pasa si se parsea mal
            Steam SteamInstance = Steam.GetInstance();
            string coverPath = SteamInstance.GetCoverPath(gameID);
            Respond(coverPath);
        }

        private void Respond(string coverPath)
        {

            networkStreamHandler.WriteString(Specification.responseHeader);
            networkStreamHandler.WriteCommand(Command.DOWNLOAD_COVER);
            fileNetworkStreamHandler.SendFile(coverPath);

        }
    }
}