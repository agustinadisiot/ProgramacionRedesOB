using Common.Domain;
using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Client.Commands
{
    public class WriteReview : TextCommand
    {
        public WriteReview(INetworkStreamHandler nwsh) : base(nwsh) { }

        public string SendRequest(Review newReview, int gameId)
        {
            SendHeader();
            SendCommand(Command.WRITE_REVIEW);

            string data = "";
            data += gameId;
            data += Specification.delimiter;
            data += newReview.Rating;
            data += Specification.delimiter;
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
