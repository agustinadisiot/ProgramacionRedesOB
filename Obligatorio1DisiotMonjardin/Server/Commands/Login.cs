using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Commands.BaseCommands
{
    public class Login : TextCommand
    {

        public Login(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override void ParsedRequestHandler(string[] req)
        {
            Steam Steam = Steam.GetInstance();
            string newUser = req[0];
            bool added = Steam.Login(newUser, networkStreamHandler);
            Respond(added);
            Console.WriteLine("Logged");
        }

        private void Respond(bool resp)
        {
            networkStreamHandler.WriteString(Specification.responseHeader);
            networkStreamHandler.WriteCommand(Command.LOGIN);
            string stringResp = resp ? "1" : "0";
            networkStreamHandler.WriteInt(1);
            networkStreamHandler.WriteString(stringResp);
        }
    }
}
