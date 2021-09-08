using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Domain
{
    public class User
    {
        public string Name { get; private set; }
        public List<Game> GamesOwned { get; private set; }

        public User(string name)
        {
            Name = name;
            GamesOwned = new List<Game>();
        }
    }
}
