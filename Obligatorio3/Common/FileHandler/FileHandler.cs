using Common.FileHandler.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Common.FileHandler
{
    public class FileHandler : IFileHandler
    {
        public async Task<bool> FileExists(string path)
        {
            return await Task.Run(()=>File.Exists(path));
        }

        public async Task<bool> PathExists(string directory)
        {
            return await Task.Run(()=>!File.Exists(directory) && Directory.Exists(directory));
        }

        public async Task<string> GetFileName(string path)
        {
            if (await FileExists(path))
            {
                return new FileInfo(path).Name;
            }

            throw new Exception("File does not exist");
        }

        public async Task<long> GetFileSize(string path)
        {
            if (await FileExists(path))
            {
                return new FileInfo(path).Length;
            }

            throw new Exception("File does not exist");
        }
    }
}