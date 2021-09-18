using Common.Domain;
using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Commands
{
    public class SearchByTitle : TextCommand
    {
        public SearchByTitle(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override void ParsedRequestHandler(string[] req)
        {
            Steam Steam = Steam.GetInstance();
            int pageNumber = int.Parse(req[0]);
            string title = req[1];
            GamePage gamePage = Steam.SearchByTitle(pageNumber,title);
            Respond(gamePage);
            Console.WriteLine("This is the game list");
        }

        private void Respond(GamePage gamePage)  //todo refactor
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



            byte[] data = Encoding.UTF8.GetBytes(dataString);
            byte[] dataLength = BitConverter.GetBytes(data.Length);

            // debuging TODO eliminar
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
