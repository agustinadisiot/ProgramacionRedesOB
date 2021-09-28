using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.BusinessLogic
{
    public class BusinessLogicUtils
    {
        private DataAccess da;
        private static BusinessLogicUtils instance;

        private static readonly object singletonPadlock = new object();

        public static BusinessLogicUtils GetInstance()
        {
            lock (singletonPadlock)
            {

                if (instance == null)
                    instance = new BusinessLogicUtils();
            }
            return instance;
        }


        public BusinessLogicUtils()
        {
            da = DataAccess.GetInstance();
        }

        public User GetUser(INetworkStreamHandler nwsh)
        {
            List<User> users = da.Users;
            lock (users)
            {
                string username = GetUsername(nwsh);
                User actualUser = users.Find(i => i.Name == username);
                if (actualUser == null) throw new ServerError("No se encontró el usuario, rehacer login");
                return users.Find(i => i.Name == username);
            }
        }

        private string GetUsername(INetworkStreamHandler nwsh)
        {
            var connections = da.Connections;
            lock (connections)
            {
                bool userLoggedIn = connections.TryGetValue(nwsh, out string username);
                if (userLoggedIn)
                    return username;
                else
                    throw new ServerError("No existe usuario con ese nombre");
            }
        }

        // PRE: Requires lock on da.Games TODO ver si esta bien poner esto
        public Game GetGameById(int gameId)
        {
            Game gameFound = da.Games.Find(game => game.Id == gameId);
            if (gameFound == null)
                throw new ServerError($"{gameId} No es una id de juego válida");
            return gameFound;
        }

        public bool ExistsNextPage<T>(List<T> fullList, int pageNumber)
        {
            int maxPageNumber = fullList.Count / Specification.PAGE_SIZE;
            if (fullList.Count % Specification.PAGE_SIZE != 0)
                maxPageNumber++;

            return pageNumber < maxPageNumber;
        }
    }
}
