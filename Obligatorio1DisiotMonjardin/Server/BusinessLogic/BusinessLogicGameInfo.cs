using Common.Domain;
using Common.NetworkUtils.Interfaces;
using System.Collections.Generic;

namespace Server.BusinessLogic
{
    public class BusinessLogicGameInfo
    {

        private DataAccess da;
        private static BusinessLogicGameInfo instance;

        private static readonly object singletonPadlock = new object();

        public static BusinessLogicGameInfo GetInstance()
        {
            lock (singletonPadlock)
            {

                if (instance == null)
                    instance = new BusinessLogicGameInfo();
            }
            return instance;
        }


        public BusinessLogicGameInfo()
        {
            da = DataAccess.GetInstance();
        }

        public GameView ViewGame(int gameId, INetworkStreamHandler nwsh)
        {
            BusinessLogicUtils utils = BusinessLogicUtils.GetInstance();
            lock (da.Games)
            {
                Game game = utils.GetGameById(gameId);
                User actualUser = utils.GetUser(nwsh);
                GameView gameView = new GameView()
                {
                    Game = CreateGameCopy(game),
                    IsOwned = actualUser.GamesOwned.Contains(game),
                    IsPublisher = actualUser.Equals(game.Publisher),
                };

                return gameView;
            }
        }
        private Game CreateGameCopy(Game gameToCopy)
        {
            return new Game
            {
                Title = gameToCopy.Title,
                Synopsis = gameToCopy.Synopsis,
                Genre = gameToCopy.Genre,
                ESRBRating = gameToCopy.ESRBRating,
                ReviewsRating = gameToCopy.ReviewsRating
            };


        }
        public string GetCoverPath(int gameId)
        {
            List<Game> games = da.Games;
            lock (games)
            {
                Game gameToGetCover = games.Find(game => game.Id == gameId);
                return gameToGetCover.CoverFilePath;
            }

        }
        public bool BuyGame(int gameID, INetworkStreamHandler nwsh)
        {
            BusinessLogicUtils utils = BusinessLogicUtils.GetInstance();
            lock (da.Games)
            {
                Game gameToBuy = utils.GetGameById(gameID);
                User userToBuyGame = utils.GetUser(nwsh);

                if (userToBuyGame.GamesOwned.Contains(gameToBuy))
                    return false;

                userToBuyGame.GamesOwned.Add(gameToBuy);
                return true;
            }
        }
    }
}
