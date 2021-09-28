using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using Common.Utils;
using System.Collections.Generic;

namespace Server.BusinessLogic
{
    public class BusinessLogicGamePage
    {

        private DataAccess da;
        private static BusinessLogicGamePage instance;

        private static readonly object singletonPadlock = new object();

        public static BusinessLogicGamePage GetInstance()
        {
            lock (singletonPadlock)
            {

                if (instance == null)
                    instance = new BusinessLogicGamePage();
            }
            return instance;
        }


        public BusinessLogicGamePage()
        {
            da = DataAccess.GetInstance();
        }

        // PRE: Requires lock on filteredList 
        private GamePage CreateGamePage(List<Game> filteredList, int pageNumber)
        {
            BusinessLogicUtils utils = BusinessLogicUtils.GetInstance();
            if (pageNumber <= 0)
                throw new ServerError("Número de Página no válido");

            int firstGamePos = (pageNumber - 1) * LogicSpecification.PAGE_SIZE;
            int lastGamePos = firstGamePos + LogicSpecification.PAGE_SIZE;
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
                HasNextPage = utils.ExistsNextPage(filteredList, pageNumber),
                HasPreviousPage = pageNumber > 1
            };
            return ret;
        }

        public GamePage BrowseGames(int pageNumber)
        {
            List<Game> games = da.Games;
            lock (games)
            {
                return CreateGamePage(games, pageNumber);
            }
        }

        public GamePage BrowseMyGames(int pageNumber, INetworkStreamHandler nwsh)
        {
            BusinessLogicUtils utils = BusinessLogicUtils.GetInstance();
            User CurrentUser = utils.GetUser(nwsh);
            List<Game> games = da.Games;
            lock (games)
            {
                return CreateGamePage(CurrentUser.GamesOwned, pageNumber);
            }
        }

        public GamePage SearchByTitle(int pageNumber, string title)
        {
            List<Game> games = da.Games;
            lock (games)
            {
                List<Game> filteredList = games.FindAll(game => TextSearchIsMatch(game.Title, title));
                return CreateGamePage(filteredList, pageNumber);
            }
        }

        public GamePage SearchByRating(int pageNumber, int minRating)
        {
            List<Game> games = da.Games;
            lock (games)
            {
                List<Game> filteredList = games.FindAll(game => game.ReviewsRating >= minRating);
                return CreateGamePage(filteredList, pageNumber);
            }
        }

        public GamePage SearchByGenre(int pageNumber, string genre)
        {
            List<Game> games = da.Games;
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


    }
}
