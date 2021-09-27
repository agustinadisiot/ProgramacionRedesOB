using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Text;

namespace Server
{
    public class ViewGame : TextCommand
    {
        public ViewGame(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.VIEW_GAME;

        public override void ParsedRequestHandler(string[] req)
        {
            Steam Steam = Steam.GetInstance();
            int gameId = parseInt(req[0]);

            GameView gameView = Steam.ViewGame(gameId, networkStreamHandler);
            Respond(gameView);
        }

        private void Respond(GameView gameView)
        {
            SendResponseHeader();

            string data = ""; // TODO sacar los indices y ponerlo en specification
            data += gameView.Game.Title; // 0
            data += Specification.FIRST_DELIMITER;
            data += gameView.Game.Synopsis; // 1
            data += Specification.FIRST_DELIMITER;
            data += gameView.Game.ReviewsRating; // 2
            data += Specification.FIRST_DELIMITER;
            data += (int)gameView.Game.ESRBRating; // 3
            data += Specification.FIRST_DELIMITER;
            data += gameView.Game.Genre;
            data += Specification.FIRST_DELIMITER;
            data += Convert.ToInt32(gameView.IsOwned); // 4
            data += Specification.FIRST_DELIMITER;
            data += Convert.ToInt32(gameView.IsPublisher); // 5

            SendData(data);
        }
    }
}