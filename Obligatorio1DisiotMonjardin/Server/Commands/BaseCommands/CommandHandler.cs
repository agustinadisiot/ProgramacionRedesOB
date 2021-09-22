using Common.FileHandler.Interfaces;
using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public abstract class CommandHandler
    {
        protected INetworkStreamHandler networkStreamHandler;
        protected IFileNetworkStreamHandler fileNetworkStreamHandler;
        public CommandHandler(INetworkStreamHandler nwsh)
        {
            networkStreamHandler = nwsh;
            fileNetworkStreamHandler = new FileNetworkStreamHandler(nwsh);
        }
        public abstract void HandleRequest();
    }
}
