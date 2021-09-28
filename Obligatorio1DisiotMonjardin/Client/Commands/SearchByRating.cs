using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;

namespace Client.Commands
{
    public class SearchByRating : CreateGamePage
    {
        public SearchByRating(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.SEARCH_BY_RATING;

        public GamePage SendRequest(int pageNumber, int rating)
        {
            SendHeader();

            string pageNumberText = pageNumber.ToString();
            SendData(pageNumberText + Specification.FIRST_DELIMITER + rating);
            return ResponseHandler(pageNumber);
        }



    }
}
