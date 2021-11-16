using System.Threading.Tasks;

namespace Common.FileHandler.Interfaces
{
    public interface IFileHandler
    {
        Task<bool> FileExists(string path);
        public Task<bool> PathExists(string directory);
        Task<string> GetFileName(string path);
        Task<long> GetFileSize(string path);
    }
}