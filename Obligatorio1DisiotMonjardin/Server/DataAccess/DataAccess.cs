using Common.Domain;
using Common.NetworkUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class DataAccess
    {

        public List<Game> Games { get; private set; }
        public List<User> Users { get; private set; }

        private int gameId;
        public int NextGameID
        {
            get
            {
                int res = gameId;
                gameId++;
                return res;

            }
            private set
            {
                gameId = value;
            }
        }
        private static DataAccess instance;

        public Dictionary<INetworkStreamHandler, string> Connections { get; private set; }

        private static readonly object singletonPadlock = new object();

        public static DataAccess GetInstance()
        {
            lock (singletonPadlock)
            {

                if (instance == null)
                    instance = new DataAccess();
            }
            return instance;
        }

        public DataAccess()
        {
            Games = new List<Game>();
            Users = new List<User>();
            gameId = 0;
            Connections = new Dictionary<INetworkStreamHandler, string>();

        }
    }
}
