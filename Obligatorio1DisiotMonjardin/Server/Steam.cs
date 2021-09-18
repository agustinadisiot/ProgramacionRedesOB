using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using Server.Domain;
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


        public bool Login(string newUserName, INetworkStreamHandler nwsh)
        {
            User newUser = new User(newUserName);
            bool alreadyExists = users.Contains(newUser);
            if (!alreadyExists)
            {
                users.Add(newUser);
                connections.Add(nwsh, newUserName);
            }

            return !alreadyExists;
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

        public void AddGame(Game newGame)
        {

            var gameWithSameTitle = games.Find(i => i.Title == newGame.Title);
            if (gameWithSameTitle != null)
                throw new Exception(); //TODO hacer la exception
            newGame.Id = this.gameId;
            gameId++;
            games.Add(newGame);
            Console.WriteLine("Game has been published with title: " + newGame.Title + " and id: " + newGame.Id);

        }

        public string FirstGame() // TODO eliminar cuando no se use mas- era para una prueba
        {
            if (games.Count == 0) return "Primer Juego asdfghjk";
            return games[0].Title;
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

        bool textSearchIsMatch(string real, string search) { 
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
