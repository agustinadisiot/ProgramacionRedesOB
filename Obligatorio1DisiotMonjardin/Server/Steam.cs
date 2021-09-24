using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class Steam
    {
        private List<Game> games;
        private List<User> users;
        private static Steam instance;
        private int gameId;
        private Dictionary<INetworkStreamHandler, string> connections;

        public static Steam GetInstance()
        {
            if (instance == null)
                instance = new Steam();
            return instance;
        }

        

        public Steam()
        {
            games = new List<Game>();
            users = new List<User>();
            gameId = 0;
            connections = new Dictionary<INetworkStreamHandler, string>();
        }

        internal void WriteReview(Review newReview, int gameId, INetworkStreamHandler nwsh)
        {
            newReview.User = GetUser(GetUsername(nwsh));
            games[gameId].Reviews.Add(newReview);
            Console.WriteLine($"Review by {newReview.User.Name} has been published");

        }
        internal ReviewPage BrowseReviews(int pageNumber, int gameId) // TODO ver si usamos public o internal en cada uno
        {
            Game gameToGetReviews = games.Find(game => game.Id == gameId);
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
                HasNextPage = ExistsNextReviewPage(allReviews, pageNumber), // TODO podria usar el if del PageSize
                HasPreviousPage = pageNumber > 1
            };
            return ret;
        }
        public bool ExistsNextReviewPage(List<Review> reviews, int pageNumber) // TODO usar la misma funcion que para ExistsNextGamePage
        {
            int maxPageNumber = reviews.Count / Specification.pageSize;
            if (reviews.Count % Specification.pageSize != 0)
                maxPageNumber++;

            return pageNumber < maxPageNumber;
        }

        internal void BuyGame(int gameID, INetworkStreamHandler nwsh)
        {
            // TODO tirar error si el juego ya esta comprado / si la id no es valida
            Game gameToBuy = games.Find(game => game.Id == gameID);
            User userToBuyGame = GetUser(GetUsername(nwsh));
            userToBuyGame.GamesOwned.Add(gameToBuy);
            //TODO eliminar
            Console.WriteLine($" {userToBuyGame.Name} bought {gameToBuy.Title} ");
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
                throw new Exception("No existe usuario con ese nombre"); 
        }

        public GameView ViewGame(int gameId, INetworkStreamHandler nwsh)
        {
            Game game = games.Find(i => i.Id == gameId);
            User actualUser = GetUser(GetUsername(nwsh));
            game.ReviewsRating = GetReviewsAvarageRating(game);
            GameView gameView = new GameView()
            {
                Game = game,
                IsOwned = actualUser.GamesOwned.Contains(game),
                IsPublisher = actualUser.Equals(game.Publisher),
            };

            return gameView;
        }

       

        private int GetReviewsAvarageRating(Game game)
        {
            // TODO lock
            decimal total = 0;
            decimal count = game.Reviews.Count;
            foreach (Review review in game.Reviews)
            {
                total += review.Rating;
            }
            decimal result;
            if (count > 0)
                result = total / count;
            else
                result = 0;
            return (int)Math.Ceiling(result);
        }

        private User GetUser(string username)
        {
            User actualUser = users.Find(i => i.Name == username);
            if (actualUser == null) throw new Exception("No se encontro usuario, rehacer login");
            return users.Find(i => i.Name == username);
        }

        public void PublishGame(Game newGame, INetworkStreamHandler nwsh)
        {

            var gameWithSameTitle = games.Find(i => i.Title == newGame.Title);
            if (gameWithSameTitle != null)
                throw new Exception("Ya existe un juego con este titulo"); 
            newGame.Id = this.gameId;
            gameId++;
            newGame.ReviewsRating = 0;
            newGame.Publisher = GetUser(GetUsername(nwsh));
            newGame.Reviews = new List<Review>();
            games.Add(newGame);
            Console.WriteLine("Game has been published with title: " + newGame.Title + " and id: " + newGame.Id);

        }

       
        internal void ModifyGame(int gameToModId, Game modifiedGame)
        {
            Game gameToMod = games.Find(i => i.Id == gameToModId);
            if (modifiedGame.Title != "") gameToMod.Title = modifiedGame.Title;
            if (modifiedGame.Synopsis != "") gameToMod.Synopsis = modifiedGame.Synopsis;
            gameToMod.ESRBRating = modifiedGame.ESRBRating;
            gameToMod.Genre = modifiedGame.Genre;
            if (modifiedGame.CoverFilePath != null) gameToMod.CoverFilePath = modifiedGame.CoverFilePath;
            Console.WriteLine(gameToMod.CoverFilePath);
            Console.WriteLine("Game has been modified with title: " + gameToMod.Title + " and id: " + gameToMod.Id);

        }

        public bool DeleteGame(int gameId)
        {
            return games.Remove(games.Find(i => i.Id == gameId));
        }


        public string GetCoverPath(int gameId)
        {
            Game gameToGetCover = games.Find(game => game.Id == gameId);
            return gameToGetCover.CoverFilePath;

        }

        public GamePage BrowseGames(int pageNumber)
        {
            int firstGamePos = (pageNumber - 1) * Specification.pageSize;
            int lastGamePos = firstGamePos + Specification.pageSize;
            List<string> gameTitles = new List<string>();
            List<int> gamesIDs = new List<int>();

            for (int i = firstGamePos; (i < games.Count) && (i < lastGamePos); i++)
            {
                gameTitles.Add(games[i].Title); //Todo checkear pagenumber > 0
                gamesIDs.Add(games[i].Id);
            }
            GamePage ret = new GamePage()
            {
                GamesTitles = gameTitles,
                GamesIDs = gamesIDs,
                HasNextPage = ExistsNextGamePage(games, pageNumber),
                HasPreviousPage = pageNumber > 1
            };
            return ret;
        }

        internal GamePage SearchByTitle(int pageNumber, string title)
        {
            int firstGamePos = (pageNumber - 1) * Specification.pageSize;
            int lastGamePos = firstGamePos + Specification.pageSize;
            List<Game> filteredList = games.FindAll(game => textSearchIsMatch(game.Title, title));
            List<string> gameTitles = new List<string>();

            for (int i = firstGamePos; (i < filteredList.Count) && (i < lastGamePos); i++)
            {
                gameTitles.Add(filteredList[i].Title); //Todo checkear pagenumber > 0
            }
            GamePage ret = new GamePage()
            {
                GamesTitles = gameTitles,
                HasNextPage = ExistsNextGamePage(filteredList, pageNumber),
                HasPreviousPage = pageNumber > 1
            };
            return ret;
        }

        bool textSearchIsMatch(string real, string search)
        {
            return (real.ToLower().Contains(search.ToLower()));
        }

        public bool ExistsNextGamePage(List<Game> games, int pageNumber)
        {
            int maxPageNumber = games.Count / Specification.pageSize;
            if (games.Count % Specification.pageSize != 0)
                maxPageNumber++;

            return pageNumber < maxPageNumber;
        }
    }
}
