using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;

namespace Client
{
    public class Logout : TextCommand
    {
        public Logout(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.LOGOUT;

        public bool SendRequest()
        {
            SendHeader();

            // TODO ver que hacer aca, si sacarlo de Text command o que
            SendData("");
            return ResponseHandler();
        }

        private bool ResponseHandler()
        {
            string[] data = GetData();
            bool response = ToBooleanFromString(data[0]);
            // TODO ver para que usamos la response
            return response;
        }

        private bool ToBooleanFromString(string text)
        {
            return (text == "1");
        }
    }
}