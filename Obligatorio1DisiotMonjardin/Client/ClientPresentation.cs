using Client.Commands;
using Common.Domain;
using Common.FileHandler;
using Common.NetworkUtils;
using Common.Protocol;
using Common.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Client
{
    public class ClientPresentation
    {
        private NetworkStreamHandler networkStreamHandler;
        private readonly Socket socketClient;
        private readonly IPEndPoint serverIpEndPoint;
        private readonly FileHandler fileHandler;

        public ClientPresentation()
        {
            socketClient = ClientNetworkUtil.GetNewSocketClient();
            serverIpEndPoint = ClientNetworkUtil.GetServerEndpoint();
            fileHandler = new FileHandler();
        }
        public async Task StartConnection()
        {
            try
            {
                await socketClient.ConnectAsync(serverIpEndPoint);
            }
            catch (SocketException)
            {
                Console.WriteLine("No se pudo conectar con el servidor");
                Environment.Exit(0);
            }

            networkStreamHandler = new NetworkStreamHandler(new NetworkStream(socketClient));
        }
        private async Task EndConnection()
        {
            Console.WriteLine("¿Seguro que quiere cerrar la conexión?");
            Console.WriteLine("1.Si");
            Console.WriteLine("2.No");
            int input = Validation.ReadValidNumber("Ingrese una opción correcta", 1, 2);
            if (input == 1)
            {
                var commandHandler = (Exit)CommandFactory.GetCommandHandler(Command.EXIT, networkStreamHandler);
                await commandHandler.SendRequest();
                socketClient.Shutdown(SocketShutdown.Both);
                socketClient.Close();
            }
            else { await StartMenu(); }
        }

        public async Task StartMenu()
        {
            Dictionary<string, Func<Task>> menuOptions = new Dictionary<string, Func<Task>>
            {
                { "Iniciar Sesión", Login },
                { "Salir", EndConnection }
            };
            try
            {
                await CliMenu.showMenu(menuOptions, "Menú Inicial");
            }
            catch (ServerError e)
            {
                await HandleServerError(e.Message);
            }
            catch (ServerShutDownException)
            {
                HandleServerShutDown();
            }
        }

        public async Task MainMenu()
        {
            Dictionary<string, Func<Task>> menuOptions = new Dictionary<string, Func<Task>>
            {
                { "Ver catálogo", async() => await BrowseCatalogue() },
                { "Publicar Juego", async () => await Publish() },
                { "Buscar por título", async() => await SearchByTitle() },
                { "Buscar por género", async() => await SearchByGenre() },
                { "Buscar por clasificación", async() => await SearchByRating() },
                { "Ver mis juegos", async() => await BrowseMyGames() },
                { "Cerrar Sesión", async () => await Logout() },
                { "Menú de desarrollador", async() => await DeveloperMenu() },
            };
            Console.WriteLine();
            try
            {
                await CliMenu.showMenu(menuOptions, "Menú");
            }
            catch (ServerError e)
            {
                await HandleServerError(e.Message);
            }
            catch (ServerShutDownException)
            {
                HandleServerShutDown();
            }
        }
        private async Task DeveloperMenu()
        {
            // Se dejó para mostrar en la defensa y para facilitar la correción
            Dictionary<string, Func<Task>> developerOptions = new Dictionary<string, Func<Task>>
            {
                { "Enviar parámetro incorrecto", async () => await BrowseMyGames(-1) },
                { "Cargar datos de prueba", async () => await TestData() },
                { "Volver al menú principal", async () => await MainMenu() },
            };
            await CliMenu.showMenu(developerOptions, "Menú de desarrollador");
        }



        private void HandleServerShutDown()
        {
            Console.WriteLine("El servidor se cerró");
            Console.WriteLine("Cerrando cliente ...");
            Console.WriteLine();
        }

        private async Task HandleServerError(string message)
        {
            Console.WriteLine("Ocurrió un error con el servidor");
            Console.WriteLine($"Error: {message}");
            Console.WriteLine();
            await MainMenu();
        }


        private async Task Login()
        {
            Console.WriteLine("Ingrese nombre de usuario: ");
            string username = Validation.ReadValidString("Reingrese un nombre de usuario válido");
            var commandHandler = (Login)CommandFactory.GetCommandHandler(Command.LOGIN, networkStreamHandler);
            bool firstTimeUser = await commandHandler.SendRequest(username);
            Console.WriteLine("Se inició sesión correctamente");
            if (firstTimeUser)
                Console.WriteLine("Bienvenido por primera vez!");
            else
                Console.WriteLine("Bienvenido de vuelta!");
            await MainMenu();

        }

        private async Task Logout()
        {
            var commandHandler = (Logout)CommandFactory.GetCommandHandler(Command.LOGOUT, networkStreamHandler);
            bool success = await commandHandler.SendRequest();
            if (success)
            {
                Console.WriteLine("Se cerró sesión correctamente");
                await StartMenu();
            }
            else
            {
                Console.WriteLine("No se pudo cerrar sesión");
                await MainMenu();
            }

        }

        private async Task BrowseCatalogue(int pageNumber = 1)
        {
            BrowseCatalogue commandHandler = (BrowseCatalogue)CommandFactory.GetCommandHandler(Command.BROWSE_CATALOGUE, networkStreamHandler);
            GamePage newGamePage = await commandHandler.SendRequest(pageNumber);

            Func<Task> nextPageOption = async () => await BrowseCatalogue(pageNumber + 1);
            Func<Task> previousPageOption = async () => await BrowseCatalogue(pageNumber - 1);
            string title = $"Catálogo de Juegos  - Página {pageNumber}";
            await ShowGamePage(newGamePage, title, nextPageOption, previousPageOption);
        }

        private async Task BrowseMyGames(int pageNumber = 1)
        {
            BrowseMyGames commandHandler = (BrowseMyGames)CommandFactory.GetCommandHandler(Command.BROWSE_MY_GAMES, networkStreamHandler);
            GamePage newGamePage = await commandHandler.SendRequest(pageNumber);

            Func<Task> nextPageOption = async () => await BrowseMyGames(pageNumber + 1);
            Func<Task> previousPageOption = async () => await BrowseMyGames(pageNumber - 1);
            string title = $"Mis Juegos - Página {pageNumber}";
            await ShowGamePage(newGamePage, title, nextPageOption, previousPageOption);
        }

        private async Task SearchByTitle()
        {
            Console.WriteLine("Escriba el título del juego: ");
            string title = Validation.ReadValidString("Escriba un título válido");
            await ShowSearchByTitlePage(title);
        }

        public async Task ShowSearchByTitlePage(string title, int pageNumber = 1)
        {
            var commandHandler = (SearchByTitle)CommandFactory.GetCommandHandler(Command.SEARCH_BY_TITLE, networkStreamHandler);
            GamePage newGamePage = await commandHandler.SendRequest(pageNumber, title);

            Func<Task> nextPageOption = async () => await ShowSearchByTitlePage(title, pageNumber + 1);
            Func<Task> previousPageOption = async () => await ShowSearchByTitlePage(title, pageNumber - 1);
            string gamePageTitle = $"Juegos con \"{title}\" - Página {pageNumber}";
            await ShowGamePage(newGamePage, gamePageTitle, nextPageOption, previousPageOption);
        }

        private async Task SearchByRating()
        {
            Console.WriteLine("Escriba la clasificación mínima del juego: ");
            int minRating = Validation.ReadValidNumber(@$"Escriba un número entre {LogicSpecification.MIN_RATING} y {LogicSpecification.MAX_RATING}", LogicSpecification.MIN_RATING, LogicSpecification.MAX_RATING);
            await ShowSearchByRatingPage(minRating);
        }

        public async Task ShowSearchByRatingPage(int minRating, int pageNumber = 1)
        {
            var commandHandler = (SearchByRating)CommandFactory.GetCommandHandler(Command.SEARCH_BY_RATING, networkStreamHandler);
            GamePage newGamePage = await commandHandler.SendRequest(pageNumber, minRating);

            Func<Task> nextPageOption = async () => await ShowSearchByRatingPage(minRating, pageNumber + 1);
            Func<Task> previousPageOption = async () => await ShowSearchByRatingPage(minRating, pageNumber - 1);
            string title = $"Juegos con clasificación {minRating} o más - Página {pageNumber}";
            await ShowGamePage(newGamePage, title, nextPageOption, previousPageOption);
        }

        private async Task SearchByGenre()
        {
            Console.WriteLine("Elija el género que quiera: ");
            string genre = Validation.ReadValidGenre();
            await ShowSearchByGenrePage(genre);
        }

        public async Task ShowSearchByGenrePage(string genre, int pageNumber = 1)
        {
            var commandHandler = (SearchByGenre)CommandFactory.GetCommandHandler(Command.SEARCH_BY_GENRE, networkStreamHandler);
            GamePage newGamePage = await commandHandler.SendRequest(pageNumber, genre);

            Func<Task> nextPageOption = async () => await ShowSearchByGenrePage(genre, pageNumber + 1);
            Func<Task> previousPageOption = async () => await ShowSearchByGenrePage(genre, pageNumber - 1);
            string title = $"Juegos de {genre}  - Página {pageNumber}";

            await ShowGamePage(newGamePage, title, nextPageOption, previousPageOption);
        }

        private async Task ShowGamePage(GamePage gamePage, string title, Func<Task> nextPage, Func<Task> previousPage)
        {
            Dictionary<string, Func<Task>> menuOptions = new Dictionary<string, Func<Task>>();

            for (int i = 0; i < gamePage.GamesTitles.Count; i++)
            {
                int idIndex = i;
                menuOptions.Add(gamePage.GamesTitles[i], async () => await ShowGameInfo(gamePage.GamesIds[idIndex]));
            }

            if (gamePage.HasNextPage)
                menuOptions.Add("Siguiene Página", () => nextPage());

            if (gamePage.HasPreviousPage)
                menuOptions.Add("Página Anterior", () => previousPage());

            menuOptions.Add("Volver al Menú Principal", async () => await MainMenu());

            await CliMenu.showMenu(menuOptions, title);
        }


        private async Task ShowGameInfo(int gameId)
        {
            ViewGame commandHandler = (ViewGame)CommandFactory.GetCommandHandler(Command.VIEW_GAME, networkStreamHandler);
            string gameID = gameId.ToString();
            GameView gameInfo = await commandHandler.SendRequest(gameID);

            Console.WriteLine();
            Console.WriteLine($"Título: {gameInfo.Game.Title}");
            Console.WriteLine($"Sinopsis: {gameInfo.Game.Synopsis}");
            if (gameInfo.Game.ReviewsRating == 0) { Console.WriteLine($"Calificación: -"); }
            else { Console.WriteLine($"Calificación: {gameInfo.Game.ReviewsRating}"); }
            Console.WriteLine($"Clasificación ESRB: {gameInfo.Game.ESRBRating}");
            Console.WriteLine($"Género: {gameInfo.Game.Genre}");

            Dictionary<string, Func<Task>> menuOptions = new Dictionary<string, Func<Task>>();
            if (!gameInfo.IsOwned) menuOptions.Add("Comprar Juego", async () => await BuyGame(gameId));
            menuOptions.Add("Ver Reseñas", async () => await ShowBrowseReviewsMenu(1, gameId));
            if (gameInfo.IsOwned) menuOptions.Add("Escribir Reseña", async () => await ShowWriteReviewMenu(gameId));
            if (gameInfo.IsPublisher)
            {
                menuOptions.Add("Modificar Juego", async () => await ModifyGame(gameId));
                menuOptions.Add("Eliminar Juego", async () => await DeleteGame(gameId));
            }
            menuOptions.Add("Descargar Carátula", async () => await DownloadCover(gameId));
            menuOptions.Add("Volver al Menú Pricipal", async () => await MainMenu());

            await CliMenu.showMenu(menuOptions, "");

        }

        private async Task DownloadCover(int gameId)
        {
            Console.WriteLine("Escriba la carpeta donde quiere guardar la caratula");
            string folderPath = await Validation.ReadValidDirectory("No se encontro tal carpeta, ingrese de nuevo", fileHandler);
            Console.WriteLine("Escriba el nombre que quiere para el archivo (sin la extensión)");
            string fileName = await Validation.ReadValidFileName("Escriba un nombre válido para su archivo", folderPath, fileHandler);


            var commandHandler = (DownloadCover)CommandFactory.GetCommandHandler(Command.DOWNLOAD_COVER, networkStreamHandler);
            string completePath = await commandHandler.SendRequest(gameId, folderPath, fileName);

            await Validation.CouldDownload(completePath, fileHandler);
            await ShowGameInfo(gameId);
        }

        private async Task BuyGame(int gameId)
        {
            var commandHandler = (BuyGame)CommandFactory.GetCommandHandler(Command.BUY_GAME, networkStreamHandler);
            string message = await commandHandler.SendRequest(gameId);
            Console.Write(message);
            await ShowGameInfo(gameId);
        }

        private async Task ShowWriteReviewMenu(int gameId)
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
            string message = await commandHandler.SendRequest(newReview, gameId);
            Console.WriteLine(message);
            await ShowGameInfo(gameId);
        }

        private async Task ShowBrowseReviewsMenu(int pageNumber, int gameId)
        {
            var commandHandler = (BrowseReviews)CommandFactory.GetCommandHandler(Command.BROWSE_REVIEWS, networkStreamHandler);
            ReviewPage newReviewPage = await commandHandler.SendRequest(pageNumber, gameId);
            await ShowReviewPage(newReviewPage, gameId);
        }

        private async Task ShowReviewPage(ReviewPage reviewPage, int gameId)
        {
            Console.WriteLine($"Calificaciones - Página {reviewPage.CurrentPage}");
            Console.WriteLine();
            Dictionary<string, Func<Task>> menuOptions = new Dictionary<string, Func<Task>>();

            foreach (Review review in reviewPage.Reviews)
            {
                Console.WriteLine($"{review.Author.Name} ({review.Rating}/{LogicSpecification.MAX_RATING}):");
                Console.WriteLine($"{review.Text}");
                Console.WriteLine();
            }

            if (reviewPage.HasNextPage)
                menuOptions.Add("Siguiene Página", async () => await ShowBrowseReviewsMenu(reviewPage.CurrentPage + 1, gameId));

            if (reviewPage.HasPreviousPage)
                menuOptions.Add("Página Anterior", async () => await ShowBrowseReviewsMenu(reviewPage.CurrentPage - 1, gameId));

            menuOptions.Add("Volver al Menú Del Juego", async () => await ShowGameInfo(gameId));
            menuOptions.Add("Volver al Menú Principal", async () => await MainMenu());

            await CliMenu.showMenu(menuOptions, "");
        }

        private async Task Publish()
        {
            PublishGame commandHandler = (PublishGame)CommandFactory.GetCommandHandler(Command.PUBLISH_GAME, networkStreamHandler);
            Console.WriteLine("Escriba el título del juego:");
            string title = Validation.ReadValidString("Escriba un título del juego válido");

            Console.WriteLine("Escriba la sinopsis del juego:");
            string synopsis = Validation.ReadValidString("Escriba una sinopsis del juego válida");

            Console.WriteLine("Elija el ESRBrating del juego:");
            int ESRBRating = Validation.ReadValidESRB();

            Console.WriteLine("Elija el género del juego:");
            string genre = Validation.ReadValidGenre();

            Console.WriteLine("Escriba la dirección del archivo de la carátula:");
            string coverPath = await Validation.ReadValidPath("Escriba un archivo válido", fileHandler);

            Game newGame = new Game
            {
                Title = title,
                Synopsis = synopsis,
                ESRBRating = (Common.ESRBRating)ESRBRating,
                Genre = genre,
                CoverFilePath = coverPath
            };
            string returnMessage = await commandHandler.SendRequest(newGame);
            await ShowServerMessage(returnMessage);
        }

        private async Task ModifyGame(int gameId)
        {
            ModifyGame commandHandler = (ModifyGame)CommandFactory.GetCommandHandler(Command.MODIFY_GAME, networkStreamHandler);
            Console.WriteLine("Escriba el nuevo título del juego: (vacío si no lo quiere modificar)");
            string title = Validation.ContainsDelimiter("Escriba un nuevo título del juego válido");

            Console.WriteLine("Escriba la nueva sinopsis del juego: (vacío si no lo quiere modificar)");
            string synopsis = Validation.ContainsDelimiter("Escriba una nueva sinopsis del juego válida");

            Console.WriteLine("Elija el nuevo ESRBrating del juego: (vacío si no lo quiere modificar)");
            int ESRBRating = Validation.ReadValidESRBModify();

            Console.WriteLine("Elija el nuevo genero del juego: (vacío si no lo quiere modificar)");
            string genre = Validation.ReadValidGenreModify();

            Console.WriteLine("Escriba la nueva dirección del archivo de la carátula: (vacio si no lo quiere modificar)");
            string coverPath = Console.ReadLine();
            if (coverPath.Length == 0) { coverPath = ""; } else { coverPath = await Validation.ReadValidPathModify(coverPath, "Escriba un archivo válido", fileHandler); }

            Game gameToModify = new Game
            {
                Title = title,
                Synopsis = synopsis,
                ESRBRating = (Common.ESRBRating)ESRBRating,
                Genre = genre,
                CoverFilePath = coverPath
            };
            string message = await commandHandler.SendRequest(gameId, gameToModify);
            Console.WriteLine(message);
            await ShowGameInfo(gameId);
        }

        private async Task DeleteGame(int gameId)
        {
            DeleteGame commandHandler = (DeleteGame)CommandFactory.GetCommandHandler(Command.DELETE_GAME, networkStreamHandler);
            Console.WriteLine("¿Seguro que quiere eliminar el juego?");
            Console.WriteLine("1.Si");
            Console.WriteLine("2.No");
            int response = Validation.ReadValidNumber("Elija una opción válida", 1, 2);
            if (response == 1)
            {
                string returnMessage = await commandHandler.SendRequest(gameId);
                await ShowServerMessage(returnMessage);
            }
            else
            {
                await ShowGameInfo(gameId);
            }

        }
        public async Task ShowServerMessage(string message)
        {
            Console.WriteLine(message);
            await MainMenu();
        }

        private async Task TestData()
        {
            PublishGame commandHandler = (PublishGame)CommandFactory.GetCommandHandler(Command.PUBLISH_GAME, networkStreamHandler);
            string currentDict = Directory.GetCurrentDirectory();
            currentDict = Path.GetFullPath(Path.Combine(currentDict, @"..\..\..\..\"));
            Game newGame = new Game
            {
                Title = "PUBG",
                Synopsis = "El pubg",
                ESRBRating = (Common.ESRBRating)4,
                Genre = "Acción",
                CoverFilePath = currentDict + "pubg.jpg"
            };
            await commandHandler.SendRequest(newGame);

            newGame = new Game
            {
                Title = "PUBG2",
                Synopsis = "El pubg2",
                ESRBRating = (Common.ESRBRating)5,
                Genre = "Acción",
                CoverFilePath = currentDict + "pubg.jpg"
            };
            await commandHandler.SendRequest(newGame);

            newGame = new Game
            {
                Title = "GTA V",
                Synopsis = "GOTY 2021 ",
                ESRBRating = (Common.ESRBRating)0,
                Genre = "Aventura",
                CoverFilePath = currentDict + "pubg.jpg"
            };
            await commandHandler.SendRequest(newGame);

            newGame = new Game
            {
                Title = "Pubg 3 - Remastered",
                Synopsis = "Comeback ",
                ESRBRating = (Common.ESRBRating)4,
                Genre = "Acción",
                CoverFilePath = currentDict + "pubg.jpg"
            };
            await commandHandler.SendRequest(newGame);

            newGame = new Game
            {
                Title = "FIFA",
                Synopsis = "El fifa",
                ESRBRating = (Common.ESRBRating)2,
                Genre = "Deporte",
                CoverFilePath = currentDict + "pubg.jpg"
            };
            await commandHandler.SendRequest(newGame);

            Review newReview = new Review()
            {
                Text = "Juegaso 10/10",
                Rating = 10
            };
            var commandHandler2 = (WriteReview)CommandFactory.GetCommandHandler(Command.WRITE_REVIEW, networkStreamHandler);
            await commandHandler2.SendRequest(newReview, 0);

            newReview = new Review()
            {
                Text = "Meh",
                Rating = 5
            };
            commandHandler2 = (WriteReview)CommandFactory.GetCommandHandler(Command.WRITE_REVIEW, networkStreamHandler);
            await commandHandler2.SendRequest(newReview, 0);

            newReview = new Review()
            {
                Text = "Podria ser mejor",
                Rating = 6
            };
            commandHandler2 = (WriteReview)CommandFactory.GetCommandHandler(Command.WRITE_REVIEW, networkStreamHandler);
            await commandHandler2.SendRequest(newReview, 0);

            newReview = new Review()
            {
                Text = "GOTY",
                Rating = 10
            };
            commandHandler2 = (WriteReview)CommandFactory.GetCommandHandler(Command.WRITE_REVIEW, networkStreamHandler);
            await commandHandler2.SendRequest(newReview, 2);

            newReview = new Review()
            {
                Text = "El mejor juego",
                Rating = 2
            };
            commandHandler2 = (WriteReview)CommandFactory.GetCommandHandler(Command.WRITE_REVIEW, networkStreamHandler);
            await commandHandler2.SendRequest(newReview, 2);
            await MainMenu();
        }


    }
}
