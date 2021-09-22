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
                case Command.MODIFY_GAME:
                case Command.BUY_GAME:
                    return new BuyGame(nwsh);
                case Command.WRITE_REVIEW:
                    return new WriteReview(nwsh);
                case Command.BROWSE_REVIEWS:
                    return new BrowseReviews(nwsh);
                case Command.SEARCH_BY_TITLE:
                    return new SearchByTitle(nwsh);
                case Command.LOGIN:
                    return new Login(nwsh);
                case Command.LOGOUT:
                    return new Logout(nwsh);
                case Command.VIEW_GAME:
                    return new ViewGame(nwsh);
                default:
                    throw new NotImplementedException();

            }

        }

    }
}
