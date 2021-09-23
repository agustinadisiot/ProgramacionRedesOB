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

        public override Command cmd => Command.WRITE_REVIEW;

        public override void ParsedRequestHandler(string[] req)
        {
            int gameId = int.Parse(req[0]);
            Review newReview = new Review
            {
                Rating = int.Parse(req[1]),
                Text = req[2]
            };
            Steam SteamInstance = Steam.GetInstance();
            SteamInstance.WriteReview(newReview, gameId, networkStreamHandler);
            string message = "Reseña agregada existosamente"; // TODO agregar catch para cuando tira error TODO cambiar mensaje
            Respond(message);
        }

        private void Respond(string message)
        {
            SendResponseHeader();
            SendData(message);
        }

    }
}
