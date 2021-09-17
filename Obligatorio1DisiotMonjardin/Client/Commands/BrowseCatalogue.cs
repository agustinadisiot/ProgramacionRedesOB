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

       public override void ParsedRequestHandler(string[] req)
        {
            //TODO
            /*Steam Steam = Steam.GetInstance();
            int pageNumber = int.Parse(req[0]);
            GamePage gamePage = Steam.BrowseGames(pageNumber);
            Respond(gamePage);
            Console.WriteLine("This is the game list");*/
        }

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

            int dataLength = _networkStreamHandler.ReadInt(Specification.dataSizeLength);
            string data = _networkStreamHandler.ReadString(dataLength);


            string[] parsedData = Parse(data);
            List<string> gameTitles = parsedData.ToList();
            gameTitles.RemoveRange((parsedData.Length - 2), 2);

            GamePage gamePage = new GamePage()
            {
                GamesTitles = gameTitles.ToArray(),
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
