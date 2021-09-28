using System;
using System.Runtime.Serialization;

namespace Client
{
    [Serializable]
    internal class ServerShutDownException : Exception
    {
        public ServerShutDownException() { }
    }
}