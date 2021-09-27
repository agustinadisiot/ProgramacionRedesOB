using Common;
using Common.Domain;
using Common.Utils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Server
{
    public class Steam
    {
        private DataAccess db;
        private static Steam instance;

        private static readonly object singletonPadlock = new object();

        public static Steam GetInstance()
        {
            lock (singletonPadlock)
            {

                if (instance == null)
                    instance = new Steam();
            }
            return instance;
        }


        public Steam()
        {
            db = new DataAccess();
        }
        //TODO organizar funciones 

        // REVIEWS 
        public string WriteReview(Review newReview, int gameId, INetworkStreamHandler nwsh)
        {

            newReview.Author = GetUser(nwsh);
            List<Game> games = db.Games;
            lock (games)
            {
                Game gameToAddReview = games[gameId];
                lock (gameToAddReview.Reviews)
                {
                    gameToAddReview.Reviews.Add(newReview);
                    gameToAddReview.UpdateReviewsRating();
                    return (@$"Clasificación por {newReview.Author.Name} para el juego {gameToAddReview.Title}
                    fue publicada correctamente");
                }
            }
        }

        public ReviewPage BrowseReviews(int pageNumber, int gameId)
        {
            if (pageNumber <= 0)
                throw new ServerError($"Número de página {pageNumber} no válido");
            lock (db.Games)
            {
                Game gameToGetReviews = GetGameById(gameId);
                lock (gameToGetReviews.Reviews)
                {
                    List<Review> allReviews = gameToGetReviews.Reviews;

                    int firstReviewPos = (pageNumber - 1) * Specification.PAGE_SIZE;
                    int pageSize;
                    if (firstReviewPos + Specification.PAGE_SIZE > allReviews.Count)
                        pageSize = allReviews.Count - firstReviewPos;
                    else
                        pageSize = Specification.PAGE_SIZE;


                    List<Review> reviewsInPage = allReviews.GetRange(firstReviewPos, pageSize);

                    ReviewPage ret = new ReviewPage()
                    {
                        Reviews = reviewsInPage,
                        HasNextPage = ExistsNextPage(allReviews, pageNumber),
                        HasPreviousPage = pageNumber > 1
                    };
                    return ret;
                }
            }
        }

        // PRE: Requires lock on db.Games TODO ver si esta bien poner esto
        private Game GetGameById(int gameId)
        {
            Game gameFound = db.Games.Find(game => game.Id == gameId);
            if (gameFound == null)
                throw new ServerError($"{gameId} No es una id de juego válida");
            return gameFound;
        }


        // SESSION
        public bool Login(string newUserName, INetworkStreamHandler nwsh)
        {
            List<User> users = db.Users;
            lock (users)
            {
                User newUser = new User(newUserName);
                bool alreadyExists = users.Contains(newUser);
                if (!alreadyExists)
                {
                    users.Add(newUser);
                }
                db.Connections.Add(nwsh, newUserName);

                return !alreadyExists;
            }
        }

        public bool Logout(INetworkStreamHandler nwsh)
        {
            var connections = db.Connections;
            lock (connections)
            {
                return connections.Remove(nwsh);
            }
        }

        // UTILS 
        private User GetUser(INetworkStreamHandler nwsh)
        {
            List<User> users = db.Users;
            lock (users)
            {
                string username = GetUsername(nwsh);
                User actualUser = users.Find(i => i.Name == username);
                if (actualUser == null) throw new ServerError("No se encontró el usuario, rehacer login");
                return users.Find(i => i.Name == username);
            }
        }
        private string GetUsername(INetworkStreamHandler nwsh)
        {
            var connections = db.Connections;
            lock (connections)
            {
                bool userLoggedIn = connections.TryGetValue(nwsh, out string username);
                if (userLoggedIn)
                    return username;
                else
                    throw new ServerError("No existe usuario con ese nombre");
            }
        }

        // GAME INFO  ????
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
                Game gameToBuy = GetGameById(gameID);
                User userToBuyGame = GetUser(nwsh);

                if (userToBuyGame.GamesOwned.Contains(gameToBuy))
                    return false;

                userToBuyGame.GamesOwned.Add(gameToBuy);
                return true;
            }
        }

        // GAME CUD
        public string PublishGame(Game newGame, INetworkStreamHandler nwsh)
        {
            //VerifyGame(newGame); todo
            List<Game> games = db.Games;
            lock (games)
            {
                var gameWithSameTitle = games.Find(i => i.Title == newGame.Title);
                if (gameWithSameTitle != null)
                    throw new TitleAlreadyExistsException();
                newGame.Id = db.NextGameID;
                newGame.ReviewsRating = 0;
                newGame.Publisher = GetUser(nwsh);
                newGame.Reviews = new List<Review>();
                games.Add(newGame);
                return $"Se publicó el juego {newGame.Title} correctamente";
            }
        }
        private void VerifyGame(Game newGame)
        {
            throw new NotImplementedException(); //TODO 
        }

        public string ModifyGame(int gameToModId, Game modifiedGame)
        {
            List<Game> games = db.Games;
            lock (games)
            {
                Game gameToMod = GetGameById(gameToModId);

                if (modifiedGame.Title != "")
                {
                    var gameWithSameTitle = games.Find(i => (i.Title == modifiedGame.Title) && (i.Id != gameToModId));
                    if (gameWithSameTitle != null)
                        throw new TitleAlreadyExistsException();
                    if (!Validation.isValidTitle(modifiedGame.Title))
                        throw new ServerError("Título no válido");
                    gameToMod.Title = modifiedGame.Title;
                }

                if (modifiedGame.Synopsis != "")
                {
                    if (!Validation.isValidSynopsis(modifiedGame.Synopsis))
                        throw new ServerError("Sinopsis no válida");
                    gameToMod.Synopsis = modifiedGame.Synopsis;
                }
                if (modifiedGame.ESRBRating != Common.ESRBRating.EmptyESRB)
                {
                    if (!Validation.isValidESRBRating((int)modifiedGame.ESRBRating))
                        throw new ServerError("Clasificación ESRB no válida");
                    gameToMod.ESRBRating = modifiedGame.ESRBRating;
                }

                if (modifiedGame.Genre != "")
                {
                    if (!Validation.isValidGenre(modifiedGame.Genre))
                        throw new ServerError("Genero no válido");
                    gameToMod.Genre = modifiedGame.Genre;
                }

                if (modifiedGame.CoverFilePath != null)
                {
                    string pathToDelete = gameToMod.CoverFilePath;
                    gameToMod.CoverFilePath = modifiedGame.CoverFilePath;
                    File.Delete(pathToDelete); // TODO capaz no tendria que ir en Steam TODO lock
                }
                return $"Se modificó el juego {gameToMod.Title} correctamente";
            }
        }

        public bool DeleteGame(int gameId)
        {
            List<Game> games = db.Games;
            lock (games)
            {
                return games.Remove(games.Find(i => i.Id == gameId));
                // TODO eliminar caratula
            }
        }


        // GAME PAGE

        // PRE: Requires lock on filteredList TODO ver si esta bien poner esto
        private GamePage CreateGamePage(List<Game> filteredList, int pageNumber)
        {
            if (pageNumber <= 0)
                throw new ServerError("Número de Página no válido");

            int firstGamePos = (pageNumber - 1) * Specification.PAGE_SIZE;
            int lastGamePos = firstGamePos + Specification.PAGE_SIZE;
            List<string> gameTitles = new List<string>();
            List<int> gameIds = new List<int>();

            for (int i = firstGamePos; (i < filteredList.Count) && (i < lastGamePos); i++)
            {
                gameTitles.Add(filteredList[i].Title);
                gameIds.Add(filteredList[i].Id);
            }
            GamePage ret = new GamePage()
            {
                GamesTitles = gameTitles,
                GamesIds = gameIds,
                HasNextPage = ExistsNextPage(filteredList, pageNumber),
                HasPreviousPage = pageNumber > 1
            };
            return ret;
        }

        public GamePage BrowseGames(int pageNumber)
        {
            List<Game> games = db.Games;
            lock (games)
            {
                return CreateGamePage(games, pageNumber);
            }
        }

        public GamePage BrowseMyGames(int pageNumber, INetworkStreamHandler nwsh)
        {
            User CurrentUser = GetUser(nwsh);
            List<Game> games = db.Games;
            lock (games)
            {
                return CreateGamePage(CurrentUser.GamesOwned, pageNumber);
            }
        }

        public GamePage SearchByTitle(int pageNumber, string title)
        {
            List<Game> games = db.Games;
            lock (games)
            {
                List<Game> filteredList = games.FindAll(game => TextSearchIsMatch(game.Title, title));
                return CreateGamePage(filteredList, pageNumber);
            }
        }

        public GamePage SearchByRating(int pageNumber, int minRating)
        {
            List<Game> games = db.Games;
            lock (games)
            {
                List<Game> filteredList = games.FindAll(game => game.ReviewsRating >= minRating);
                return CreateGamePage(filteredList, pageNumber);
            }
        }

        public GamePage SearchByGenre(int pageNumber, string genre)
        {
            List<Game> games = db.Games;
            lock (games)
            {
                List<Game> filteredList = games.FindAll(game => game.Genre == genre);
                return CreateGamePage(filteredList, pageNumber);
            }
        }

        private bool TextSearchIsMatch(string real, string search)
        {
            return (real.ToLower().Contains(search.ToLower()));
        }

        private bool ExistsNextPage<T>(List<T> fullList, int pageNumber)
        {
            int maxPageNumber = fullList.Count / Specification.PAGE_SIZE;
            if (fullList.Count % Specification.PAGE_SIZE != 0)
                maxPageNumber++;

            return pageNumber < maxPageNumber;
        }
    }
}
