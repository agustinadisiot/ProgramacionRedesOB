using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using Server.BusinessLogic;
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
            int gameId = parseInt(req[0]);
            Review newReview = new Review
            {
                Rating = parseInt(req[1]),
                Text = req[2]
            };
            BusinessLogicReview Review = BusinessLogicReview.GetInstance();
            string message = Review.WriteReview(newReview, gameId, networkStreamHandler);
            Respond(message);
        }

        private void Respond(string message)
        {
            SendResponseHeader();
            SendData(message);
        }

    }
}
