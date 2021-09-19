using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Text;

namespace Server
{
    internal class ViewGame : TextCommand
    {
        public ViewGame(INetworkStreamHandler nwsh) : base(nwsh)
        {
        }

        public override void ParsedRequestHandler(string[] req)
        {
            Steam Steam = Steam.GetInstance();
            int gameID = int.Parse(req[0]);

            GameView gameView = Steam.ViewGame(gameID, networkStreamHandler); //ver que hacer en caso de juego en null
            Respond(gameView);
            Console.WriteLine("This is the game and actions");
        }

        private void Respond(GameView gameView)  //todo refactor (?
        {
            byte[] header = Encoding.UTF8.GetBytes(Specification.responseHeader);
            ushort command = (ushort)Command.BROWSE_CATALOGUE;
            byte[] cmd = BitConverter.GetBytes(command);

            // TODO usar stringStream
            string dataString = "";
            dataString += gameView.Game.Title;
            dataString += Specification.delimiter;
            dataString += gameView.Game.Synopsis;
            dataString += Specification.delimiter;
            dataString += gameView.Game.ReviewsRating;
            dataString += Specification.delimiter;
            dataString += gameView.Game.ESRBRating;
            dataString += Specification.delimiter;
            dataString += gameView.Game.Genre;
            dataString += Specification.delimiter;
            dataString += Convert.ToInt32(gameView.CanBuy);
            dataString += Specification.delimiter;
            dataString += Convert.ToInt32(gameView.IsPublisher);

            byte[] data = Encoding.UTF8.GetBytes(dataString);
            byte[] dataLength = BitConverter.GetBytes(data.Length);

            // debuging TODO eliminar
            networkStreamHandler.Write(header); // Header
            Console.WriteLine(header.Length);
            networkStreamHandler.Write(cmd); // CMD
            Console.WriteLine(cmd.Length);
            networkStreamHandler.Write(dataLength); // Largo
            Console.WriteLine(BitConverter.ToInt32(dataLength));
            networkStreamHandler.Write(data); //data 
            Console.WriteLine(Encoding.UTF8.GetString(data));

        }
    }
}