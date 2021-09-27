using Common.Domain;
using Common.NetworkUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.BusinessLogic
{
    public class BusinessLogicGameInfo
    {
        DataAccess db = DataAccess.GetInstance();

        public GameView ViewGame(int gameId, INetworkStreamHandler nwsh)
        {
            lock (db.Games)
            {
                Game game = GetGameById(gameId);
                User actualUser = GetUser(nwsh);
                GameView gameView = new GameView()
                {
                    Game = CreateGameCopy(game),
                    IsOwned = actualUser.GamesOwned.Contains(game),
                    IsPublisher = actualUser.Equals(game.Publisher),
                };

                return gameView;
            }
        }
        private Game CreateGameCopy(Game gameToCopy) // TODO sacar de steam
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
            List<Game> games = db.Games;
            lock (games)
            {
                Game gameToGetCover = games.Find(game => game.Id == gameId);
                return gameToGetCover.CoverFilePath; // TODO lock
            }

        }
        public bool BuyGame(int gameID, INetworkStreamHandler nwsh)
        {
            lock (db.Games)
            {
                Game gameToBuy = GetGameById(gameID); //esta en reviews
                User userToBuyGame = GetUser(nwsh);

                if (userToBuyGame.GamesOwned.Contains(gameToBuy))
                    return false;

                userToBuyGame.GamesOwned.Add(gameToBuy);
                return true;
            }
        }
    }
}
