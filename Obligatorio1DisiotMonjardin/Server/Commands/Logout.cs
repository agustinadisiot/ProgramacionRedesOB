using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using Server.BusinessLogic;
using System.Threading.Tasks;

namespace Server.Commands.BaseCommands
{
    public class Logout : TextCommand
    {

        public Logout(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.LOGOUT;

        public override async Task ParsedRequestHandler(string[] req)
        {
            BusinessLogicSession Session = BusinessLogicSession.GetInstance();
            bool success = Session.Logout(networkStreamHandler);
            await Respond (success);
        }

        private async Task Respond(bool resp)
        {
            await SendResponseHeader ();
            string stringResp = resp ? "1" : "0";
            await SendData (stringResp);
        }
    }
}
