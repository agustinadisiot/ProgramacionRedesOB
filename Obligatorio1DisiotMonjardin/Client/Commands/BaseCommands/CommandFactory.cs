using Client.Commands;
using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    public static class CommandFactory
    {
        public static CommandHandler GetCommandHandler(Command cmd, INetworkStreamHandler nwsh)
        {
            switch (cmd)
            {
                case Command.PUBLISH_GAME:
                    return new PublishGame(nwsh);
                case Command.BROWSE_CATALOGUE:
                    return new BrowseCatalogue(nwsh);
                case Command.CREATE_USER:
                case Command.MODIFY_GAME:
                case Command.LOGIN:
                default:
                    throw new NotImplementedException();

            }

        }

    }
}
