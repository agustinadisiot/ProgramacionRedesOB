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

        public string[] Parse(string unparsedData)
        {
            string[] parsedData = unparsedData.Split(Specification.delimiter);
            return parsedData;
        } // TODO poner en common(el server tambien la tiene)

        public abstract void ParsedRequestHandler(string[] req); // TODO sacar si no lo usamos en el cliente

        protected void SendData(string data)
        {
            _networkStreamHandler.WriteInt(data.Length);
            _networkStreamHandler.WriteString(data);
        }

        protected string[] GetData() {
            ReadHeader();
            ReadCommand(); // TODO ver si hacemos algo mas con estos 

            int dataLength = _networkStreamHandler.ReadInt(Specification.dataSizeLength);
            string data = _networkStreamHandler.ReadString(dataLength);


            string[] parsedData = Parse(data);
            return parsedData;
        }
    }
}
