using Common.Domain;
using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Commands
{
    public class WriteReview : TextCommand
    {
        public WriteReview(INetworkStreamHandler nwsh) : base(nwsh) { }
        public override void ParsedRequestHandler(string[] req)
        {
            int gameId = int.Parse(req[0]);
            Review newReview = new Review
            {
                Rating = int.Parse(req[0]),
                Text = req[1]
            };
            Steam SteamInstance = Steam.GetInstance();
            SteamInstance.WriteReview(newReview, gameId, networkStreamHandler);
            string message = "Review added succesfully"; // TODO agregar catch para cuando tira error
            Respond(message);
        }

        private void Respond(string message)
        {
            
            networkStreamHandler.WriteString(Specification.responseHeader);
            networkStreamHandler.WriteCommand(Command.WRITE_REVIEW);
            networkStreamHandler.WriteInt(message.Length);
            networkStreamHandler.WriteString(message);

        }

    }
}
