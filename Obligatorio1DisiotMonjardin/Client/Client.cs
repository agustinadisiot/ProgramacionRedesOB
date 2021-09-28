﻿using Client.Commands;
using Common;
using Common.Domain;
using Common.FileHandler;
using Common.NetworkUtils;
using Common.Protocol;
using Common.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    public class Client // TODO cambiar nombre de client y clienteProgram
                        // TODO organizar funciones
    {
        private NetworkStreamHandler networkStreamHandler;
        private readonly TcpClient tcpClient;
        private readonly IPEndPoint serverIpEndPoint;
        private readonly FileHandler fileHandler;

        public Client()
        {
            var clientIpEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0); // Puerto 0 -> usa el primer puerto disponible
            tcpClient = new TcpClient(clientIpEndPoint);
            serverIpEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6000); // TODO usar config files
            fileHandler = new FileHandler();
        }
        public void StartConnection()
        {
            tcpClient.Connect(serverIpEndPoint);
            networkStreamHandler = new NetworkStreamHandler(tcpClient.GetStream());
        }

        public void StartMenu()
        {
            Dictionary<string, Action> menuOptions = new Dictionary<string, Action>
            {
                { "Iniciar Sesión", () => Login() },
                { "Salir", () => EndConnection()}
            };
            CliMenu.showMenu(menuOptions, "Menu Inicial");

        }

        private void DeveloperMenu()
        {
            // Se dejó para mostrar en la defensa y para facilitar la correción
            Dictionary<string, Action> developerOptions = new Dictionary<string, Action>
            {
                { "Enviar parámetro incorrecto", () => BrowseMyGames(-1) },
                { "Datos de prueba", () => TestData() },
            };
            CliMenu.showMenu(developerOptions, "Menu de desarrollador");
        }
        private void EndConnection()
        {
            Console.WriteLine("Seguro que quiere cerrar la conexion?");
            Console.WriteLine("1.Si");
            Console.WriteLine("2.No");
            int input = Validation.ReadValidNumber("Ingrese una opción correcta", 1, 2);
            if (input == 1)
            {
                var commandHandler = (Exit)CommandFactory.GetCommandHandler(Command.EXIT, networkStreamHandler);
                commandHandler.SendRequest();
                tcpClient.Close();
            }
            else { StartMenu(); }
        }

        public void MainMenu()
        {
            Dictionary<string, Action> menuOptions = new Dictionary<string, Action>
            {
                { "Ver catalogo", () => BrowseCatalogue() },
                { "Publicar Juego", () => Publish() },
                { "Buscar por título", () => SearchByTitle() },
                { "Buscar por género", () => SearchByGenre() },
                { "Buscar por clasificación", () => SearchByRating() },
                { "Ver mis juegos", () => BrowseMyGames() },
                { "Cerrar Sesión", () => Logout() },
                { "Menu de desarrollador", () => DeveloperMenu() },
            };
            Console.WriteLine();
            try
            {
                CliMenu.showMenu(menuOptions, "Menu");
            }
            catch (ServerError e)
            {
                HandleServerError(e.Message);
            }
            catch (ServerShutDown)
            {
                HandleServerShutDown();
            }
        }

        private void HandleServerShutDown()
        {
            Console.WriteLine("El servidor se cerró");
            Console.WriteLine("Cerrando cliente ...");
            Console.WriteLine();
        }

        private void HandleServerError(string message)
        {
            Console.WriteLine("Ocurrió un error con el servidor");
            Console.WriteLine($"Error: {message}");
            Console.WriteLine();
            MainMenu();
        }

        private void Login()
        {
            Console.WriteLine("Ingrese nombre de usuario: ");
            string username = Validation.ReadValidString("Reingrese un nombre de usuario válido");
            var commandHandler = (Login)CommandFactory.GetCommandHandler(Command.LOGIN, networkStreamHandler);
            bool success = commandHandler.SendRequest(username);
            if (success)
            {
                Console.WriteLine("Se inicio sesión correctamente");
                MainMenu();
            }
            else
            {
                Console.WriteLine("No se pudo iniciar sesión");
                Login();
            }

        }

        private void Logout()
        {
            var commandHandler = (Logout)CommandFactory.GetCommandHandler(Command.LOGOUT, networkStreamHandler);
            bool success = commandHandler.SendRequest();
            if (success)
            {
                Console.WriteLine("Se cerró sesión correctamente");
                StartMenu();
            }
            else
            {
                Console.WriteLine("No se pudo cerrar sesión");
                MainMenu();
            }

        }

        private void BrowseCatalogue(int pageNumber = 1)
        {
            BrowseCatalogue commandHandler = (BrowseCatalogue)CommandFactory.GetCommandHandler(Command.BROWSE_CATALOGUE, networkStreamHandler);
            GamePage newGamePage = commandHandler.SendRequest(pageNumber);

            Action nextPageOption = () => BrowseCatalogue(pageNumber + 1);
            Action previousPageOption = () => BrowseCatalogue(pageNumber - 1);
            string title = $"Catálogo de Juegos  - Página {pageNumber}";
            ShowGamePage(newGamePage, title, nextPageOption, previousPageOption);
        }

        private void BrowseMyGames(int pageNumber = 1)
        {
            BrowseMyGames commandHandler = (BrowseMyGames)CommandFactory.GetCommandHandler(Command.BROWSE_MY_GAMES, networkStreamHandler);
            GamePage newGamePage = commandHandler.SendRequest(pageNumber);

            Action nextPageOption = () => BrowseMyGames(pageNumber + 1);
            Action previousPageOption = () => BrowseMyGames(pageNumber - 1);
            string title = $"Mis Juegos - Página {pageNumber}";
            ShowGamePage(newGamePage, title, nextPageOption, previousPageOption);
        }

        private void SearchByTitle()
        {
            Console.WriteLine("Escriba el título del juego: ");
            string title = Validation.ReadValidString("Escriba un título válido");
            ShowSearchByTitlePage(title);
        }

        public void ShowSearchByTitlePage(string title, int pageNumber = 1)
        {
            var commandHandler = (SearchByTitle)CommandFactory.GetCommandHandler(Command.SEARCH_BY_TITLE, networkStreamHandler);
            GamePage newGamePage = commandHandler.SendRequest(pageNumber, title);

            Action nextPageOption = () => ShowSearchByTitlePage(title, pageNumber + 1);
            Action previousPageOption = () => ShowSearchByTitlePage(title, pageNumber - 1);
            string gamePageTitle = $"Juegos con \"{title}\" - Página {pageNumber}";
            ShowGamePage(newGamePage, gamePageTitle, nextPageOption, previousPageOption);
        }
        private void SearchByRating()
        {
            Console.WriteLine("Escriba la clasificación mínima del juego: ");
            int minRating = Validation.ReadValidNumber(@$"Escriba un número entre {LogicSpecification.MIN_RATING} y {LogicSpecification.MAX_RATING}", LogicSpecification.MIN_RATING, LogicSpecification.MAX_RATING);
            ShowSearchByRatingPage(minRating);
        }

        public void ShowSearchByRatingPage(int minRating, int pageNumber = 1)
        {
            var commandHandler = (SearchByRating)CommandFactory.GetCommandHandler(Command.SEARCH_BY_RATING, networkStreamHandler);
            GamePage newGamePage = commandHandler.SendRequest(pageNumber, minRating);

            Action nextPageOption = () => ShowSearchByRatingPage(minRating, pageNumber + 1);
            Action previousPageOption = () => ShowSearchByRatingPage(minRating, pageNumber - 1);
            string title = $"Juegos con clasificación {minRating} o más - Página {pageNumber}";
            ShowGamePage(newGamePage, title, nextPageOption, previousPageOption);
        }

        private void SearchByGenre()
        {
            Console.WriteLine("Elija el género que quiera: ");
            string genre = Validation.ReadValidGenre();
            ShowSearchByGenrePage(genre);
        }

        public void ShowSearchByGenrePage(string genre, int pageNumber = 1)
        {
            var commandHandler = (SearchByGenre)CommandFactory.GetCommandHandler(Command.SEARCH_BY_GENRE, networkStreamHandler);
            GamePage newGamePage = commandHandler.SendRequest(pageNumber, genre);

            Action nextPageOption = () => ShowSearchByGenrePage(genre, pageNumber + 1);
            Action previousPageOption = () => ShowSearchByGenrePage(genre, pageNumber - 1);
            string title = $"Juegos de {genre}  - Página {pageNumber}";

            ShowGamePage(newGamePage, title, nextPageOption, previousPageOption);
        }


        private void ShowGamePage(GamePage gamePage, string title, Action nextPage, Action previousPage)
        {
            Dictionary<string, Action> menuOptions = new Dictionary<string, Action>();

            for (int i = 0; i < gamePage.GamesTitles.Count; i++)
            {
                int idIndex = i;
                menuOptions.Add(gamePage.GamesTitles[i], () => ShowGameInfo(gamePage.GamesIds[idIndex]));
            }

            if (gamePage.HasNextPage)
                menuOptions.Add("Siguiene Página", () => nextPage());

            if (gamePage.HasPreviousPage)
                menuOptions.Add("Página Anterior", () => previousPage());

            menuOptions.Add("Volver al Menu Principal", () => MainMenu());

            CliMenu.showMenu(menuOptions, title);
        }


        private void ShowGameInfo(int gameId)
        {
            ViewGame commandHandler = (ViewGame)CommandFactory.GetCommandHandler(Command.VIEW_GAME, networkStreamHandler);
            string gameID = gameId.ToString();
            GameView gameInfo = commandHandler.SendRequest(gameID);

            Console.WriteLine();
            Console.WriteLine($"título: {gameInfo.Game.Title}");
            Console.WriteLine($"Sinopsis: {gameInfo.Game.Synopsis}");
            if (gameInfo.Game.ReviewsRating == 0) { Console.WriteLine($"Calificacion: -"); }
            else { Console.WriteLine($"Calificacion: {gameInfo.Game.ReviewsRating}"); }
            Console.WriteLine($"Clasificacion ESRB: {gameInfo.Game.ESRBRating}");
            Console.WriteLine($"Genero: {gameInfo.Game.Genre}");

            Dictionary<string, Action> menuOptions = new Dictionary<string, Action>();
            if (!gameInfo.IsOwned) menuOptions.Add("Comprar Juego", () => ShowBuyGameMenu(gameId));
            menuOptions.Add("Ver Reviews", () => ShowBrowseReviewsMenu(1, gameId));
            if (gameInfo.IsOwned) menuOptions.Add("Escribir Review", () => ShowWriteReviewMenu(gameId));
            if (gameInfo.IsPublisher)
            {
                menuOptions.Add("Modificar Juego", () => ModifyGame(gameId));
                menuOptions.Add("Eliminar Juego", () => DeleteGame(gameId));
            }
            menuOptions.Add("Descargar Caratula", () => DownloadCover(gameId));
            menuOptions.Add("Volver al Menu Pricipal", () => MainMenu());

            CliMenu.showMenu(menuOptions, "");

        }

        private void DownloadCover(int gameId)
        {
            Console.WriteLine("Escriba la carpeta donde quiere guardar la caratula");
            string folderPath = Validation.ReadValidDirectory("No se encontro tal carpeta, ingrese de nuevo", fileHandler);
            Console.WriteLine("Escriba el nombre que quiere para el archivo");
            string fileName = Validation.ReadValidString("Escriba un nombre válido para su archivo");

            var commandHandler = (DownloadCover)CommandFactory.GetCommandHandler(Command.DOWNLOAD_COVER, networkStreamHandler);
            string completePath = commandHandler.SendRequest(gameId, folderPath, fileName);

            Validation.CouldDownload(completePath, fileHandler);
            ShowGameInfo(gameId);
        }

        private void ShowBuyGameMenu(int gameId)
        {
            var commandHandler = (BuyGame)CommandFactory.GetCommandHandler(Command.BUY_GAME, networkStreamHandler);
            string message = commandHandler.SendRequest(gameId);
            Console.Write(message);
            ShowGameInfo(gameId);
        }

        private void ShowWriteReviewMenu(int gameId)
        {
            Console.WriteLine("Escriba una puntuación: (del 1 al 10)");
            int rating = Validation.ReadValidNumber("Escriba una puntuación: (del 1 al 10)", 1, 10);

            Console.WriteLine("Escriba un comentario ");
            string comment = Validation.ReadValidString("Escriba un comentario válido");

            Review newReview = new Review()
            {
                Text = comment,
                Rating = rating
            };
            var commandHandler = (WriteReview)CommandFactory.GetCommandHandler(Command.WRITE_REVIEW, networkStreamHandler);
            string message = commandHandler.SendRequest(newReview, gameId);
            Console.WriteLine(message);
            ShowGameInfo(gameId);
        }

        private void ShowBrowseReviewsMenu(int pageNumber, int gameId)
        {
            var commandHandler = (BrowseReviews)CommandFactory.GetCommandHandler(Command.BROWSE_REVIEWS, networkStreamHandler);
            ReviewPage newReviewPage = commandHandler.SendRequest(pageNumber, gameId);
            ShowReviewPage(newReviewPage, gameId);
        }

        private void ShowReviewPage(ReviewPage reviewPage, int gameId)
        {
            Console.WriteLine($"Calificaciones - Página {reviewPage.CurrentPage}");
            Console.WriteLine();
            Dictionary<string, Action> menuOptions = new Dictionary<string, Action>();

            foreach (Review review in reviewPage.Reviews)
            {
                Console.WriteLine($"{review.Author.Name} ({review.Rating}/{LogicSpecification.MAX_RATING}):");
                Console.WriteLine($"{review.Text}");
                Console.WriteLine();
            }

            if (reviewPage.HasNextPage)
                menuOptions.Add("Siguiene Página", () => ShowBrowseReviewsMenu(reviewPage.CurrentPage + 1, gameId));

            if (reviewPage.HasPreviousPage)
                menuOptions.Add("Página Anterior", () => ShowBrowseReviewsMenu(reviewPage.CurrentPage - 1, gameId));

            menuOptions.Add("Volver al Menu Del Juego", () => ShowGameInfo(gameId));
            menuOptions.Add("Volver al Menu Principal", () => MainMenu());

            CliMenu.showMenu(menuOptions, "");
        }

        private void Publish()
        {
            PublishGame commandHandler = (PublishGame)CommandFactory.GetCommandHandler(Command.PUBLISH_GAME, networkStreamHandler);
            Console.WriteLine("Escriba el título del juego:");
            string title = Validation.ReadValidString("Escriba un título del juego válido");

            Console.WriteLine("Escriba la sinopsis del juego:");
            string synopsis = Validation.ReadValidString("Escriba una sinopsis del juego valida");

            Console.WriteLine("Elija el ESRBrating del juego:");
            int ESRBRating = Validation.ReadValidESRB();

            Console.WriteLine("Elija el genero del juego:");
            string genre = Validation.ReadValidGenre();

            Console.WriteLine("Escriba la dirección del archivo de la caratula:");
            string coverPath = Validation.ReadValidPath("Escriba un archivo válido", fileHandler);

            Game newGame = new Game
            {
                Title = title,
                Synopsis = synopsis,
                ESRBRating = (Common.ESRBRating)ESRBRating,
                Genre = genre,
                CoverFilePath = coverPath
            };
            string returnMessage = commandHandler.SendRequest(newGame);
            ShowServerMessage(returnMessage);
        }

        private void ModifyGame(int gameId)
        {
            ModifyGame commandHandler = (ModifyGame)CommandFactory.GetCommandHandler(Command.MODIFY_GAME, networkStreamHandler);
            Console.WriteLine("Escriba el nuevo título del juego: (vacio si no lo quiere modificar)");
            string title = Validation.ContainsDelimiter("Escriba un nuevo título del juego válido");

            Console.WriteLine("Escriba la nueva sinopsis del juego: (vacio si no lo quiere modificar)");
            string synopsis = Validation.ContainsDelimiter("Escriba una nueva sinopsis del juego valida");

            Console.WriteLine("Elija el nuevo ESRBrating del juego:");
            int ESRBRating = Validation.ReadValidESRBModify();

            Console.WriteLine("Elija el nuevo genero del juego:");
            string genre = Validation.ReadValidGenreModify();

            Console.WriteLine("Escriba la nueva dirección del archivo de la caratula: (vacio si no lo quiere modificar)");
            string coverPath = Console.ReadLine();
            if (coverPath.Length == 0) { coverPath = ""; } else { coverPath = Validation.ReadValidPathModify(coverPath, "Escriba un archivo válido", fileHandler); }

            Game gameToModify = new Game
            {
                Title = title,
                Synopsis = synopsis,
                ESRBRating = (Common.ESRBRating)ESRBRating,
                Genre = genre,
                CoverFilePath = coverPath
            };
            string message = commandHandler.SendRequest(gameId, gameToModify);
            Console.WriteLine(message);
            ShowGameInfo(gameId);
        }

        private void DeleteGame(int gameId)
        {
            DeleteGame commandHandler = (DeleteGame)CommandFactory.GetCommandHandler(Command.DELETE_GAME, networkStreamHandler);
            Console.WriteLine("Seguro que quiere eliminar el juego?");
            Console.WriteLine("1.Si");
            Console.WriteLine("2.No");
            int response = Validation.ReadValidNumber("Elija una opción valida", 1, 2);
            if (response == 1)
            {
                string returnMessage = commandHandler.SendRequest(gameId);
                ShowServerMessage(returnMessage);
            }
            else
            {
                ShowGameInfo(gameId);
            }

        }

        public void ShowServerMessage(string message)
        {
            Console.WriteLine(message);
            MainMenu();
        }


        private void TestData()
        {
            PublishGame commandHandler = (PublishGame)CommandFactory.GetCommandHandler(Command.PUBLISH_GAME, networkStreamHandler);
            string currentDict = Directory.GetCurrentDirectory();
            currentDict = currentDict.Remove(currentDict.Length - 31) + "\\";
            Game newGame = new Game
            {
                Title = "PUBG",
                Synopsis = "El pubg",
                ESRBRating = (Common.ESRBRating)4,
                Genre = "Acción",
                CoverFilePath = currentDict + "pubg.jpg"
            };
            commandHandler.SendRequest(newGame);

            newGame = new Game
            {
                Title = "PUBG2",
                Synopsis = "El pubg2",
                ESRBRating = (Common.ESRBRating)5,
                Genre = "Acción",
                CoverFilePath = currentDict + "pubg.jpg"
            };
            commandHandler.SendRequest(newGame);

            newGame = new Game
            {
                Title = "GTA V",
                Synopsis = "GOTY 2021 ",
                ESRBRating = (Common.ESRBRating)0,
                Genre = "Aventura",
                CoverFilePath = currentDict + "pubg.jpg"
            };
            commandHandler.SendRequest(newGame);

            newGame = new Game
            {
                Title = "Pubg 3 - Remastered",
                Synopsis = "Comeback ",
                ESRBRating = (Common.ESRBRating)4,
                Genre = "Acción",
                CoverFilePath = currentDict + "pubg.jpg"
            };
            commandHandler.SendRequest(newGame);

            newGame = new Game
            {
                Title = "FIFA",
                Synopsis = "El fifa",
                ESRBRating = (Common.ESRBRating)2,
                Genre = "Deporte",
                CoverFilePath = currentDict + "pubg.jpg"
            };
            commandHandler.SendRequest(newGame);

            Review newReview = new Review()
            {
                Text = "Juegaso 10/10",
                Rating = 10
            };
            var commandHandler2 = (WriteReview)CommandFactory.GetCommandHandler(Command.WRITE_REVIEW, networkStreamHandler);
            commandHandler2.SendRequest(newReview, 0);

            newReview = new Review()
            {
                Text = "Meh",
                Rating = 5
            };
            commandHandler2 = (WriteReview)CommandFactory.GetCommandHandler(Command.WRITE_REVIEW, networkStreamHandler);
            commandHandler2.SendRequest(newReview, 0);

            newReview = new Review()
            {
                Text = "Podria ser mejor",
                Rating = 6
            };
            commandHandler2 = (WriteReview)CommandFactory.GetCommandHandler(Command.WRITE_REVIEW, networkStreamHandler);
            commandHandler2.SendRequest(newReview, 0);

            newReview = new Review()
            {
                Text = "GOTY",
                Rating = 10
            };
            commandHandler2 = (WriteReview)CommandFactory.GetCommandHandler(Command.WRITE_REVIEW, networkStreamHandler);
            commandHandler2.SendRequest(newReview, 2);

            newReview = new Review()
            {
                Text = "El mejor juego",
                Rating = 2
            };
            commandHandler2 = (WriteReview)CommandFactory.GetCommandHandler(Command.WRITE_REVIEW, networkStreamHandler);
            commandHandler2.SendRequest(newReview, 2);
            MainMenu();
        }


    }
}
