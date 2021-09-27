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
            int pageNumber = parseInt(req[0]);
            int gameId = parseInt(req[1]);
            ReviewPage reviewPage = Steam.BrowseReviews(pageNumber, gameId);
            Respond(reviewPage);
        }

        private void Respond(ReviewPage reviewPage)
        {
            string data = "";
            foreach (Review review in reviewPage.Reviews)
            {
                data += review.User.Name;
                data += Specification.SECOND_DELIMITER;
                data += review.Rating;
                data += Specification.SECOND_DELIMITER;
                data += review.Text;

                data += Specification.FIRST_DELIMITER;
            }

            data += Convert.ToInt32(reviewPage.HasNextPage);
            data += Specification.FIRST_DELIMITER;
            data += Convert.ToInt32(reviewPage.HasPreviousPage);

            SendResponseHeader();
            SendData(data);

        }
    }
}
