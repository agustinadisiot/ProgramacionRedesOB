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
    public class BrowseCatalogue : TextCommand
    {
        public BrowseCatalogue(INetworkStreamHandler nwsh) : base(nwsh) { }


        public GamePage SendRequest(int pageNumber)
        {
            SendHeader();
            SendCommand(Command.BROWSE_CATALOGUE);

            string pageNumberText = pageNumber.ToString();
            SendData(pageNumberText);
            return ResponseHandler(pageNumber);
        }


        private GamePage ResponseHandler(int pageNumber)
        {
           
            ReadHeader();
            ReadCommand(); // TODO ver si hacemos algo mas con estos 

            int dataLength = networkStreamHandler.ReadInt(Specification.dataSizeLength);
            string data = networkStreamHandler.ReadString(dataLength);


            string[] parsedData = Parse(data);
            List<string> gamesInfo = parsedData.ToList();
            gamesInfo.RemoveRange((parsedData.Length - 2), 2);
            List<string> gamesTitles = gamesInfo.GetRange(0, gamesInfo.Count / 2);
            List<string> gameIDs = gamesInfo.GetRange(gamesInfo.Count / 2, gamesInfo.Count / 2);
            GamePage gamePage = new GamePage()
            {
                GamesTitles = gamesTitles,
                GamesIDs = gameIDs.Select(int.Parse).ToList(),
                CurrentPage = pageNumber,
                HasNextPage = ToBooleanFromString(parsedData[parsedData.Length - 2]),
                HasPreviousPage = ToBooleanFromString(parsedData[parsedData.Length - 1])
            };

            return gamePage;
    
        }

        private bool ToBooleanFromString(string text)
        {
            return (text == "1");
        }


    }
}
