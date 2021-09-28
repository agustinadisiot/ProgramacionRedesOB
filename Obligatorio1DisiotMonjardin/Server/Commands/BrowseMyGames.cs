using Common.Domain;
using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Commands
{
    public class BrowseMyGames : CreateGamePage
    {
        public BrowseMyGames(INetworkStreamHandler nwsh) : base(nwsh) { }
        public override Command cmd => Command.BROWSE_MY_GAMES;

        protected override GamePage GetGamePage(int pageNumber, string unParsedfilter)
        {
            return GamePage.BrowseMyGames(pageNumber, networkStreamHandler);
        }
    }
}
