using Common.Domain;
using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

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
            return steamInstance.SearchByRating(pageNumber, minRating);
        }
    }
}
