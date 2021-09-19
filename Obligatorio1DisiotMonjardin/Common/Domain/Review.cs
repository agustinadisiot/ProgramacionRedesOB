using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Domain
{
    public class Review
    {
        public string Text { get; set; }
        public User User { get; set; } // TODO capaz cambiar por algo mas nemotecnico
        public int Rating { get; set; } 
    }
}
