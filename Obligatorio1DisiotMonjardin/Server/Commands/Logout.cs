﻿using Common.NetworkUtils.Interfaces;
using Common.Protocol;

namespace Server.Commands.BaseCommands
{
    public class Logout : TextCommand
    {

        public Logout(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.LOGOUT;

        public override void ParsedRequestHandler(string[] req)
        {
            Steam Steam = Steam.GetInstance();
            bool success = Steam.Logout(networkStreamHandler);
            Respond(success);
        }

        private void Respond(bool resp)
        {
            SendResponseHeader();
            string stringResp = resp ? "1" : "0";
            SendData(stringResp);
        }
    }
}
