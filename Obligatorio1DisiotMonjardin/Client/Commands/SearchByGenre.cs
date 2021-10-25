using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System.Threading.Tasks;

namespace Client.Commands
{
    public class SearchByGenre : CreateGamePage
    {
        public SearchByGenre(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.SEARCH_BY_GENRE;

        public async Task<GamePage> SendRequest(int pageNumber, string genre)
        {
            await SendHeader();

            string pageNumberText = pageNumber.ToString();
            await SendData(pageNumberText + Specification.FIRST_DELIMITER + genre);
            return await ResponseHandler(pageNumber);
        }



    }
}
