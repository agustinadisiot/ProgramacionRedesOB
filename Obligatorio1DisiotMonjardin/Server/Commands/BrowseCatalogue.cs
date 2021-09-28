using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;

namespace Server.Commands
{
    public class BrowseCatalogue : CreateGamePage
    {
        public BrowseCatalogue(INetworkStreamHandler nwsh) : base(nwsh) { }
        public override Command cmd => Command.BROWSE_CATALOGUE;

        protected override GamePage GetGamePage(int pageNumber, string unParsedfilter)
        {
            return steamInstance.BrowseGames(pageNumber);
        }
    }
}
