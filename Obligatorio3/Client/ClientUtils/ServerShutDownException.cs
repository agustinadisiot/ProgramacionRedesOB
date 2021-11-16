using System;

namespace Client
{
    [Serializable]
    internal class ServerShutDownException : Exception
    {
        public ServerShutDownException() { }
    }
}