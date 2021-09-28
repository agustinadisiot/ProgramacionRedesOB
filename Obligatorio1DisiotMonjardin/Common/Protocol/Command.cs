namespace Common.Protocol
{
    public enum Command
    {
        LOGIN,
        LOGOUT,
        PUBLISH_GAME,
        MODIFY_GAME,
        DELETE_GAME,
        BUY_GAME,
        VIEW_GAME,
        SEARCH_BY_TITLE,
        SEARCH_BY_RATING,
        SEARCH_BY_GENRE,
        WRITE_REVIEW,
        BROWSE_REVIEWS,
        DOWNLOAD_COVER,
        BROWSE_CATALOGUE,
        BROWSE_MY_GAMES,
        ERROR,
        SERVER_SHUTDOWN,
        EXIT,
    }
}
