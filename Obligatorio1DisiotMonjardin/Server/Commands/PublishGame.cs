using Common.Domain;
using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
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
                Title = req[0],
                Synopsis = req[1],
                ReviewsRating = int.Parse(req[2]),
                ESRBRating = (Common.ESRBRating)int.Parse(req[3]),
                Genre = req[4]
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
