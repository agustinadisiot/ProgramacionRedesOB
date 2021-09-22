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
            switch (cmd)
            {
                case Command.PUBLISH_GAME:
                    return new PublishGame(nwsh);
                case Command.BROWSE_CATALOGUE:
                    return new BrowseCatalogue(nwsh);
                case Command.BROWSE_REVIEWS:
                    return new BrowseReviews(nwsh);
                case Command.MODIFY_GAME:
                case Command.BUY_GAME:
                    return new BuyGame(nwsh);
                case Command.SEARCH_BY_TITLE:
                    return new SearchByTitle(nwsh);
                case Command.WRITE_REVIEW:
                    return new WriteReview(nwsh);
                case Command.LOGIN:
                    return new Login(nwsh);
                case Command.LOGOUT:
                    return new Logout(nwsh);
                case Command.VIEW_GAME:
                    return new ViewGame(nwsh);
                case Command.DOWNLOAD_COVER:
                    return new DownloadCover(nwsh);
                default:
                    throw new NotImplementedException();

            }

        }

    }
}
