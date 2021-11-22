using Common;
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
                bool alreadyExists = users.Exists(u => u.Name == newUserName);

                if (!alreadyExists)
                {
                    User newUser = new User()
                    {
                        Name = newUserName,
                        Id = da.NextUserID
                    };
                    da.Users.Add(newUser);
                    LogUser(newUser, "Usuario creado");
                }
                da.Connections.Add(nwsh, newUserName);
                User loggedInUser = users.Find(u => u.Name == newUserName);
                LogUser(loggedInUser, "Usuario inició sesióñ");

                return !alreadyExists;
            }
        }

        public bool Logout(INetworkStreamHandler nwsh)
        {
            var connections = da.Connections;
            bool res = false;
            {
                connections.TryGetValue(nwsh, out string username);
                User loggedOutUser = da.Users.Find(u => u.Name == username);
                LogUser(loggedOutUser, "Usuario cerró sesión");
            }
            lock (connections)
            {
                res = connections.Remove(nwsh);
            }
            lock (da.Users)
                return res;
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
                    LogUser(newUser, "Usuario creado correctamente");
                }
                else
                {
                    Logger.Log(new LogRecord
                    {
                        Message = "Ya existe el usuario",
                        UserId = newUser.Id,
                        Username = newUser.Name,
                        Severity = LogRecord.WarningSeverity
                    });
                }
                return alreadyExists ? "Ya existe el usuario" : "Usuario creado correctamente";
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
                {
                    try
                    {
                        modifiedUser.Name = ModifiedValidName(modifiedUser.Name, id, users);
                    }
                    catch (Exception e) when (e is NameAlreadyExistsException || e is ServerError)
                    {
                        Logger.Log(new LogRecord
                        {
                            UserId = id,
                            Username = modifiedUser.Name,
                            Severity = LogRecord.WarningSeverity,
                            Message = $"Se intento modificar el nombre a {modifiedUser.Name} pero ocurrión el error: {e.Message}"
                        });
                    }
                }
                Logger.Log(new LogRecord
                {
                    UserId = id,
                    Username = modifiedUser.Name,
                    Severity = LogRecord.InfoSeverity,
                    Message = $"Se  modificar el nombre usuario"
                });
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
                throw new ServerError("Nombre no válido");
            return name;
        }

        private void LogUser(User userToLog, string msg)
        {
            Logger.Log(new LogRecord
            {
                Message = msg,
                UserId = userToLog.Id,
                Username = userToLog.Name,
                Severity = LogRecord.InfoSeverity
            });
        }

        internal List<User> GetUsers()
        {
            return da.Users;
        }
    }
}
