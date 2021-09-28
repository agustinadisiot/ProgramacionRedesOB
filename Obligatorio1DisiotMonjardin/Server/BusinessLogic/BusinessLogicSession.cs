using Common.Domain;
using Common.NetworkUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.BusinessLogic
{
    public class BusinessLogicSession
    {
        private DataAccess da;
        private static BusinessLogicSession instance;

        private static readonly object singletonPadlock = new object();

        public static BusinessLogicSession GetInstance()
        {
            lock (singletonPadlock)
            {

                if (instance == null)
                    instance = new BusinessLogicSession();
            }
            return instance;
        }


        public BusinessLogicSession()
        {
            da = DataAccess.GetInstance();
        }

        public bool Login(string newUserName, INetworkStreamHandler nwsh)
        {
            List<User> users = da.Users;
            lock (users)
            {
                User newUser = new User(newUserName);
                bool alreadyExists = users.Contains(newUser);
                if (!alreadyExists)
                {
                    users.Add(newUser);
                }
                da.Connections.Add(nwsh, newUserName);

                return !alreadyExists;
            }
        }

        public bool Logout(INetworkStreamHandler nwsh)
        {
            var connections = da.Connections;
            lock (connections)
            {
                return connections.Remove(nwsh);
            }
        }
    }
}
