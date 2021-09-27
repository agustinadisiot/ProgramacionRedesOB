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
        DataAccess db = DataAccess.GetInstance();

        public User GetUser(INetworkStreamHandler nwsh)
        {
            List<User> users = db.Users;
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
            var connections = db.Connections;
            lock (connections)
            {
                bool userLoggedIn = connections.TryGetValue(nwsh, out string username);
                if (userLoggedIn)
                    return username;
                else
                    throw new ServerError("No existe usuario con ese nombre");
            }
        }

    }
}
