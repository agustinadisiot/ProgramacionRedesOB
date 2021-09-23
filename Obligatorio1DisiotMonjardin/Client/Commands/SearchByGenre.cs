using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Common.Domain;

namespace Client.Commands
{
    public class SearchByGenre : CreateGamePage
    {
        public SearchByGenre(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.SEARCH_BY_GENRE;

        public GamePage SendRequest(int pageNumber, string genre)
        {
            SendHeader();

            string pageNumberText = pageNumber.ToString();
            SendData(pageNumberText + Specification.delimiter + genre);
            return ResponseHandler(pageNumber);
        }



    }
}
