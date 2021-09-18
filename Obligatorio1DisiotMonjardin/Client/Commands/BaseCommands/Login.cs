using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;

namespace Client
{
    public class Login : TextCommand
    {
        public Login(INetworkStreamHandler nwsh) : base(nwsh) { }

        public bool SendRequest(string userName)
        {
            SendHeader();
            SendCommand(Command.LOGIN);

            SendData(userName);
            return ResponseHandler();
        }

        private bool ResponseHandler()
        {
            string[] data = GetData();
            bool response = ToBooleanFromString(data[0]);
            return response;
        }

        private bool ToBooleanFromString(string text)
        {
            return (text == "1");
        }
    }
}