using Common;
using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using Common.Utils;
using Server.Utils;
using System.Collections.Generic;
using System.IO;

namespace Server.BusinessLogic
{
    public class BusinessLogicGameCUD
    {

        private DataAccess da;
        private static BusinessLogicGameCUD instance;

        private static readonly object singletonPadlock = new object();

        public static BusinessLogicGameCUD GetInstance()
        {
            lock (singletonPadlock)
            {

                if (instance == null)
                    instance = new BusinessLogicGameCUD();
            }
            return instance;
        }


        public BusinessLogicGameCUD()
        {
            da = DataAccess.GetInstance();
        }

        public string PublishGame(Game newGame, INetworkStreamHandler nwsh)
        {
            BusinessLogicUtils utils = BusinessLogicUtils.GetInstance();
            VerifyGame(newGame);
            List<Game> games = da.Games;
            lock (games)
            {
                var gameWithSameTitle = games.Find(i => i.Title == newGame.Title);
                if (gameWithSameTitle != null)
                    throw new TitleAlreadyExistsException();
                newGame.Id = da.NextGameID;
                newGame.ReviewsRating = 0;
                newGame.Publisher = utils.GetUser(nwsh);
                newGame.Reviews = new List<Review>();
                games.Add(newGame);
                return $"Se publicó el juego {newGame.Title} correctamente";
            }
        }
        private void VerifyGame(Game newGame)
        {
            if (!Validation.IsValidTitle(newGame.Title))
                throw new ServerError("Título no válido");

            var gameWithSameTitle = da.Games.Find(i => (i.Title == newGame.Title) && (i.Id != newGame.Id));
            if (gameWithSameTitle != null)
                throw new TitleAlreadyExistsException();

            if (!Validation.IsValidSynopsis(newGame.Synopsis))
                throw new ServerError("Sinopsis no válida");

            if (!Validation.IsValidESRBRating((int)newGame.ESRBRating))
                throw new ServerError("Clasificación ESRB no válida");

            if (!Validation.IsValidGenre(newGame.Genre))
                throw new ServerError("Genero no válido");

            if (!File.Exists(newGame.CoverFilePath))
                throw new ServerError("CoverPath no válido");
        }

        public bool DeleteGame(int gameId)
        {
            List<Game> games = da.Games;
            bool success;
            lock (games)
            {
                Game gameToDelete = games.Find(i => i.Id == gameId);
                string pathToDelete = gameToDelete.CoverFilePath;
                DeleteFile.DeleteFileInAnotherThread(pathToDelete);
                success = games.Remove(gameToDelete);
            }
            List<User> users = da.Users;
            lock (users)
            {
                foreach (User user in users)
                    user.DeleteGame(gameId);
            }
            return success;
        }

        public string ModifyGame(int gameToModId, Game modifiedGame)
        {
            BusinessLogicUtils utils = BusinessLogicUtils.GetInstance();
            List<Game> games = da.Games;
            lock (games)
            {
                Game gameToMod = utils.GetGameById(gameToModId);

                if (modifiedGame.Title != "")
                    gameToMod.Title = ModifiedValidTitle(modifiedGame.Title, gameToModId, games);

                if (modifiedGame.Synopsis != "")
                    gameToMod.Synopsis = ModifiedValidSynopsis(modifiedGame.Synopsis);

                if (modifiedGame.ESRBRating != Common.ESRBRating.EmptyESRB)
                    gameToMod.ESRBRating = ModifiedValidESRBRating(modifiedGame.ESRBRating);

                if (modifiedGame.Genre != "")
                    gameToMod.Genre = ModifedValidGenre(modifiedGame.Genre);

                if (modifiedGame.CoverFilePath != null)
                {
                    string pathToDelete = gameToMod.CoverFilePath;
                    gameToMod.CoverFilePath = modifiedGame.CoverFilePath;
                    DeleteFile.DeleteFileInAnotherThread(pathToDelete);
                }
                return $"Se modificó el juego {gameToMod.Title} correctamente";
            }
        }


        private string ModifiedValidTitle(string title, int gameToModId, List<Game> games)
        {
            var gameWithSameTitle = games.Find(i => (i.Title == title) && (i.Id != gameToModId));
            if (gameWithSameTitle != null)
                throw new TitleAlreadyExistsException();
            if (!Validation.IsValidTitle(title))
                throw new ServerError("Título no válido");
            return title;
        }
        private string ModifiedValidSynopsis(string synopsis)
        {
            if (!Validation.IsValidSynopsis(synopsis))
                throw new ServerError("Sinopsis no válida");
            return synopsis;
        }
        private ESRBRating ModifiedValidESRBRating(ESRBRating eSRBRating)
        {
            if (!Validation.IsValidESRBRating((int)eSRBRating))
                throw new ServerError("Clasificación ESRB no válida");
            return eSRBRating;
        }

        private string ModifedValidGenre(string genre)
        {
            if (!Validation.IsValidGenre(genre))
                throw new ServerError("Genero no válido");
            return genre;
        }
    }
}
