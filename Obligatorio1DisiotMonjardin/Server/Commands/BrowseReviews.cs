using Common.Domain;
using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Commands
{
    public class BrowseReviews : TextCommand
    {
        public BrowseReviews(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override void ParsedRequestHandler(string[] req)
        {
            Steam Steam = Steam.GetInstance();
            int pageNumber = int.Parse(req[0]);
            int gameId = int.Parse(req[1]);
            ReviewPage reviewPage = Steam.BrowseReviews(pageNumber, gameId);
            Respond(reviewPage);
        }

        private void Respond(ReviewPage reviewPage)  //todo refactor
        {
            byte[] header = Encoding.UTF8.GetBytes(Specification.responseHeader);
            ushort command = (ushort)Command.BROWSE_REVIEWS;
            byte[] cmd = BitConverter.GetBytes(command);

            //byte[] data = Encoding.UTF8.GetBytes(info);
            //byte[] dataLength = BitConverter.GetBytes(data.Length);

            // TODO usar stringStream?
            string dataString = "";
            foreach (Review review in reviewPage.Reviews)
            {
                dataString += review.User.Name;
                dataString += Specification.secondDelimiter;
                dataString += review.Rating;
                dataString += Specification.secondDelimiter;
                dataString += review.Text;

                dataString += Specification.delimiter;
            }

            dataString += Convert.ToInt32(reviewPage.HasNextPage);
            dataString += Specification.delimiter;
            dataString += Convert.ToInt32(reviewPage.HasPreviousPage);


            byte[] data = Encoding.UTF8.GetBytes(dataString);
            byte[] dataLength = BitConverter.GetBytes(data.Length);

            networkStreamHandler.Write(header); // Header
            networkStreamHandler.Write(cmd); // CMD
            networkStreamHandler.Write(dataLength); // Largo
            networkStreamHandler.Write(data); //data 

        }
    }
}
