
using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;

namespace Server.Commands
{
    public class SearchByGenre : CreateGamePage
    {
        public SearchByGenre(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.SEARCH_BY_GENRE;

        protected override GamePage GetGamePage(int pageNumber, string unParsedfilter)
        {
            string genre = unParsedfilter;
            return GamePage.SearchByGenre(pageNumber, genre);
        }
    }
}
