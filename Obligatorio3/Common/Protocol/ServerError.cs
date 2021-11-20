using System;

namespace Common.Protocol
{
    public class ServerError : Exception
    {
        public ServerError(string errorMessage)
            : base(errorMessage) { }
    }
}
