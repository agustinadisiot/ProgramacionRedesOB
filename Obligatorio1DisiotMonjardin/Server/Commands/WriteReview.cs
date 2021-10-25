using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using Server.BusinessLogic;
using System.Threading.Tasks;

namespace Server.Commands
{
    public class WriteReview : TextCommand
    {
        public WriteReview(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.WRITE_REVIEW;

        public override async Task ParsedRequestHandler(string[] req)
        {
            int gameId = parseInt(req[0]);
            Review newReview = new Review
            {
                Rating = parseInt(req[1]),
                Text = req[2]
            };
            BusinessLogicReview Review = BusinessLogicReview.GetInstance();
            string message = Review.WriteReview(newReview, gameId, networkStreamHandler);
            await Respond(message);
        }

        private async Task Respond(string message)
        {
            await SendResponseHeader();
            await SendData(message);
        }

    }
}
