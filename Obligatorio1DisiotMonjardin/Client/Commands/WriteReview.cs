using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System.Threading.Tasks;

namespace Client.Commands
{
    public class WriteReview : TextCommand
    {
        public WriteReview(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.WRITE_REVIEW;

        public async Task<string> SendRequest(Review newReview, int gameId)
        {
            await SendHeader();

            string data = "";
            data += gameId;
            data += Specification.FIRST_DELIMITER;
            data += newReview.Rating;
            data += Specification.FIRST_DELIMITER;

            data += newReview.Text;

            await SendData(data);
            return await ResponseHandler();
        }


        private async Task<string> ResponseHandler()
        {
            string[] data = await GetData();
            string message = data[0];
            return message;

        }
    }
}
