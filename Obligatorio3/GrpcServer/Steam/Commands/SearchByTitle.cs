using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;

namespace Server.Commands
{
    public class SearchByTitle : CreateGamePage
    {
        public SearchByTitle(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.SEARCH_BY_TITLE;

        protected override GamePage GetGamePage(int pageNumber, string unParsedfilter)
        {
            string titleToSearch = unParsedfilter;
            return GamePage.SearchByTitle(pageNumber, titleToSearch);
        }
    }
}
