using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using Server.BusinessLogic;
using System;
using System.Threading.Tasks;

namespace Server.Commands
{
    public class BrowseReviews : TextCommand
    {
        public BrowseReviews(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.BROWSE_REVIEWS;

        public override async Task ParsedRequestHandler(string[] req)
        {
            BusinessLogicReview Review = BusinessLogicReview.GetInstance();
            int pageNumber = parseInt(req[0]);
            int gameId = parseInt(req[1]);
            ReviewPage reviewPage = Review.BrowseReviews(pageNumber, gameId);
            await Respond(reviewPage);
        }

        private async Task Respond(ReviewPage reviewPage)
        {
            string data = "";
            foreach (Review review in reviewPage.Reviews)
            {
                data += review.Author.Name;
                data += Specification.SECOND_DELIMITER;
                data += review.Rating;
                data += Specification.SECOND_DELIMITER;
                data += review.Text;

                data += Specification.FIRST_DELIMITER;
            }

            data += Convert.ToInt32(reviewPage.HasNextPage);
            data += Specification.FIRST_DELIMITER;
            data += Convert.ToInt32(reviewPage.HasPreviousPage);

            await SendResponseHeader();
            await SendData(data);

        }
    }
}
