using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public abstract class TextCommand : CommandHandler
    {
        public TextCommand(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override void HandleRequest()
        {
            //string unParseddata = networkStreamHandler.ReadString(Specification.dataSizeLength);
            //string[] parsedData = Parse(unParseddata);
            //ParsedRequestHandler(parsedData);
            
            //--------------
            byte[] dataLength = networkStreamHandler.Read(Specification.dataSizeLength);
            int parsedLength = BitConverter.ToInt32(dataLength);

            byte[] data = networkStreamHandler.Read(parsedLength);
            string unparsedData = Encoding.UTF8.GetString(data);

            string[] parsedData = Parse(unparsedData);
            try
            {
            ParsedRequestHandler(parsedData);
            }
            catch (ServerError e) {
                SendErrorToClient(e.message);
            }

        }

        private void SendErrorToClient(string message)
        {
            networkStreamHandler.WriteString(Specification.responseHeader);
            networkStreamHandler.WriteCommand(Command.ERROR);
            SendData(message); ;
        }

        private string[] Parse(string unparsedData)
        {
            string[] parsedData = unparsedData.Split(Specification.delimiter);
            return parsedData;
        }

        public abstract void ParsedRequestHandler(string[] req);

    }
}
