using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using Server.BusinessLogic;
using System.Threading.Tasks;

namespace Server.Commands.BaseCommands
{
    public class Login : TextCommand
    {

        public Login(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.LOGIN;

        public override async Task ParsedRequestHandler(string[] req)
        {
            BusinessLogicSession Session = BusinessLogicSession.GetInstance();
            string newUser = req[0];
            bool added = Session.Login(newUser, networkStreamHandler);
            await Respond (added);
        }

        private async Task Respond(bool resp)
        {
            await SendResponseHeader ();
            string stringResp = resp ? "1" : "0";
            await SendData(stringResp);
        }
    }
}
