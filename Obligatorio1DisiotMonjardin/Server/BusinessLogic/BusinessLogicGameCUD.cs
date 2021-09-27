using Common.Domain;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using Common.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Server.BusinessLogic
{
    public class BusinessLogicGameCUD
    {
        DataAccess db = DataAccess.GetInstance();

        public string PublishGame(Game newGame, INetworkStreamHandler nwsh)
        {
            //VerifyGame(newGame); todo
            List<Game> games = db.Games;
            lock (games)
            {
                var gameWithSameTitle = games.Find(i => i.Title == newGame.Title);
                if (gameWithSameTitle != null)
                    throw new TitleAlreadyExistsException();
                newGame.Id = db.NextGameID;
                newGame.ReviewsRating = 0;
                newGame.Publisher = GetUser(nwsh);
                newGame.Reviews = new List<Review>();
                games.Add(newGame);
                return $"Se publicó el juego {newGame.Title} correctamente";
            }
        }
        private void VerifyGame(Game newGame)
        {
            throw new NotImplementedException(); //TODO 
        }

        public string ModifyGame(int gameToModId, Game modifiedGame)
        {
            List<Game> games = db.Games;
            lock (games)
            {
                Game gameToMod = GetGameById(gameToModId);

                if (modifiedGame.Title != "")
                {
                    var gameWithSameTitle = games.Find(i => (i.Title == modifiedGame.Title) && (i.Id != gameToModId));
                    if (gameWithSameTitle != null)
                        throw new TitleAlreadyExistsException();
                    if (!Validation.isValidTitle(modifiedGame.Title))
                        throw new ServerError("Título no válido");
                    gameToMod.Title = modifiedGame.Title;
                }

                if (modifiedGame.Synopsis != "")
                {
                    if (!Validation.isValidSynopsis(modifiedGame.Synopsis))
                        throw new ServerError("Sinopsis no válida");
                    gameToMod.Synopsis = modifiedGame.Synopsis;
                }
                if (modifiedGame.ESRBRating != Common.ESRBRating.EmptyESRB)
                {
                    if (!Validation.isValidESRBRating((int)modifiedGame.ESRBRating))
                        throw new ServerError("Clasificación ESRB no válida");
                    gameToMod.ESRBRating = modifiedGame.ESRBRating;
                }

                if (modifiedGame.Genre != "")
                {
                    if (!Validation.isValidGenre(modifiedGame.Genre))
                        throw new ServerError("Genero no válido");
                    gameToMod.Genre = modifiedGame.Genre;
                }

                if (modifiedGame.CoverFilePath != null)
                {
                    string pathToDelete = gameToMod.CoverFilePath;
                    gameToMod.CoverFilePath = modifiedGame.CoverFilePath;
                    File.Delete(pathToDelete); // TODO capaz no tendria que ir en Steam TODO lock
                }
                return $"Se modificó el juego {gameToMod.Title} correctamente";
            }
        }

        public bool DeleteGame(int gameId)
        {
            List<Game> games = db.Games;
            lock (games)
            {
                return games.Remove(games.Find(i => i.Id == gameId));
                // TODO eliminar caratula
            }
        }

    }
}
