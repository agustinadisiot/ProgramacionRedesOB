using Common;
using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using Common.Utils;
using System.Collections.Generic;

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
                    string msg = $"Clasificación por {newReview.Author.Name} para el juego {gameToAddReview.Title} fue publicada correctamente";
                    Logger.Log(new LogRecord
                    {
                        GameName = gameToAddReview.Title,
                        GameId = gameToAddReview.Id,
                        Severity = LogRecord.InfoSeverity,
                        UserId = newReview.Author.Id,
                        Username = newReview.Author.Name,
                        Message = msg
                    });
                    return msg;
                }
            }
        }

        public ReviewPage BrowseReviews(int pageNumber, int gameId)
        {
            BusinessLogicGameCRUD crud = BusinessLogicGameCRUD.GetInstance();
            BusinessLogicUtils utils = BusinessLogicUtils.GetInstance();
            if (pageNumber <= 0)
                throw new ServerError($"Número de página {pageNumber} no válido");
            lock (da.Games)
            {
                Game gameToGetReviews = crud.GetGameById(gameId);
                lock (gameToGetReviews.Reviews)
                {
                    List<Review> allReviews = gameToGetReviews.Reviews;

                    int firstReviewPos = (pageNumber - 1) * LogicSpecification.PAGE_SIZE;
                    int pageSize;
                    if (firstReviewPos + LogicSpecification.PAGE_SIZE > allReviews.Count)
                        pageSize = allReviews.Count - firstReviewPos;
                    else
                        pageSize = LogicSpecification.PAGE_SIZE;


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
