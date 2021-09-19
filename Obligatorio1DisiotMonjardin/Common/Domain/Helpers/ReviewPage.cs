using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Domain
{
    public struct ReviewPage
    {
        public List<string> Reviews { get; set; }
        public List<int> GamesIDs { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public int CurrentPage { get; set; }
    }
}
