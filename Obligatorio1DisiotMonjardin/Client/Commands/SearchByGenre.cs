using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;

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
            SendData(pageNumberText + Specification.FIRST_DELIMITER + genre);
            return ResponseHandler(pageNumber);
        }



    }
}
