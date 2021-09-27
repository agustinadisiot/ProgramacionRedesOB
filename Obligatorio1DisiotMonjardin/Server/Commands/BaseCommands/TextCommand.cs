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
            byte[] dataLength = networkStreamHandler.Read(Specification.DATA_SIZE_LENGTH);
            int parsedLength = BitConverter.ToInt32(dataLength);

            byte[] data = networkStreamHandler.Read(parsedLength);
            string unparsedData = Encoding.UTF8.GetString(data);

            string[] parsedData = Parse(unparsedData);
            try
            {
                ParsedRequestHandler(parsedData);
            }
            catch (ServerError e)
            {
                SendErrorToClient(e.Message);
            }

        }

        private void SendErrorToClient(string message)
        {
            networkStreamHandler.WriteString(Specification.RESPONSE_HEADER);
            networkStreamHandler.WriteCommand(Command.ERROR);
            SendData(message);
        }

        private string[] Parse(string unparsedData)
        {
            string[] parsedData = unparsedData.Split(Specification.FIRST_DELIMITER);
            return parsedData;
        }

        protected int parseInt(string unparsedInt)
        {
            int result;
            bool parseSuccessful = int.TryParse(unparsedInt, out result);
            if (!parseSuccessful)
                throw new ServerError($"Se esperaba un numero pero no se recibió {unparsedInt}");
            return result;
        }

        public abstract void ParsedRequestHandler(string[] req);

    }
}
