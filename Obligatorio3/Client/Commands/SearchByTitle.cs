using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System.Threading.Tasks;

namespace Client.Commands
{
    public class SearchByTitle : CreateGamePage
    {
        public SearchByTitle(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.SEARCH_BY_TITLE;

        public async Task<GamePage> SendRequest(int pageNumber, string title)
        {
            await SendHeader();

            string pageNumberText = pageNumber.ToString();
            await SendData(pageNumberText + Specification.FIRST_DELIMITER + title);
            return await ResponseHandler(pageNumber);
        }



    }
}
