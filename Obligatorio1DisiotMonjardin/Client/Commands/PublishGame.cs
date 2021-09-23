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

        public override Command cmd => Command.PUBLISH_GAME;

        public string SendRequest(Game newGame)
        {
            SendHeader();

            string data = "";
            data += newGame.Title;
            data += Specification.delimiter;
            data += newGame.Synopsis;
            data += Specification.delimiter;
            data += (int)newGame.ESRBRating;
            data += Specification.delimiter;
            data += newGame.Genre;

            SendData(data);
            fileNetworkStreamHandler.SendFile(newGame.CoverFilePath);
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
