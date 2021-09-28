namespace Common.FileHandler.Interfaces
{
    public interface IFileNetworkStreamHandler
    {
        string ReceiveFile(string folderPath, string fileName = "");
        void SendFile(string fileName);
    }
}
