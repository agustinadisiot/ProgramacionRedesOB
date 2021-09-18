using Common.Domain;
using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Client.Commands
{
    public class BuyGame : TextCommand
    {
        public BuyGame(INetworkStreamHandler nwsh) : base(nwsh) { }

        public string SendRequest(int gameID)
        {
            SendHeader();
            SendCommand(Command.BUY_GAME);

            string data = "";
            data += gameID;

            SendData(data);
            return ResponseHandler();
        }


        private string ResponseHandler()
        {
            string[] data = GetData();
            string message = data[0];
            return message;

        }
    }
}
