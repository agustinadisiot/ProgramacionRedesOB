using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using Server.Commands;
using Server.Commands.BaseCommands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public static class CommandFactory
    {
        public static CommandHandler GetCommandHandler(Command cmd, INetworkStreamHandler nwsh)
        {
            switch(cmd){
                case Command.PUBLISH_GAME:
                    return new PublishGame(nwsh);
                case Command.BROWSE_CATALOGUE:
                    return new BrowseCatalogue(nwsh);
                case Command.CREATE_USER:
                case Command.MODIFY_GAME:
                case Command.SEARCH_BY_TITLE:
                    return new SearchByTitle(nwsh);
                case Command.LOGIN:
                    return new Login(nwsh);
                case Command.VIEW_GAME:
                    return new ViewGame(nwsh);
                default:
                    throw new NotImplementedException();

            }

        }

    }
}
