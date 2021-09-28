using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;

namespace Client.Commands
{
    public class SearchByTitle : CreateGamePage
    {
        public SearchByTitle(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.SEARCH_BY_TITLE;

        public GamePage SendRequest(int pageNumber, string title)
        {
            SendHeader();

            string pageNumberText = pageNumber.ToString();
            SendData(pageNumberText + Specification.FIRST_DELIMITER + title);
            return ResponseHandler(pageNumber);
        }



    }
}
