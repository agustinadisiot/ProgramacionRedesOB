using Common.NetworkUtils.Interfaces;
using Common.Protocol;

namespace Client
{
    public class Logout : TextCommand
    {
        public Logout(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.LOGOUT;

        public bool SendRequest()
        {
            SendHeader();
            SendData("");
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