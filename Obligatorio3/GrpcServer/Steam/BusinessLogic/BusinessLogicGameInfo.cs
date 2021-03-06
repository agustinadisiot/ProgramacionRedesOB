using Common;
using Common.Domain;
using Common.NetworkUtils.Interfaces;
using System;
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
            BusinessLogicGameCRUD crud = BusinessLogicGameCRUD.GetInstance();
            BusinessLogicUtils utils = BusinessLogicUtils.GetInstance();
            lock (da.Games)
            {
                Game game = crud.GetGameById(gameId);
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
            BusinessLogicGameCRUD crud = BusinessLogicGameCRUD.GetInstance();
            BusinessLogicUtils utils = BusinessLogicUtils.GetInstance();
            lock (da.Games)
            {
                Game gameToBuy = crud.GetGameById(gameID);
                User userToBuyGame = utils.GetUser(nwsh);

                if (userToBuyGame.GamesOwned.Contains(gameToBuy))
                {
                    LogGameErrorBuyGameTwice(gameToBuy, userToBuyGame);
                    return false;
                }

                userToBuyGame.GamesOwned.Add(gameToBuy);
                LogGamePurchase(gameToBuy, userToBuyGame);

                return true;
            }
        }

        public bool AssociateGameToUser(int gameID, int userId)
        {
            BusinessLogicGameCRUD crud = BusinessLogicGameCRUD.GetInstance();
            BusinessLogicUtils utils = BusinessLogicUtils.GetInstance();
            lock (da.Games)
            {
                Game gameToBuy = crud.GetGameById(gameID);
                User userToBuyGame = utils.GetUser(userId);

                if (userToBuyGame.GamesOwned.Contains(gameToBuy))
                {
                    LogGameAssociationError(gameToBuy, userToBuyGame);
                    return false;
                }

                userToBuyGame.GamesOwned.Add(gameToBuy);
                LogGamePurchase(gameToBuy, userToBuyGame);
                return true;
            }
        }

        private void LogGamePurchase(Game game, User buyer)
        {
            Logger.Log(new LogRecord
            {
                GameName = game.Title,
                GameId = game.Id,
                Severity = LogRecord.InfoSeverity,
                UserId = buyer.Id,
                Username = buyer.Name,
                Message = $"El juego {game.Title} fue comprado por {buyer.Name}"
            });
        }

        private void LogGameAssociationError(Game game, User buyer)
        {
            Logger.Log(new LogRecord
            {
                GameName = game.Title,
                GameId = game.Id,
                Severity = LogRecord.WarningSeverity,
                UserId = buyer.Id,
                Username = buyer.Name,
                Message = $"El juego {game.Title} ya esta asociado al usuario {buyer.Name}"
            });
        }

        private void LogGameErrorBuyGameTwice(Game game, User buyer)
        {
            Logger.Log(new LogRecord
            {
                GameName = game.Title,
                GameId = game.Id,
                Severity = LogRecord.ErrorSeverity,
                UserId = buyer.Id,
                Username = buyer.Name,
                Message = $"El juego {game.Title} ya fue comprado por {buyer.Name}"
            });
        }

        internal bool ReturnGame(int idGame, int idUser)
        {
            BusinessLogicGameCRUD crud = BusinessLogicGameCRUD.GetInstance();
            BusinessLogicUtils utils = BusinessLogicUtils.GetInstance();
            lock (da.Games)
            {
                Game gameToReturn = crud.GetGameById(idGame);
                User userToReturnGame = utils.GetUser(idUser);

                return userToReturnGame.GamesOwned.Remove(gameToReturn);
            }
        }
    }
}
