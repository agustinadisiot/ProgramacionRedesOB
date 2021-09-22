using Common.NetworkUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.FileHandler.Interfaces
{
    public interface IFileNetworkStreamHandler
    {
        string ReceiveFile(string folderPath, string fileName = "");
        void SendFile(string fileName);
    }
}
