using Common.Domain;
using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Commands
{
    public class BrowseReviews : TextCommand
    {
        public BrowseReviews(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.BROWSE_REVIEWS;

        public override void ParsedRequestHandler(string[] req)
        {
            Steam Steam = Steam.GetInstance();
            int pageNumber = int.Parse(req[0]);
            int gameId = int.Parse(req[1]);
            ReviewPage reviewPage = Steam.BrowseReviews(pageNumber, gameId);
            Respond(reviewPage);
        }

        private void Respond(ReviewPage reviewPage)
        {
            string data = "";
            foreach (Review review in reviewPage.Reviews)
            {
                data += review.User.Name;
                data += Specification.secondDelimiter;
                data += review.Rating;
                data += Specification.secondDelimiter;
                data += review.Text;

                data += Specification.delimiter;
            }

            data += Convert.ToInt32(reviewPage.HasNextPage);
            data += Specification.delimiter;
            data += Convert.ToInt32(reviewPage.HasPreviousPage);

            SendResponseHeader();
            SendData(data);

        }
    }
}
