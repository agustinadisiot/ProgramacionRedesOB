﻿using Common.Domain;
using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using Server.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Commands
{
    public class PublishGame : TextCommand
    {
        public PublishGame(INetworkStreamHandler nwsh) : base(nwsh) { }
        public override void ParsedRequestHandler(string[] req)
        {
            Game newGame = new Game
            {
                Title = req[0]
                // TODO agregar los parametros de Game que faltan
            };
            Steam SteamInstance = Steam.GetInstance();
            SteamInstance.AddGame(newGame);
            string message = "Game added succesfully"; // TODO agregar catch para cuando tira error
            Respond(message);
        }

        private void Respond(string message)
        {
            
            _networkStreamHandler.WriteString(Specification.responseHeader);
            _networkStreamHandler.WriteCommand(Command.PUBLISH_GAME);
            _networkStreamHandler.WriteInt(message.Length);
            _networkStreamHandler.WriteString(message);

        }
    }
}
