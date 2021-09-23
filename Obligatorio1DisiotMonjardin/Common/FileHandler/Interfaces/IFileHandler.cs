namespace Common.FileHandler.Interfaces
{
    public interface IFileHandler
    {
        bool FileExists(string path);
        public bool PathExists(string directory);
        string GetFileName(string path);
        long GetFileSize(string path);
    }
}