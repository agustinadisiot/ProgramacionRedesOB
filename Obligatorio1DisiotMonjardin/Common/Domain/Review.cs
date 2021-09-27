using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Domain
{
    public class Review
    {
        public string Text { get; set; }
        public User Author { get; set; }
        public int Rating { get; set; }
    }
}
