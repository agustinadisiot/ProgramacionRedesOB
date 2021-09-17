using Common.NetworkUtils.Interfaces;
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
            throw new NotImplementedException();
        }
    }
}
