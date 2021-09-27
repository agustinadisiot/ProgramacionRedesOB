using Common.NetworkUtils.Interfaces;
using Common.Protocol;

namespace Client
{
    public class DownloadCover : TextCommand
    {
        public DownloadCover(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.DOWNLOAD_COVER;

        public string SendRequest(int gameId, string folderPath, string fileName)
        {
            SendHeader();

            SendData(gameId.ToString());
            return ResponseHandler(folderPath, fileName);
        }

        private string ResponseHandler(string folderPath, string fileName)
        {

            ReadHeader();
            ReadCommand();

            string completePath = fileNetworkStreamHandler.ReceiveFile(folderPath, fileName);
            return completePath;
        }


    }
}