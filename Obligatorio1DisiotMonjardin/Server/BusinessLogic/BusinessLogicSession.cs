using Common.Domain;
using Common.NetworkUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.BusinessLogic
{
    public class BusinessLogicSession
    {
        DataAccess db = DataAccess.GetInstance();
        
        public bool Login(string newUserName, INetworkStreamHandler nwsh)
        {
            List<User> users = db.Users;
            lock (users)
            {
                User newUser = new User(newUserName);
                bool alreadyExists = users.Contains(newUser);
                if (!alreadyExists)
                {
                    users.Add(newUser);
                }
                db.Connections.Add(nwsh, newUserName);

                return !alreadyExists;
            }
        }

        public bool Logout(INetworkStreamHandler nwsh)
        {
            var connections = db.Connections;
            lock (connections)
            {
                return connections.Remove(nwsh);
            }
        }
    }
}
