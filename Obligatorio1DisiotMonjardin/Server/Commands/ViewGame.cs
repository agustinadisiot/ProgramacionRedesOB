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

        public override Command cmd => Command.VIEW_GAME;

        public override void ParsedRequestHandler(string[] req)
        {
            Steam Steam = Steam.GetInstance();
            int gameID = parseInt(req[0]);

            GameView gameView = Steam.ViewGame(gameID, networkStreamHandler);
            Respond(gameView);
        }

        private void Respond(GameView gameView)  
        {
            SendResponseHeader();

            string data = ""; // TODO sacar los indices y ponerlo en specification
            data += gameView.Game.Title; // 0
            data += Specification.delimiter;
            data += gameView.Game.Synopsis; // 1
            data += Specification.delimiter;
            data += gameView.Game.ReviewsRating; // 2
            data += Specification.delimiter;
            data += (int)gameView.Game.ESRBRating; // 3
            data += Specification.delimiter;
            data += gameView.Game.Genre;
            data += Specification.delimiter;
            data += Convert.ToInt32(gameView.IsOwned); // 4
            data += Specification.delimiter;
            data += Convert.ToInt32(gameView.IsPublisher); // 5

            SendData(data);
        }
    }
}