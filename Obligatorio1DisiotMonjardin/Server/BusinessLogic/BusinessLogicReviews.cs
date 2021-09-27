using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.BusinessLogic
{
    public class BusinessLogicReviews
    {
        DataAccess db = DataAccess.GetInstance();

        public string WriteReview(Review newReview, int gameId, INetworkStreamHandler nwsh)
        {

            newReview.Author = GetUser(nwsh); //getUser esta en businessLogicUtils
            List<Game> games = db.Games;
            lock (games)
            {
                Game gameToAddReview = games[gameId];
                lock (gameToAddReview.Reviews)
                {
                    gameToAddReview.Reviews.Add(newReview);
                    gameToAddReview.UpdateReviewsRating();
                    return (@$"Clasificación por {newReview.Author.Name} para el juego {gameToAddReview.Title}
                    fue publicada correctamente");
                }
            }
        }

        public ReviewPage BrowseReviews(int pageNumber, int gameId)
        {
            if (pageNumber <= 0)
                throw new ServerError($"Número de página {pageNumber} no válido");
            lock (db.Games)
            {
                Game gameToGetReviews = GetGameById(gameId);
                lock (gameToGetReviews.Reviews)
                {
                    List<Review> allReviews = gameToGetReviews.Reviews;

                    int firstReviewPos = (pageNumber - 1) * Specification.PAGE_SIZE;
                    int pageSize;
                    if (firstReviewPos + Specification.PAGE_SIZE > allReviews.Count)
                        pageSize = allReviews.Count - firstReviewPos;
                    else
                        pageSize = Specification.PAGE_SIZE;


                    List<Review> reviewsInPage = allReviews.GetRange(firstReviewPos, pageSize);

                    ReviewPage ret = new ReviewPage()
                    {
                        Reviews = reviewsInPage,
                        HasNextPage = ExistsNextPage(allReviews, pageNumber), //existNextPage esta en gamePage
                        HasPreviousPage = pageNumber > 1
                    };
                    return ret;
                }
            }
        }

        // PRE: Requires lock on db.Games TODO ver si esta bien poner esto
        private Game GetGameById(int gameId)
        {
            Game gameFound = db.Games.Find(game => game.Id == gameId);
            if (gameFound == null)
                throw new ServerError($"{gameId} No es una id de juego válida");
            return gameFound;
        }
    }
}
