using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Protocol
{
    public enum Command
    {
        LOGIN,
        CREATE_USER,
        PUBLISH_GAME,
        MODIFY_GAME,
        BUY_GAME,
        VIEW_GAME,
        SEARCH_BY_TITLE,
        SEARCH_BY_RATING,
        VIEW_GENRES,
        SEARCH_BY_GENRE,
        WRITE_REVIEW,
        BROWSE_REVIEWS,
        DOWNLOAD_COVER,
        BROWSE_CATALOGUE
    }
}
