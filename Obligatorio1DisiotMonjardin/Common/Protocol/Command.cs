using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Protocol
{
    public enum Command
    {
        LOGIN, // maso
        LOGOUT, // maso
        PUBLISH_GAME, //hecho 
        MODIFY_GAME, 
        DELETE_GAME,
        BUY_GAME, //hecho
        VIEW_GAME, // hecho
        SEARCH_BY_TITLE, // si - hacer refactor para juntar con browse games
        SEARCH_BY_RATING,
        SEARCH_BY_GENRE,
        VIEW_GENRES, 
        WRITE_REVIEW, // faltan verificaciones
        BROWSE_REVIEWS, // hecho
        DOWNLOAD_COVER,
        BROWSE_CATALOGUE, // si ponele
        VIEW_MY_GAME
    }
}
