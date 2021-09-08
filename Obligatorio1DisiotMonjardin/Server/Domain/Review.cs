using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Domain
{
    public class Review
    {
        public string Text { get; set; }
        public User User { get; set; }
        public int Rating { get; set; } 
    }
}
