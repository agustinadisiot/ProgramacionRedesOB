using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public string[] Parse(string unparsedData) // TODO capaz renombrar a ParsedByFirstDelimiter y cambiarle el nombre a delimiter
        {
            string[] parsedData = unparsedData.Split(Specification.FIRST_DELIMITER);
            return parsedData;
        } // TODO poner en common(el server tambien la tiene)

        public string[] ParseBySecondDelimiter(string unparsedData)
        {
            string[] parsedData = unparsedData.Split(Specification.SECOND_DELIMITER);
            return parsedData;
        } // TODO poner en common(el server tambien la tiene)?

        protected void SendData(string data)
        {
            int dataLengthInBytes = Encoding.UTF8.GetBytes(data).Length;
            networkStreamHandler.WriteInt(dataLengthInBytes);
            networkStreamHandler.WriteString(data);
        }

        protected string[] GetData()
        {
            ReadHeader();
            ReadCommand(); // TODO ver si hacemos algo mas con estos 

            int dataLength = networkStreamHandler.ReadInt(Specification.DATA_SIZE_LENGTH);
            string data = networkStreamHandler.ReadString(dataLength);


            string[] parsedData = Parse(data);
            return parsedData;
        }
    }
}
