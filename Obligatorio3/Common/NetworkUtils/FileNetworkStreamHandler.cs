using Common.FileHandler;
using Common.FileHandler.Interfaces;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using Common.Utils;
using System;
using System.Threading.Tasks;

namespace Common.NetworkUtils
{
    public class FileNetworkStreamHandler : IFileNetworkStreamHandler
    {
        private readonly INetworkStreamHandler networkStreamHandler;
        private readonly IFileHandler fileHandler;
        private readonly IFileStreamHandler fileStreamHandler;
        public FileNetworkStreamHandler(INetworkStreamHandler nwsh)
        {
            networkStreamHandler = nwsh;
            fileStreamHandler = new FileStreamHandler();
            fileHandler = new FileHandler.FileHandler();

        }
        public async Task<string> ReceiveFile(string folderPath, string fileName = "")
        {
            {
                long fileSize = await networkStreamHandler.ReadFileSize();

                long parts = SpecificationHelper.GetParts(fileSize);
                long offset = 0;
                long currentPart = 1;

                // Generates a random file Name to avoid duplicates
                if (fileName == "")
                    fileName = Guid.NewGuid().ToString();

                if ((!folderPath.Trim().EndsWith("\\")) && (!folderPath.Trim().EndsWith("/")))
                    folderPath += "\\";

                string completeFileName = folderPath + fileName + LogicSpecification.IMAGE_EXTENSION;

                while (fileSize > offset)
                {
                    byte[] data;
                    if (currentPart == parts)
                    {
                        var lastPartSize = (int)(fileSize - offset);
                        data = await networkStreamHandler.Read(lastPartSize);
                        offset += lastPartSize;
                    }
                    else
                    {
                        data = await networkStreamHandler.Read(Specification.MAX_PACKET_SIZE);
                        offset += Specification.MAX_PACKET_SIZE;
                    }
                    await fileStreamHandler.Write(completeFileName, data);
                    currentPart++;
                }
                return completeFileName;
            }
        }


        public async Task SendFile(string path)
        {
            if (!await fileHandler.FileExists(path))
                throw new Exception("File does not exists");

            long fileSize = await fileHandler.GetFileSize(path);
            await networkStreamHandler.WriteFileSize(fileSize);

            var parts = SpecificationHelper.GetParts(fileSize);
            long offset = 0;
            long currentPart = 1;

            while (fileSize > offset)
            {
                byte[] data;
                if (currentPart == parts)
                {
                    var lastPartSize = (int)(fileSize - offset);
                    data = await fileStreamHandler.Read(path, offset, lastPartSize);
                    offset += lastPartSize;
                }
                else
                {
                    data = await fileStreamHandler.Read(path, offset, Specification.MAX_PACKET_SIZE);
                    offset += Specification.MAX_PACKET_SIZE;
                }

                await networkStreamHandler.Write(data);
                currentPart++;
            }
        }
    }
}

