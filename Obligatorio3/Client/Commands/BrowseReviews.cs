using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Commands
{
    public class BrowseReviews : TextCommand
    {
        public BrowseReviews(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.BROWSE_REVIEWS;

        public async Task<ReviewPage> SendRequest(int pageNumber, int gameId)
        {
            await SendHeader();

            string pageNumberText = pageNumber.ToString();
            await SendData (pageNumberText + Specification.FIRST_DELIMITER + gameId);
            return await ResponseHandler(pageNumber);
        }

        private async Task<ReviewPage> ResponseHandler(int pageNumber)
        {
            string[] parsedData = await GetData();
            List<string> unParsedReviews = parsedData.ToList();
            unParsedReviews.RemoveRange(parsedData.Length - 2, 2);
            List<Review> parsedReviews = ParseReviews(unParsedReviews);
            ReviewPage reviewPage = new ReviewPage()
            {
                Reviews = parsedReviews,
                CurrentPage = pageNumber,
                HasNextPage = ToBooleanFromString(parsedData[parsedData.Length - 2]),
                HasPreviousPage = ToBooleanFromString(parsedData[parsedData.Length - 1])
            };

            return reviewPage;
        }
        private List<Review> ParseReviews(List<string> unParsedReviews)
        {
            List<Review> reviews = new List<Review>();

            foreach (string unParsedReview in unParsedReviews)
            {
                string[] reviewsInfoSepareted = ParseBySecondDelimiter(unParsedReview);

                for (int i = 0; i < reviewsInfoSepareted.Length; i += 3)
                {
                    User reviewWriter = new User()
                    {
                        Name = reviewsInfoSepareted[i],
                    };
                    Review newReview = new Review
                    {
                        Author = reviewWriter,
                        Rating = int.Parse(reviewsInfoSepareted[i + 1]),
                        Text = reviewsInfoSepareted[i + 2],
                    };
                    reviews.Add(newReview);
                }
            }
            return reviews;
        }

        private bool ToBooleanFromString(string text)
        {
            return (text == "1");
        }

    }
}
