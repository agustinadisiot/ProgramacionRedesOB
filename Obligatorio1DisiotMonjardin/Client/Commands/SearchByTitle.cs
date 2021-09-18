using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Common.Domain;

namespace Client.Commands
{
    public class SearchByTitle : TextCommand
    {
        public SearchByTitle(INetworkStreamHandler nwsh) : base(nwsh) { }


        public GamePage SendRequest(int pageNumber, string title )
        {
            SendHeader();
            SendCommand(Command.SEARCH_BY_TITLE);


            string pageNumberText = pageNumber.ToString();
            SendData(pageNumberText + Specification.delimiter +  title);
            return ResponseHandler(pageNumber);
        }


        private GamePage ResponseHandler(int pageNumber)
        {
           
            ReadHeader();
            ReadCommand(); // TODO ver si hacemos algo mas con estos 

            int dataLength = networkStreamHandler.ReadInt(Specification.dataSizeLength);
            string data = networkStreamHandler.ReadString(dataLength);


            string[] parsedData = Parse(data);
            List<string> gameTitles = parsedData.ToList();
            gameTitles.RemoveRange((parsedData.Length - 2), 2);

            GamePage gamePage = new GamePage()
            {
                GamesTitles = gameTitles,
                CurrentPage = pageNumber,
                HasNextPage = ToBooleanFromString(parsedData[parsedData.Length - 2]),
                HasPreviousPage = ToBooleanFromString(parsedData[parsedData.Length - 1])
            };

            return gamePage;
    
        }

        // TODO pasar a Common o a algun lado que  tambien lo pueda usar BrowseCatalog y SearchByX
        private bool ToBooleanFromString(string text)
        {
            return (text == "1");
        }


    }
}
