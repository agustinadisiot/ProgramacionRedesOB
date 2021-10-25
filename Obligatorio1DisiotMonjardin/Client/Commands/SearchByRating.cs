using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System.Threading.Tasks;

namespace Client.Commands
{
    public class SearchByRating : CreateGamePage
    {
        public SearchByRating(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.SEARCH_BY_RATING;

        public async Task<GamePage> SendRequest(int pageNumber, int rating)
        {
            await SendHeader();

            string pageNumberText = pageNumber.ToString();
            await SendData(pageNumberText + Specification.FIRST_DELIMITER + rating);
            return await ResponseHandler(pageNumber);
        }



    }
}
