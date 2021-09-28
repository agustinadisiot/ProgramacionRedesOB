using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Text;

namespace Client
{
    public abstract class TextCommand : CommandHandler
    {
        public TextCommand(INetworkStreamHandler nwsh) : base(nwsh) { }


        public string Format(string[] parsedData)
        {
            string unParsedData = String.Join(Specification.FIRST_DELIMITER, parsedData);
            return unParsedData;
        }

        public string[] ParseByFirstDelimiter(string unparsedData)
        {
            string[] parsedData = unparsedData.Split(Specification.FIRST_DELIMITER);
            return parsedData;
        } //
          // poner en common(el server tambien la tiene)

        public string[] ParseBySecondDelimiter(string unparsedData)
        {
            string[] parsedData = unparsedData.Split(Specification.SECOND_DELIMITER);
            return parsedData;
        }

        protected void SendData(string data)
        {
            int dataLengthInBytes = Encoding.UTF8.GetBytes(data).Length;
            networkStreamHandler.WriteInt(dataLengthInBytes);
            networkStreamHandler.WriteString(data);
        }

        protected string[] GetData()
        {
            ReadHeader();
            ReadCommand();

            int dataLength = networkStreamHandler.ReadInt(Specification.DATA_SIZE_LENGTH);
            string data = networkStreamHandler.ReadString(dataLength);


            string[] parsedData = ParseByFirstDelimiter(data);
            return parsedData;
        }
    }
}
