using Common.FileHandler;
using Common.FileHandler.Interfaces;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Text;

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
        public string ReceiveFile(string folderPath, string fileName = "")
        {
            {
                long fileSize = networkStreamHandler.ReadFileSize();

                long parts = SpecificationHelper.GetParts(fileSize);
                long offset = 0;
                long currentPart = 1;

                // Generates a random file Name to avoid duplicates
                if (fileName == "")
                    fileName = Guid.NewGuid().ToString();

                if (!folderPath.Trim().EndsWith("\\"))
                    folderPath += "\\"; // TODO ver que pasa si me pasan el folder con "/"

                string completeFileName = folderPath + fileName + Specification.IMAGE_EXTENSION; // TODO ver si poner un while que cheque que el file no existe o si usar otro metodo

                while (fileSize > offset)
                {
                    byte[] data;
                    if (currentPart == parts)
                    {
                        var lastPartSize = (int)(fileSize - offset);
                        data = networkStreamHandler.Read(lastPartSize);
                        offset += lastPartSize;
                    }
                    else
                    {
                        data = networkStreamHandler.Read(Specification.MAX_PACKET_SIZE);
                        offset += Specification.MAX_PACKET_SIZE;
                    }
                    fileStreamHandler.Write(completeFileName, data);
                    currentPart++;
                }
                return completeFileName;
            }
        }


        public void SendFile(string path)
        {
            if (!fileHandler.FileExists(path))
                throw new Exception("File does not exists"); // TODO catchear capaz 

            long fileSize = fileHandler.GetFileSize(path); 
            networkStreamHandler.WriteFileSize(fileSize);

            var parts = SpecificationHelper.GetParts(fileSize);
            long offset = 0;
            long currentPart = 1;

            while (fileSize > offset)
            {
                byte[] data;
                if (currentPart == parts)
                {
                    var lastPartSize = (int)(fileSize - offset);
                    data = fileStreamHandler.Read(path, offset, lastPartSize);
                    offset += lastPartSize;
                }
                else
                {
                    data = fileStreamHandler.Read(path, offset, Specification.MAX_PACKET_SIZE);
                    offset += Specification.MAX_PACKET_SIZE;
                }

                networkStreamHandler.Write(data);
                currentPart++;
            }
        }
    }
}

