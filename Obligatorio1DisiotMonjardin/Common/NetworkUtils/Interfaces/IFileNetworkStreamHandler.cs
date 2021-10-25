using System.Threading.Tasks;

namespace Common.FileHandler.Interfaces
{
    public interface IFileNetworkStreamHandler
    {
        Task<string> ReceiveFile(string folderPath, string fileName = "");
        Task SendFile(string fileName);
    }
}
