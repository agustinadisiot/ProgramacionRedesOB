using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Protocol
{
    public class ServerError : Exception
    {
        public ServerError(string errorMessage)
            : base(errorMessage) { }
    }
}
