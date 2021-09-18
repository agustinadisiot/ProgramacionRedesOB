﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Protocol
{
    public enum Command
    {
        LOGIN, // maso
        CREATE_USER, 
        PUBLISH_GAME, // maso - solo hay titulo
        MODIFY_GAME, 
        BUY_GAME, 
        VIEW_GAME, 
        SEARCH_BY_TITLE, // si - hacer refactor para juntar con browse games
        SEARCH_BY_RATING,
        SEARCH_BY_GENRE,
        VIEW_GENRES, 
        WRITE_REVIEW,
        BROWSE_REVIEWS,
        DOWNLOAD_COVER,
        BROWSE_CATALOGUE // si ponele
    }
}
