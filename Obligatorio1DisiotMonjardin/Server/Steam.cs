using Common.Domain;
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
        private List<Game> games;
        private List<User> users;
        private static Steam instance;
        private static readonly object singletonPadlock = new object();
        private int gameId;
        private Dictionary<INetworkStreamHandler, string> connections;


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
            games = new List<Game>();
            users = new List<User>();
            gameId = 0;
            connections = new Dictionary<INetworkStreamHandler, string>();
        }

        internal string WriteReview(Review newReview, int gameId, INetworkStreamHandler nwsh)
        {
            newReview.User = GetUser(nwsh);
            Game gameToAddReview = games[gameId];
            gameToAddReview.Reviews.Add(newReview);
            gameToAddReview.UpdateReviewsRating();
            return (@$"Clasificación por {newReview.User.Name} para el juego {gameToAddReview.Title}
                    fue publicada correctamente");

        }


        internal ReviewPage BrowseReviews(int pageNumber, int gameId) // TODO ver si usamos public o internal en cada uno
        {
            if (pageNumber <= 0)
                throw new ServerError($"Número de página {pageNumber} no válido");
            Game gameToGetReviews = GetGameById(gameId);
            List<Review> allReviews = gameToGetReviews.Reviews;

            int firstReviewPos = (pageNumber - 1) * Specification.pageSize;
            int pageSize;
            if (firstReviewPos + Specification.pageSize > allReviews.Count)
                pageSize = allReviews.Count - firstReviewPos;
            else
                pageSize = Specification.pageSize;


            List<Review> reviewsInPage = allReviews.GetRange(firstReviewPos, pageSize); // TODO usar esto para BrowseCatalogue y search

            ReviewPage ret = new ReviewPage()
            {
                Reviews = reviewsInPage,
                HasNextPage = ExistsNextPage(allReviews, pageNumber),
                HasPreviousPage = pageNumber > 1
            };
            return ret;
        }

        private Game GetGameById(int gameId)
        {
            Game gameFound = games.Find(game => game.Id == gameId);
            if (gameFound == null)
                throw new ServerError($"{gameId} No es una id de juego válida");
            return gameFound;
        }

        internal bool BuyGame(int gameID, INetworkStreamHandler nwsh)
        {
            Game gameToBuy = GetGameById(gameID);
            User userToBuyGame = GetUser(nwsh);

            if (userToBuyGame.GamesOwned.Contains(gameToBuy))
                return false;

            userToBuyGame.GamesOwned.Add(gameToBuy);
            return true;
        }

        public bool Login(string newUserName, INetworkStreamHandler nwsh)
        {
            User newUser = new User(newUserName);
            bool alreadyExists = users.Contains(newUser);
            if (!alreadyExists)
            {
                users.Add(newUser);
            }
            connections.Add(nwsh, newUserName);

            return !alreadyExists;
        }

        public bool Logout(INetworkStreamHandler nwsh)
        {
            return connections.Remove(nwsh);
        }

        private string GetUsername(INetworkStreamHandler nwsh)
        {
            bool userLoggedIn = connections.TryGetValue(nwsh, out string username);
            if (userLoggedIn)
                return username;
            else
                throw new ServerError("No existe usuario con ese nombre");
        }

        public GameView ViewGame(int gameId, INetworkStreamHandler nwsh)
        {
            Game game = GetGameById(gameId);
            User actualUser = GetUser(nwsh);
            GameView gameView = new GameView()
            {
                Game = game,
                IsOwned = actualUser.GamesOwned.Contains(game),
                IsPublisher = actualUser.Equals(game.Publisher),
            };

            return gameView;
        }

        private User GetUser(INetworkStreamHandler nwsh)
        {
            string username = GetUsername(nwsh);
            User actualUser = users.Find(i => i.Name == username);
            if (actualUser == null) throw new ServerError("No se encontró el usuario, rehacer login");
            return users.Find(i => i.Name == username);
        }

        public string PublishGame(Game newGame, INetworkStreamHandler nwsh)
        {
            var gameWithSameTitle = games.Find(i => i.Title == newGame.Title);
            if (gameWithSameTitle != null)
                throw new TitleAlreadyExistseException();
            newGame.Id = this.gameId;
            gameId++;
            newGame.ReviewsRating = 0;
            newGame.Publisher = GetUser(nwsh);
            newGame.Reviews = new List<Review>();
            games.Add(newGame);
            return $"Se publicó el juego {newGame.Title} correctamente";
        }


        internal string ModifyGame(int gameToModId, Game modifiedGame)
        {
            Game gameToMod = GetGameById(gameToModId);
            if (modifiedGame.Title != "") {
                gameToMod.Title = modifiedGame.Title;
                var gameWithSameTitle = games.Find(i => (i.Title == gameToMod.Title) && (i.Id != gameToModId));
                if (gameWithSameTitle != null)
                    throw new TitleAlreadyExistseException();
            }
            if (modifiedGame.Synopsis != "") gameToMod.Synopsis = modifiedGame.Synopsis;
            if(modifiedGame.ESRBRating != Common.ESRBRating.EmptyESRB) gameToMod.ESRBRating = modifiedGame.ESRBRating;
            if(modifiedGame.Genre != "") gameToMod.Genre = modifiedGame.Genre;
            if (modifiedGame.CoverFilePath != null) {
                string pathToDelete = gameToMod.CoverFilePath;
                gameToMod.CoverFilePath = modifiedGame.CoverFilePath;
                File.Delete(pathToDelete); // TODO capaz no tendria que ir en Steam
            }
            return $"Se modificó el juego {gameToMod.Title} correctamente";
        }

        public bool DeleteGame(int gameId)
        {
            // TODO hacer lock en la lista 
            return games.Remove(games.Find(i => i.Id == gameId));
        }


        public string GetCoverPath(int gameId)
        {
            Game gameToGetCover = games.Find(game => game.Id == gameId);
            return gameToGetCover.CoverFilePath;

        }

        public GamePage BrowseGames(int pageNumber)
        {
            // TODO validar que el pageNumber >0
            return CreateGamePage(games, pageNumber);
        }

        internal GamePage BrowseMyGames(int pageNumber, INetworkStreamHandler nwsh)
        {
            User CurrentUser = GetUser(nwsh);
            return CreateGamePage(CurrentUser.GamesOwned, pageNumber);
        }
        internal GamePage SearchByTitle(int pageNumber, string title)
        {
            List<Game> filteredList = games.FindAll(game => TextSearchIsMatch(game.Title, title));
            return CreateGamePage(filteredList, pageNumber);
        }

        internal GamePage SearchByRating(int pageNumber, int minRating)
        {
            List<Game> filteredList = games.FindAll(game => game.ReviewsRating >= minRating);
            return CreateGamePage(filteredList, pageNumber);
        }

        internal GamePage SearchByGenre(int pageNumber, string genre)
        {
            List<Game> filteredList = games.FindAll(game => game.Genre == genre);
            return CreateGamePage(filteredList, pageNumber);
        }

        private GamePage CreateGamePage(List<Game> filteredList, int pageNumber)
        {
            if (pageNumber <= 0)
                throw new ServerError("Número de Página no valido");

            int firstGamePos = (pageNumber - 1) * Specification.pageSize;
            int lastGamePos = firstGamePos + Specification.pageSize;
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
        private bool TextSearchIsMatch(string real, string search)
        {
            return (real.ToLower().Contains(search.ToLower()));
        }

        private bool ExistsNextPage<T>(List<T>fullList, int pageNumber)
        {
            int maxPageNumber = fullList.Count / Specification.pageSize;
            if (fullList.Count % Specification.pageSize != 0)
                maxPageNumber++;

            return pageNumber < maxPageNumber;
        }
    }
}
