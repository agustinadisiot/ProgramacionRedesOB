using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Common.Domain;

namespace Client.Commands
{
    public class BrowseReviews : TextCommand
    {
        public BrowseReviews(INetworkStreamHandler nwsh) : base(nwsh) { }


        public ReviewPage SendRequest(int pageNumber, int gameId)
        {
            SendHeader();
            SendCommand(Command.BROWSE_REVIEWS);

            string pageNumberText = pageNumber.ToString();
            SendData(pageNumberText + Specification.delimiter + gameId);
            return ResponseHandler(pageNumber);
        }

        private ReviewPage ResponseHandler(int pageNumber) // TODO capaz que el currentPage ya te lo de el server (lo tendria  que mandar por el nwsh)
        {

            ReadHeader();
            ReadCommand(); // TODO ver si hacemos algo mas con estos 

            int dataLength = networkStreamHandler.ReadInt(Specification.dataSizeLength);
            string data = networkStreamHandler.ReadString(dataLength);


            string[] parsedData = Parse(data); // TODO usar GetData()
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
                    User reviewWriter = new User(reviewsInfoSepareted[i]);
                    Review newReview = new Review
                    {
                        User = reviewWriter, // TODO poner los indices en la specification capaz (en vez de usar +0, +1, +2)
                        Rating = int.Parse(reviewsInfoSepareted[i + 1]), // TODO usar Try parse y tirar error?
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
