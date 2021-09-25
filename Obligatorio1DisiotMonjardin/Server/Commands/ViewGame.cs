using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Text;

namespace Server
{
    internal class ViewGame : TextCommand
    {
        public ViewGame(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => throw new NotImplementedException();

        public override void ParsedRequestHandler(string[] req)
        {
            Steam Steam = Steam.GetInstance();
            int gameID = parseInt(req[0]);

            GameView gameView = Steam.ViewGame(gameID, networkStreamHandler);
            Respond(gameView);
        }

        private void Respond(GameView gameView)  //todo refactor (?
        {
            byte[] header = Encoding.UTF8.GetBytes(Specification.responseHeader);
            ushort command = (ushort)Command.BROWSE_CATALOGUE;
            byte[] cmd = BitConverter.GetBytes(command);

            string dataString = ""; // TODO sacar los indices y ponerlo en specification
            dataString += gameView.Game.Title; // 0
            dataString += Specification.delimiter;
            dataString += gameView.Game.Synopsis; // 1
            dataString += Specification.delimiter;
            dataString += gameView.Game.ReviewsRating; // 2
            dataString += Specification.delimiter;
            dataString += (int)gameView.Game.ESRBRating; // 3
            dataString += Specification.delimiter;
            dataString += gameView.Game.Genre;
            dataString += Specification.delimiter;
            dataString += Convert.ToInt32(gameView.IsOwned); // 4
            dataString += Specification.delimiter;
            dataString += Convert.ToInt32(gameView.IsPublisher); // 5

            byte[] data = Encoding.UTF8.GetBytes(dataString);
            byte[] dataLength = BitConverter.GetBytes(data.Length);

        }
    }
}