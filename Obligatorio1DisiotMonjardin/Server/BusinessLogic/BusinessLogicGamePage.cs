using Common.Domain;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.BusinessLogic
{
    public class BusinessLogicGamePage
    {
        DataAccess db = DataAccess.GetInstance();


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
