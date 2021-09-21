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
            string unParsedData = String.Join(Specification.delimiter, parsedData);
            return unParsedData;
        }

        public string[] Parse(string unparsedData) // TODO capaz renombrar a ParsedByFirstDelimiter y cambiarle el nombre a delimiter
        {
            string[] parsedData = unparsedData.Split(Specification.delimiter);
            return parsedData;
        } // TODO poner en common(el server tambien la tiene)

        public string[] ParseBySecondDelimiter(string unparsedData)
        {
            string[] parsedData = unparsedData.Split(Specification.secondDelimiter);
            return parsedData;
        } // TODO poner en common(el server tambien la tiene)?

        protected void SendData(string data)
        {
            networkStreamHandler.WriteInt(data.Length);
            networkStreamHandler.WriteString(data);
        }

        protected string[] GetData()
        {
            ReadHeader();
            ReadCommand(); // TODO ver si hacemos algo mas con estos 

            int dataLength = networkStreamHandler.ReadInt(Specification.dataSizeLength);
            string data = networkStreamHandler.ReadString(dataLength);


            string[] parsedData = Parse(data);
            return parsedData;
        }
    }
}
