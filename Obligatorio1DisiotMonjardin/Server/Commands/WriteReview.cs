using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;

namespace Server.Commands
{
    public class WriteReview : TextCommand
    {
        public WriteReview(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.WRITE_REVIEW;

        public override void ParsedRequestHandler(string[] req)
        {
            int gameId = parseInt(req[0]);
            Review newReview = new Review
            {
                Rating = parseInt(req[1]),
                Text = req[2]
            };
            Steam SteamInstance = Steam.GetInstance();
            string message = SteamInstance.WriteReview(newReview, gameId, networkStreamHandler);
            Respond(message);
        }

        private void Respond(string message)
        {
            SendResponseHeader();
            SendData(message);
        }

    }
}
