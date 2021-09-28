using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;

namespace Client.Commands
{
    public class WriteReview : TextCommand
    {
        public WriteReview(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.WRITE_REVIEW;

        public string SendRequest(Review newReview, int gameId)
        {
            SendHeader();

            string data = "";
            data += gameId;
            data += Specification.FIRST_DELIMITER;
            data += newReview.Rating;
            data += Specification.FIRST_DELIMITER;

            data += newReview.Text;

            SendData(data);
            return ResponseHandler();
        }


        private string ResponseHandler()
        {
            string[] data = GetData();
            string message = data[0];
            return message;

        }
    }
}
