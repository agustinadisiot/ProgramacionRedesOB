using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using Common.Utils;
using System;
using System.Collections.Generic;

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
                User newUser = new User() { 
                    Name = newUserName,
                    Id = da.NextUserID
                };
                bool alreadyExists = users.Contains(newUser);
                if (!alreadyExists)
                {
                    da.Users.Add(newUser);
                }
                da.Connections.Add(nwsh, newUserName);

                Logger.Log(new Log{ 
                    Message= "User logged in",
                    UserId = newUser.Id,
                    Username = newUser.Name,
                    Severity = Log.InfoSeverity
                });

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

        internal string CreateUser(string name)
        {
            List<User> users = da.Users;
            lock (users)
            {
                User newUser = new User()
                {
                    Name = name,
                    Id = da.NextUserID
                };
                bool alreadyExists = users.Contains(newUser);
                if (!alreadyExists)
                {
                    da.Users.Add(newUser);
                }

                return alreadyExists ? "Could not create user" : "User created succesfully";
            }
        }

        internal string ModifyUser(int id, User modifiedUser)
        {
            BusinessLogicUtils utils = BusinessLogicUtils.GetInstance();
            List<User> users = da.Users;
            lock (users)
            {
                User userToMod = utils.GetUser(id);

                if (modifiedUser.Name != "")
                    modifiedUser.Name = ModifiedValidName(modifiedUser.Name, id, users);

                return $"Se modificó el usuario {userToMod.Name} correctamente";
            }

        }
        internal bool DeleteUser(int id)
        {
            List<User> users = da.Users;
            bool success;
            lock (users)
            {
                User userToDelete = users.Find(i => i.Id == id);
                success = users.Remove(userToDelete);
            }
            return success;
        }

        private string ModifiedValidName(string name, int userToModId, List<User> users)
        {
            var userWithSameTitle = users.Find(i => (i.Name == name) && (i.Id != userToModId));
            if (userWithSameTitle != null)
                throw new NameAlreadyExistsException();
            if (!Validation.IsValidTitle(name))
                throw new ServerError("Título no válido");
            return name;
        }
    }
}
