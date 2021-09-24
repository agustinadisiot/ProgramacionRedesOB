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
    public class SearchByRating : CreateGamePage
    {
        public SearchByRating(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.SEARCH_BY_RATING;

        public GamePage SendRequest(int pageNumber, int rating)
        {
            SendHeader();

            string pageNumberText = pageNumber.ToString();
            SendData(pageNumberText + Specification.delimiter + rating);
            return ResponseHandler(pageNumber);
        }



    }
}
