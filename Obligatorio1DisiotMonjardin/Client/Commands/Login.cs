using Common.NetworkUtils.Interfaces;
using Common.Protocol;

namespace Client
{
    public class Login : TextCommand
    {
        public Login(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.LOGIN;

        public bool SendRequest(string username)
        {
            SendHeader();

            SendData(username);
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