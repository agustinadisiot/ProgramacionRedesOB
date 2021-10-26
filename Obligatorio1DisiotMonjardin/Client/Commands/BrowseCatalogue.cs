using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System.Threading.Tasks;

namespace Client.Commands
{
    public class BrowseCatalogue : CreateGamePage
    {
        public BrowseCatalogue(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.BROWSE_CATALOGUE;

        public async Task<GamePage> SendRequest(int pageNumber)
        {
            await SendHeader();

            string pageNumberText = pageNumber.ToString();
            await SendData(pageNumberText);
            return await ResponseHandler(pageNumber);
        }


    }
}
