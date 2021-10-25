using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System.Threading.Tasks;

namespace Client
{
    public class DownloadCover : TextCommand
    {
        public DownloadCover(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.DOWNLOAD_COVER;

        public async Task<string> SendRequest(int gameId, string folderPath, string fileName)
        {
            await SendHeader ();

            await SendData(gameId.ToString());
            return await ResponseHandler(folderPath, fileName);
        }

        private async Task<string> ResponseHandler(string folderPath, string fileName)
        {
            await ReadHeader();
            await ReadCommand ();

            string completePath = await fileNetworkStreamHandler.ReceiveFile(folderPath, fileName);
            return completePath;
        }


    }
}