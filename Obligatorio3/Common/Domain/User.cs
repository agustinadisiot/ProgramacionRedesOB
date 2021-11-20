using System;
using System.Collections.Generic;

namespace Common.Domain
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public List<Game> GamesOwned { get; private set; }

        public User()
        {
            GamesOwned = new List<Game>();
        }

        public override bool Equals(object obj)
        {
            User compare = (User)obj;
            return compare.Name == this.Name;
        }

        public void DeleteGame(int gameId)
        {
            lock (GamesOwned)
            {
                Game gameToDelete = GamesOwned.Find(i => i.Id == gameId);
                GamesOwned.Remove(gameToDelete);
            }
        }
    }
}
