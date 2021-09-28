using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;

namespace Server.Commands
{
    public class BrowseMyGames : CreateGamePage
    {
        public BrowseMyGames(INetworkStreamHandler nwsh) : base(nwsh) { }
        public override Command cmd => Command.BROWSE_MY_GAMES;

        protected override GamePage GetGamePage(int pageNumber, string unParsedfilter)
        {
            return steamInstance.BrowseMyGames(pageNumber, networkStreamHandler);
        }
    }
}
