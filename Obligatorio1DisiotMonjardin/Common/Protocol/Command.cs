﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Protocol
{
    public enum Command
    {
        LOGIN, // hecho
        LOGOUT, // hecho
        PUBLISH_GAME, //hecho 
        MODIFY_GAME,
        DELETE_GAME,
        BUY_GAME, //hecho
        VIEW_GAME, // hecho
        SEARCH_BY_TITLE, // si - hacer refactor para juntar con browse games
        SEARCH_BY_RATING,
        SEARCH_BY_GENRE,
        WRITE_REVIEW, // hecho
        BROWSE_REVIEWS, // hecho
        DOWNLOAD_COVER, //hecho
        BROWSE_CATALOGUE, // si ponele
        BROWSE_MY_GAMES,
        ERROR
    }
}
