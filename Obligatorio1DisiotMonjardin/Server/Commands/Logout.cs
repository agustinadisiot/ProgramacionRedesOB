using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Commands.BaseCommands
{
    public class Logout : TextCommand
    {

        public Logout(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override void ParsedRequestHandler(string[] req)
        {
            Steam Steam = Steam.GetInstance();
            bool success = Steam.Logout(networkStreamHandler);
            Respond(success);
            Console.WriteLine("Logged out");
        }

        private void Respond(bool resp)
        {
            networkStreamHandler.WriteString(Specification.responseHeader);
            networkStreamHandler.WriteCommand(Command.LOGOUT);
            string stringResp = resp ? "1" : "0";
            networkStreamHandler.WriteInt(1);
            networkStreamHandler.WriteString(stringResp);
        }
    }
}
