using Common.NetworkUtils.Interfaces;
using Common.Protocol;

namespace Server.Commands.BaseCommands
{
    public class Login : TextCommand
    {

        public Login(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.LOGIN;

        public override void ParsedRequestHandler(string[] req)
        {
            Steam Steam = Steam.GetInstance();
            string newUser = req[0];
            bool added = Steam.Login(newUser, networkStreamHandler);
            Respond(added);
        }

        private void Respond(bool resp)
        {
            SendResponseHeader();
            string stringResp = resp ? "1" : "0";
            SendData(stringResp);
        }
    }
}
