using Common;
using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using Common.Utils;
using System.Collections.Generic;

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
                if (actualUser == null)
                {
                    string msg = "No se encontró el usuario, rehacer login";
                    Logger.Log(new LogRecord { Message = msg, Severity = LogRecord.ErrorSeverity });
                    throw new ServerError("No se encontró el usuario, rehacer login");
                }
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

                string msg = "Error al conseguir nombre del usuario";
                Logger.Log(new LogRecord { Message = msg, Severity = LogRecord.ErrorSeverity });
                throw new ServerError(msg);
            }
        }


        public User GetUser(int Id)
        {
            List<User> users = da.Users;
            lock (users)
            {
                User actualUser = users.Find(i => i.Id == Id);
                if (actualUser == null)
                {
                    string msg = "No se encontró el usuario, rehacer login";
                    Logger.Log(new LogRecord { UserId = Id, Message = msg, Severity = LogRecord.WarningSeverity });
                    throw new ServerError(msg);
                }
                return actualUser;
            }
        }




        public bool ExistsNextPage<T>(List<T> fullList, int pageNumber)
        {
            int maxPageNumber = fullList.Count / LogicSpecification.PAGE_SIZE;
            if (fullList.Count % LogicSpecification.PAGE_SIZE != 0)
                maxPageNumber++;

            return pageNumber < maxPageNumber;
        }
    }
}
