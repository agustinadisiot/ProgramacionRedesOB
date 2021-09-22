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
        public string ReceiveFile(string folderPath)
        {
            {
                long fileSize = networkStreamHandler.ReadFileSize();

                long parts = SpecificationHelper.GetParts(fileSize);
                long offset = 0;
                long currentPart = 1;

                // Generates a random file Name to avoid duplicates
                string fileName = folderPath + Guid.NewGuid().ToString() + ".jpg"; // TODO ver si poner un while que cheque que el file no existe o si usar otro metodo

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
                        data = networkStreamHandler.Read(Specification.MaxPacketSize);
                        offset += Specification.MaxPacketSize;
                    }
                    fileStreamHandler.Write(fileName, data);
                    currentPart++;
                }
                return fileName;
            }
        }


        public void SendFile(string path)
        {
            if (!fileHandler.FileExists(path))
                throw new Exception("File does not exists");
            // El envio del archivo se compone de las siguientes etapas:
            // 1) Creo un paquete de datos que tiene esta estructura XXXX YYYYYYYY <NOMBRE>
            //          a) XXXX -> Largo del nombre del archivo
            //          b) YYYYYYYY -> Tamaño en bytes del archivo
            //          c) <NOMBRE> -> Nombre del archivo

            long fileSize = fileHandler.GetFileSize(path); //Obtenemos el tamaño del archivo
            networkStreamHandler.WriteFileSize(fileSize);
            // 2) Calculo tamaño y cantidad de partes a enviar
            var parts = SpecificationHelper.GetParts(fileSize);
            long offset = 0;
            long currentPart = 1;

            // 3) Mientras tengo partes, envio TODO borrar comentarios del profe
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
                    data = fileStreamHandler.Read(path, offset, Specification.MaxPacketSize);
                    offset += Specification.MaxPacketSize;
                }

                networkStreamHandler.Write(data);
                currentPart++;
            }
        }
    }
}

