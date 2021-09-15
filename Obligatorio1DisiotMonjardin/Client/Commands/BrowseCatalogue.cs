using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using Server.SteamHelpers;
using System.Linq;

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

        public void SendRequest(int pageNumber)
        {
            SendHeader();
            SendCommand(Command.BROWSE_CATALOGUE);

            string pageNumberText = pageNumber.ToString();
            SendData(pageNumberText);
            ResponseHandler(pageNumber);
        }


        private void ResponseHandler(int pageNumber)
        {
            // conseguir respuesta

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

            ClientProgram.ShowCataloguePage(gamePage);
    
        }

        private bool ToBooleanFromString(string text)
        {
            return (text == "1");
        }

        private void Respond(GamePage gamePage)
        {
            byte[] header = Encoding.UTF8.GetBytes(Specification.responseHeader);
            ushort command = (ushort)Command.BROWSE_CATALOGUE;
            byte[] cmd = BitConverter.GetBytes(command);

            //byte[] data = Encoding.UTF8.GetBytes(info);
            //byte[] dataLength = BitConverter.GetBytes(data.Length);

            // TODO usar stringStream
            string dataString = "";
            foreach (string title in gamePage.GamesTitles)
            {
                dataString += title;
                dataString += Specification.delimiter;
            }
            dataString += Convert.ToInt32(gamePage.HasNextPage);
            dataString += Specification.delimiter;
            dataString += Convert.ToInt32(gamePage.HasPreviousPage);
            dataString += Specification.delimiter;



            byte[] data = Encoding.UTF8.GetBytes(dataString);
            byte[] dataLength = BitConverter.GetBytes(data.Length);

            _networkStreamHandler.Write(header); // Header
            Console.WriteLine(header.Length);
            _networkStreamHandler.Write(cmd); // CMD
            Console.WriteLine(cmd.Length);
            _networkStreamHandler.Write(dataLength); // Largo
            Console.WriteLine(BitConverter.ToInt32(dataLength));
            _networkStreamHandler.Write(data); //data 
            Console.WriteLine(Encoding.UTF8.GetString(data));

        }

    }
}
