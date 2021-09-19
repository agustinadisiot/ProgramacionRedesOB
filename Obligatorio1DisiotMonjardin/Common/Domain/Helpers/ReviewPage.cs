using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Domain
{
    public struct ReviewPage
    {
        public List<Review> Reviews { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public int CurrentPage { get; set; }
    }
}
