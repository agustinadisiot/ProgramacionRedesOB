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

        public string FirstGame()
        {
            if (games.Count == 0) return "Primer Juego";
            return games[0].Title;
        }
    }
}
