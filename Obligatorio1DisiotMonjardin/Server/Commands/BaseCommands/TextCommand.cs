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

            byte[] dataLength = networkStreamHandler.Read(Specification.dataSizeLength);
            int parsedLength = BitConverter.ToInt32(dataLength);

            byte[] data = networkStreamHandler.Read(parsedLength);
            string unparsedData = Encoding.UTF8.GetString(data);

            string[] parsedData = Parse(unparsedData);
            ParsedRequestHandler(parsedData);
        }

        public string[] Parse(string unparsedData)
        {
            string[] parsedData = unparsedData.Split(Specification.delimiter);
            return parsedData;
        }

        public abstract void ParsedRequestHandler(string[] req);

    }
}
