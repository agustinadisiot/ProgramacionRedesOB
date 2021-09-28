using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using Server.BusinessLogic;

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

            string coverPath = GameInfo.GetCoverPath(gameId);
            // Aca podria eliminarse la caratula 
            lock (coverPath)
            {
                Respond(coverPath);
            }

        }



        private void Respond(string coverPath)
        {
            SendResponseHeader();
            fileNetworkStreamHandler.SendFile(coverPath);

        }
    }
}