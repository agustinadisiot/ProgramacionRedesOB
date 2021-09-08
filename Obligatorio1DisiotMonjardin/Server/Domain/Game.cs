using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Domain
{
    public class Game
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Synopsis { get; set; }
        public int ReviewsRating { get; }
        public string CoverFilePath  { get; set; }
        public ESRBRating ESRBRating { get; set; }
        public string Genre { get; set; }

        //Usuario que lo publico, genero, reviews y clasificacion, enum de esrbrating
    }
}
