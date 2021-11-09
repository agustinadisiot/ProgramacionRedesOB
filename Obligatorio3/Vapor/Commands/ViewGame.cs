using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using Server.BusinessLogic;
using System;
using System.Threading.Tasks;

namespace Server
{
    public class ViewGame : TextCommand
    {
        public ViewGame(INetworkStreamHandler nwsh) : base(nwsh) { }

        public override Command cmd => Command.VIEW_GAME;

        public override async Task ParsedRequestHandler(string[] req)
        {
            BusinessLogicGameInfo GameInfo = BusinessLogicGameInfo.GetInstance();
            int gameId = parseInt(req[0]);

            GameView gameView = GameInfo.ViewGame(gameId, networkStreamHandler);
            await Respond(gameView);
        }

        private async Task Respond(GameView gameView)
        {
            await SendResponseHeader();

            string data = "";
            data += gameView.Game.Title;
            data += Specification.FIRST_DELIMITER;
            data += gameView.Game.Synopsis;
            data += Specification.FIRST_DELIMITER;
            data += gameView.Game.ReviewsRating;
            data += Specification.FIRST_DELIMITER;
            data += (int)gameView.Game.ESRBRating;
            data += Specification.FIRST_DELIMITER;
            data += gameView.Game.Genre;
            data += Specification.FIRST_DELIMITER;
            data += Convert.ToInt32(gameView.IsOwned);
            data += Specification.FIRST_DELIMITER;
            data += Convert.ToInt32(gameView.IsPublisher);

            await SendData(data);
        }
    }
}