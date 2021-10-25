using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using Server.BusinessLogic;
using System.Threading.Tasks;

namespace Server
{
    public class DownloadCover : TextCommand
    {
        public DownloadCover(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.DOWNLOAD_COVER;

        public override async Task ParsedRequestHandler(string[] req)
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



        private async Task Respond(string coverPath)
        {
            await SendResponseHeader();
            await fileNetworkStreamHandler.SendFile(coverPath);

        }
    }
}