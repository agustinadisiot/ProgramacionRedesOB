using System.Collections.Generic;

namespace Common.Domain
{
    public struct GamePage
    {
        public List<string> GamesTitles { get; set; }
        public List<int> GamesIds { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public int CurrentPage { get; set; }
    }
}
