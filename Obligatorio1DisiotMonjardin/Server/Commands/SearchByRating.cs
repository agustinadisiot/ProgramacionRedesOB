using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;

namespace Server.Commands
{
    public class SearchByRating : CreateGamePage
    {
        public SearchByRating(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.SEARCH_BY_RATING;

        protected override GamePage GetGamePage(int pageNumber, string unParsedfilter)
        {
            int minRating = 1;
            int.TryParse(unParsedfilter, out minRating);
            return GamePage.SearchByRating(pageNumber, minRating);
        }
    }
}
