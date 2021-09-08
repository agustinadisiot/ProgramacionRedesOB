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

        public Steam()
        {
            games = new List<Game>();
            users = new List<User>();
        }
    }
}
