using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Protocol
{
    public class ServerError : Exception
    {
        public string message { get; set; }
        public ServerError(string errorMessage) {
            message = errorMessage;
        }
    }
}
