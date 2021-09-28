using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.BusinessLogic
{
    public class BusinessLogicReview
    {
        private DataAccess da;
        private static BusinessLogicReview instance;

        private static readonly object singletonPadlock = new object();

        public static BusinessLogicReview GetInstance()
        {
            lock (singletonPadlock)
            {

                if (instance == null)
                    instance = new BusinessLogicReview();
            }
            return instance;
        }


        public BusinessLogicReview()
        {
            da = DataAccess.GetInstance();
        }

        public string WriteReview(Review newReview, int gameId, INetworkStreamHandler nwsh)
        {
            BusinessLogicUtils utils = BusinessLogicUtils.GetInstance();
            newReview.Author = utils.GetUser(nwsh); 
            List<Game> games = da.Games;
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
            BusinessLogicUtils utils = BusinessLogicUtils.GetInstance();
            if (pageNumber <= 0)
                throw new ServerError($"Número de página {pageNumber} no válido");
            lock (da.Games)
            {
                Game gameToGetReviews = utils.GetGameById(gameId);
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
                        HasNextPage = utils.ExistsNextPage(allReviews, pageNumber), 
                        HasPreviousPage = pageNumber > 1
                    };
                    return ret;
                }
            }
        }

        
    }
}
