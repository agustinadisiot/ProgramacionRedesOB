﻿using Common.Domain;
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
            games[gameId].UpdateReviewsRating(); // TODO capaz hacer automatica 
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
            User userToBuyGame = users.Find(user => user.Name == GetUsername(nwsh)); // TODO ver que pasa si no hay user logeado
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
            connections.Remove(nwsh);
            // TODO ver que retornamos
            return true;
        }

        private string GetUsername(INetworkStreamHandler nwsh)
        {
            string username;
            bool userLoggedIn = connections.TryGetValue(nwsh, out username);
            if (userLoggedIn)
                return username;
            else
                throw new Exception(); // TODO cambiar excepction
        }

        public GameView ViewGame(int gameId, INetworkStreamHandler nwsh)
        {
            Game game = games.Find(i => i.Id == gameId);
            User actualUser = GetUser(GetUsername(nwsh));
            GameView gameView = new GameView()
            {
                Game = game,
                IsOwned = actualUser.GamesOwned.Contains(game),
                IsPublisher = actualUser.Equals(game.Publisher),
            };

            return gameView;
        }


        private User GetUser(string username)
        {
            return users.Find(i => i.Name == username);
        }

        public void PublishGame(Game newGame, INetworkStreamHandler nwsh)
        {

            var gameWithSameTitle = games.Find(i => i.Title == newGame.Title);
            if (gameWithSameTitle != null)
                throw new Exception("Ya existe un juego con este titulo"); //TODO hacer la exception
            newGame.Id = this.gameId;
            gameId++;
            newGame.ReviewsRating = 0;
            newGame.Publisher = GetUser(GetUsername(nwsh));
            newGame.Reviews = new List<Review>();
            games.Add(newGame);
            Console.WriteLine("Game has been published with title: " + newGame.Title + " and id: " + newGame.Id);

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

        internal GamePage BrowseMyGames(int pageNumber, INetworkStreamHandler networkStreamHandler)
        {
            User CurrentUser = GetUser(GetUsername(networkStreamHandler));
            // TODO validar que el pageNumber >0
            return CreateGamePage(CurrentUser.GamesOwned, pageNumber);
        }
        internal GamePage SearchByTitle(int pageNumber, string title)
        {
            List<Game> filteredList = games.FindAll(game => textSearchIsMatch(game.Title, title));
            // TODO validar que el pageNumber >0
            return CreateGamePage(filteredList, pageNumber);
        }

        internal GamePage SearchByRating(int pageNumber, int minRating)
        {
            List<Game> filteredList = games.FindAll(game => game.ReviewsRating >= minRating);
            // TODO validar que el pageNumber >0
            return CreateGamePage(filteredList, pageNumber);
        }

        internal GamePage SearchByGenre(int pageNumber, string genre)
        {
            List<Game> filteredList = games.FindAll(game => game.Genre == genre);
            // TODO validar que el pageNumber >0
            return CreateGamePage(filteredList, pageNumber);
        }

        private GamePage CreateGamePage(List<Game> filteredList, int pageNumber)
        {
            int firstGamePos = (pageNumber - 1) * Specification.pageSize;
            int lastGamePos = firstGamePos + Specification.pageSize;
            List<string> gameTitles = new List<string>();
            List<int> gameIds = new List<int>();

            for (int i = firstGamePos; (i < filteredList.Count) && (i < lastGamePos); i++)
            {
                gameTitles.Add(filteredList[i].Title); //Todo checkear pagenumber > 0
                gameIds.Add(filteredList[i].Id);
            }
            GamePage ret = new GamePage()
            {
                GamesTitles = gameTitles,
                GamesIDs = gameIds,
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
