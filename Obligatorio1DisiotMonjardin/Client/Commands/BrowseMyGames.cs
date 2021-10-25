using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System.Threading.Tasks;

namespace Client.Commands
{
    public class BrowseMyGames : CreateGamePage
    {
        public BrowseMyGames(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.BROWSE_MY_GAMES;

        public async Task<GamePage> SendRequest(int pageNumber)
        {
            await SendHeader();

            string pageNumberText = pageNumber.ToString();
            await SendData(pageNumberText);
            return await ResponseHandler(pageNumber);
        }


    }
}
