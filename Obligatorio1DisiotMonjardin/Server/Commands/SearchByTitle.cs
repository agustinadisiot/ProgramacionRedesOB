using Common.Domain;
using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Commands
{
    public class SearchByTitle : CreateGamePage
    {
        public SearchByTitle(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.SEARCH_BY_TITLE;

        protected override GamePage GetGamePage(int pageNumber, string unParsedfilter)
        {
            string titleToSearch = unParsedfilter;
            return steamInstance.SearchByTitle(pageNumber, titleToSearch);
        }
    }
}
