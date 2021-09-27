using Common;
using Common.Domain;
using Common.Utils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Server
{
    public class Steam
    {
        private DataAccess db;
        private static Steam instance;

        private static readonly object singletonPadlock = new object();

        public static Steam GetInstance()
        {
            lock (singletonPadlock)
            {

                if (instance == null)
                    instance = new Steam();
            }
            return instance;
        }


        public Steam()
        {
            db = new DataAccess();
        }
        
    }
}
