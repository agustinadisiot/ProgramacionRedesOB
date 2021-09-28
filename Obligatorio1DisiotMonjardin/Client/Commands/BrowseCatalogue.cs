using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;

namespace Client.Commands
{
    public class BrowseCatalogue : CreateGamePage
    {
        public BrowseCatalogue(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.BROWSE_CATALOGUE;

        public GamePage SendRequest(int pageNumber)
        {
            SendHeader();

            string pageNumberText = pageNumber.ToString();
            SendData(pageNumberText);
            return ResponseHandler(pageNumber);
        }


    }
}
