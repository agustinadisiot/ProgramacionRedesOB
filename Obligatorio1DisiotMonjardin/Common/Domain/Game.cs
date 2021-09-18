using Common;
using Server.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Domain
{
    public class Game
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Synopsis { get; set; }
        public int ReviewsRating { get; set; }
        public string CoverFilePath  { get; set; }
        public ESRBRating ESRBRating { get; set; }
        public string Genre { get; set; }
        public User Publisher { get; set; }
        public List<Review> Reviews { get; set; }

    }
}
