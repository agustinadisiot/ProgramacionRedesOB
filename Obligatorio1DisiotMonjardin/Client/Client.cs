using Client.Commands;
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
    public class Client
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
                { "Iniciar Sesión", () => Login() }
            };
            CliMenu.showMenu(menuOptions, "Menu Inicial");

        }
        public void MainMenu()
        {
            Dictionary<string, Action> menuOptions = new Dictionary<string, Action>
            {
                { "Ver catalogo", () => BrowseCatalogue() },
                { "Publicar Juego", () => Publish() },
                { "Buscar por titulo", () => SearchByTitle() },
                { "Buscar por género", () => SearchByGenre() },
                { "Buscar por clasificación", () => SearchByRating() },
                { "Ver mis juegos", () => BrowseMyGames() },
                { "Datos de prueba", () => TestData() },
                { "Logout", () => Logout() },
                { "Salir", () => Console.WriteLine("seguro que quiere salir????!!") }
            };
            while (true) // TODO sacar
            {
                CliMenu.showMenu(menuOptions, "Menu");
            }
        }


        private void Login()
        {
            Console.WriteLine("Ingrese nombre de usuario: ");
            string username = Validation.ReadValidString("Reingrese un nombre de usuario valido");

            var commandHandler = (Login)CommandFactory.GetCommandHandler(Command.LOGIN, networkStreamHandler);
            commandHandler.SendRequest(username);
            MainMenu();
        }

        private void Logout()
        {
            var commandHandler = (Logout)CommandFactory.GetCommandHandler(Command.LOGOUT, networkStreamHandler);
            commandHandler.SendRequest();
            StartMenu();
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
            Console.WriteLine("Escriba el titulo del juego: ");
            string title = Validation.ReadValidString("Escriba un titulo valido");
            ShowSearchByTitlePage(title);
        }

        public void ShowSearchByTitlePage(string title, int pageNumber = 1)
        {
            var commandHandler = (SearchByTitle)CommandFactory.GetCommandHandler(Command.SEARCH_BY_TITLE, networkStreamHandler);
            GamePage newGamePage = commandHandler.SendRequest(pageNumber, title);

            Action nextPageOption = () => ShowSearchByTitlePage(title, pageNumber + 1);
            Action previousPageOption = () => ShowSearchByTitlePage(title, pageNumber - 1);
            ShowGamePage(newGamePage, title, nextPageOption, previousPageOption);
        }
        private void SearchByRating()
        {
            Console.WriteLine("Escriba la clasificación minima del juego: ");
            int minRating = Validation.ReadValidNumber("Escriba un número entre 1 y 10", 1, 10);
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
            string genre = Validation.ReadValidString("READ VALID GENRE TODO ALFJADKLS"); // TODO
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
                menuOptions.Add(gamePage.GamesTitles[i], () => ShowGameInfo(gamePage.GamesIDs[idIndex]));
            }

            if (gamePage.HasNextPage)
                menuOptions.Add("Siguiene Página", () => nextPage());

            if (gamePage.HasPreviousPage)
                menuOptions.Add("Página Anterior", () => previousPage());

            menuOptions.Add("Volver al Menu Principal", () => MainMenu());

            CliMenu.showMenu(menuOptions, title);
        }


        private void ShowGameInfo(int id)
        {
            ViewGame commandHandler = (ViewGame)CommandFactory.GetCommandHandler(Command.VIEW_GAME, networkStreamHandler);
            string gameID = id.ToString();
            GameView gameInfo = commandHandler.SendRequest(gameID);
            Console.WriteLine($"Titulo: {gameInfo.Game.Title}");
            Console.WriteLine($"Sinopsis: {gameInfo.Game.Synopsis}");
            if (gameInfo.Game.ReviewsRating == 0) { Console.WriteLine($"Calificacion: -"); }
            else { Console.WriteLine($"Calificacion: {gameInfo.Game.ReviewsRating}"); }
            Console.WriteLine($"Clasificacion ESRB: {gameInfo.Game.ESRBRating}");
            Console.WriteLine($"Genero: {gameInfo.Game.Genre}");
            Dictionary<string, Action> menuOptions = new Dictionary<string, Action>();
            if (!gameInfo.IsOwned) menuOptions.Add("Comprar Juego", () => ShowBuyGameMenu(id));
            menuOptions.Add("Ver Reviews", () => ShowBrowseReviewsMenu(1, id)); //todooooo
            if (gameInfo.IsOwned) menuOptions.Add("Escribir Review", () => ShowWriteReviewMenu(id));
            if (gameInfo.IsPublisher)
            {
                menuOptions.Add("Modificar Juego", () => MainMenu()); //todo
                menuOptions.Add("Eliminar Juego", () => MainMenu()); //todo
            }
            menuOptions.Add("Descargar Caratula", () => DownloadCover(id));
            menuOptions.Add("Volver al Menu Principal", () => MainMenu());

            CliMenu.showMenu(menuOptions, "");

        }

        private void DownloadCover(int gameId)
        {
            Console.WriteLine("Escriba la carpeta donde quiere guardar la caratula");
            string folderPath = Validation.ReadValidDirectory("No se encontro tal carpeta, ingrese de nuevo", fileHandler);
            Console.WriteLine("Escriba el nombre que quiere para el archivo");
            string fileName = Validation.ReadValidString("Escriba un nombre valido para su archivo");

            var commandHandler = (DownloadCover)CommandFactory.GetCommandHandler(Command.DOWNLOAD_COVER, networkStreamHandler);
            string completePath = commandHandler.SendRequest(gameId, folderPath, fileName);

            Validation.CouldDownload(completePath, fileHandler);
            ShowGameInfo(gameId);
        }

        private void ShowBuyGameMenu(int gameID)
        {
            var commandHandler = (BuyGame)CommandFactory.GetCommandHandler(Command.BUY_GAME, networkStreamHandler);
            string message = commandHandler.SendRequest(gameID);
            ShowServerMessage(message);
        }

        private void ShowWriteReviewMenu(int gameID)
        {
            Console.WriteLine("Escriba una puntuación: (del 1 al 10)");
            int rating = Validation.ReadValidNumber("Escriba una puntuación: (del 1 al 10)", 1, 10);

            Console.WriteLine("Escriba un comentario ");
            string comment = Validation.ReadValidString("Escriba un comentario valido");

            Review newReview = new Review()
            {
                Text = comment,
                Rating = rating
            };
            var commandHandler = (WriteReview)CommandFactory.GetCommandHandler(Command.WRITE_REVIEW, networkStreamHandler);
            string message = commandHandler.SendRequest(newReview, gameID);
            ShowServerMessage(message);
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
            Console.WriteLine();// TODO capaz poner el nombre del juego
            Dictionary<string, Action> menuOptions = new Dictionary<string, Action>();

            foreach (Review review in reviewPage.Reviews)
            {
                Console.WriteLine($"{review.User.Name} ({review.Rating}/10):"); // TODO el 10 que sea una constante, en Common, pero no parte del protocolo
                Console.WriteLine($"{review.Text}"); // TODO ver si implementamos un "Ver mas" si es muy larga 
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
            Console.WriteLine("Escriba el titulo del juego:");
            string title = Validation.ReadValidString("Escriba un titulo del juego valido");

            Console.WriteLine("Escriba la sinopsis del juego:");
            string synopsis = Validation.ReadValidString("Escriba una sinopsis del juego valida");

            Console.WriteLine("Elija el ESRBrating del juego:");
            int ESRBRating = Validation.ReadValidESRB();

            Console.WriteLine("Elija el genero del juego:");
            string genre = Validation.ReadValidGenre();

            Console.WriteLine("Escriba la dirección del archivo de la caratula:");
            string coverPath = Validation.ReadValidPath("Escriba un archivo valido", fileHandler);

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
                Genre = "Deporte",
                CoverFilePath = currentDict + "pubg.jpg"
            };
            commandHandler.SendRequest(newGame);

            newGame = new Game
            {
                Title = "My little Pony",
                Synopsis = "GOTY 2021 ",
                ESRBRating = (Common.ESRBRating)0,
                Genre = "Deporte",
                CoverFilePath = currentDict + "pubg.jpg"
            };
            commandHandler.SendRequest(newGame);

            newGame = new Game
            {
                Title = "Pubg 3 - Remastered",
                Synopsis = "Comback ",
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
        }


    }
}
