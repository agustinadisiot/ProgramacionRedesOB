using Common.Domain;
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
            SteamInstance.PublishGame(newGame, networkStreamHandler);
            string message = "Game added succesfully"; // TODO agregar catch para cuando tira error
            Respond(message);
        }

        private void Respond(string message)
        {
            
            networkStreamHandler.WriteString(Specification.responseHeader);
            networkStreamHandler.WriteCommand(Command.PUBLISH_GAME);
            networkStreamHandler.WriteInt(message.Length);
            networkStreamHandler.WriteString(message);

        }

    }
}
