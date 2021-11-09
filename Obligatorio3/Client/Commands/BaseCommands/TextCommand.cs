using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Text;
using System.Threading.Tasks;

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
        }

        public string[] ParseBySecondDelimiter(string unparsedData)
        {
            string[] parsedData = unparsedData.Split(Specification.SECOND_DELIMITER);
            return parsedData;
        }

        protected async Task SendData(string data)
        {
            int dataLengthInBytes = Encoding.UTF8.GetBytes(data).Length;
            await networkStreamHandler.WriteInt(dataLengthInBytes);
            await networkStreamHandler.WriteString(data);
        }

        protected async Task<string[]> GetData()
        {
            await ReadHeader();
            await ReadCommand();

            int dataLength = await networkStreamHandler.ReadInt(Specification.DATA_SIZE_LENGTH);
            string data = await networkStreamHandler.ReadString(dataLength);


            string[] parsedData = ParseByFirstDelimiter(data);
            return parsedData;
        }
    }
}
