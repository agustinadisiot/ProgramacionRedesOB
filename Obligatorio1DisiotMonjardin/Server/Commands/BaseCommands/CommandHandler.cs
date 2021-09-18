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
        public CommandHandler(INetworkStreamHandler nwsh)
        {
            networkStreamHandler = nwsh;
        }
        public abstract void HandleRequest();
    }
}
