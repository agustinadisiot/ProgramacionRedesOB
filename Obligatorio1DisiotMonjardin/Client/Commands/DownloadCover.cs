using Common.NetworkUtils.Interfaces;
using Common.Protocol;

namespace Client
{
    internal class DownloadCover : TextCommand // TODO ver si dejar internal o public
    {
        public DownloadCover(INetworkStreamHandler nwsh) : base(nwsh) { }

        public string SendRequest(int gameId, string folderPath, string fileName)
        {
            SendHeader();
            SendCommand(Command.DOWNLOAD_COVER);

            SendData(gameId.ToString());
            return ResponseHandler(folderPath, fileName);
        }

        private string ResponseHandler(string folderPath, string fileName)
        {

            ReadHeader();
            ReadCommand(); // TODO ver si hacemos algo mas con estos 

            string completePath = fileNetworkStreamHandler.ReceiveFile(folderPath, fileName);
            return completePath;
        }


    }
}