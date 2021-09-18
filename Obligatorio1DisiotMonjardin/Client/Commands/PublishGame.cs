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
    public class PublishGame : TextCommand
    {
        public PublishGame(INetworkStreamHandler nwsh) : base(nwsh) { }

        public string SendRequest(Game newGame)
        {
            SendHeader();
            SendCommand(Command.PUBLISH_GAME);

            string data = "";
            data += newGame.Title;
            // TODO agregar el resto
            // data += Specification.delimiter;
            /*data += newGame.Title;
            data += newGame.Title;
            data += newGame.Title;*/

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
