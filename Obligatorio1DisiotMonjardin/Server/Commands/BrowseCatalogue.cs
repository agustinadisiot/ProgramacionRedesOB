using Common.Domain;
using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

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
