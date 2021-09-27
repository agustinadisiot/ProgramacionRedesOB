using System;
using System.Runtime.Serialization;

namespace Client
{
    [Serializable]
    internal class ServerShutDown : Exception
    {
        public ServerShutDown() { }
    }
}