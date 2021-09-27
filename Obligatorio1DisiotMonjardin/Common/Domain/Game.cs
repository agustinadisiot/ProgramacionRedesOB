using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Domain
{
    public class Game
    {
        public static readonly string[] genres = { "Accion", "Aventura", "Juego de Rol", "Estrategia", "Deporte", "Carreras", "Otros" }; //todo refactor(?
        public int Id { get; set; }
        public string Title { get; set; }
        public string Synopsis { get; set; }
        public int ReviewsRating { get; set; }
        public string CoverFilePath { get; set; }
        public ESRBRating ESRBRating { get; set; }
        public string Genre { get; set; }
        public User Publisher { get; set; }
        public List<Review> Reviews { get; set; }

        public void UpdateReviewsRating()
        {
            lock (Reviews)
            {
                decimal total = 0;
                decimal count = Reviews.Count;
                foreach (Review review in Reviews)
                {
                    total += review.Rating;
                }
                decimal result;
                if (count > 0)
                    result = total / count;
                else
                    result = 0;
                ReviewsRating = (int)Math.Ceiling(result);
            }
        }
    }
}
