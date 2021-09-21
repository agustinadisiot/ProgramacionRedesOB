using Client.Commands;
using Common;
using Common.Domain;
using Common.NetworkUtils;
using Common.Protocol;
using Server;
using System;
using System.Collections.Generic;
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

        public Client()
        {
            var clientIpEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0); // Puerto 0 -> usa el primer puerto disponible
            tcpClient = new TcpClient(clientIpEndPoint);
            serverIpEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6000); // TODO usar config files

        }
        public void StartConnection()
        {
            tcpClient.Connect(serverIpEndPoint);
            networkStreamHandler = new NetworkStreamHandler(tcpClient.GetStream());
        }

        public void StartMenu()
        {
            Dictionary<string, Action> menuOptions = new Dictionary<string, Action>();
            menuOptions.Add("Iniciar Sesión", () => Login());
            CliMenu.showMenu(menuOptions, "Menu Inicial");

        }
        public void MainMenu()
        {
            Dictionary<string, Action> menuOptions = new Dictionary<string, Action>();
            menuOptions.Add("Ver catalogo", () => BrowseCatalogue());
            menuOptions.Add("Publicar Juego", () => Publish());
            menuOptions.Add("Buscar por titulo", () => SearchByTitle());
            menuOptions.Add("Logout", () => Logout());
            menuOptions.Add("Salir", () => Console.WriteLine("seguro que quiere salir????!!"));
            menuOptions.Add("reimprimir", () => CliMenu.showMenu(menuOptions, "menucito"));
            while (true) // TODO sacar
            {
                CliMenu.showMenu(menuOptions, "Menu");
            }
        }


        private void Login()
        {
            Console.WriteLine("Ingrese nombre de usuario: ");
            string username = Console.ReadLine();
            var commandHandler = (Login)CommandFactory.GetCommandHandler(Command.LOGIN, networkStreamHandler);
            commandHandler.SendRequest(username);
            // TODO separar createUser y log in
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
            ShowCataloguePage(newGamePage);
        }

        private void SearchByTitle()
        {
            Console.WriteLine("Escriba el titulo del juego: ");
            string title = Console.ReadLine();
            // TODO pedir titulo devuelta si es vacio
            ShowSearchByTitlePage(title);
        }
      
        public void ShowSearchByTitlePage(string title, int pageNumber = 1)
        {
            var commandHandler = (SearchByTitle)CommandFactory.GetCommandHandler(Command.SEARCH_BY_TITLE, networkStreamHandler);
            GamePage newGamePage = commandHandler.SendRequest(pageNumber, title);
            ShowSearchByTitlePage(newGamePage, title);
        }

        private void ShowSearchByTitlePage(GamePage gamePage, string title)
        {
            Dictionary<string, Action> menuOptions = new Dictionary<string, Action>();

            foreach (string gameTitle in gamePage.GamesTitles)
            {
                menuOptions.Add(gameTitle, () => MainMenu());
            }

            if (gamePage.HasNextPage)
                menuOptions.Add("Siguiene Página", () => ShowSearchByTitlePage(title, gamePage.CurrentPage + 1));

            if (gamePage.HasPreviousPage)
                menuOptions.Add("Página Anterior", () => ShowSearchByTitlePage(title, gamePage.CurrentPage - 1));

            menuOptions.Add("Volver al Menu Principal", () => MainMenu());

            CliMenu.showMenu(menuOptions, $"Juegos con \"{title}\" - Página {gamePage.CurrentPage}");
        }

        private void ShowCataloguePage(GamePage gamePage)
        {
            Dictionary<string, Action> menuOptions = new Dictionary<string, Action>();

            for (int i = 0; i < gamePage.GamesTitles.Count; i++)
            {
                int idIndex = i;
                menuOptions.Add(gamePage.GamesTitles[i], () => ShowGameInfo(gamePage.GamesIDs[idIndex]));
            }

            if (gamePage.HasNextPage)
                menuOptions.Add("Siguiene Página", () => BrowseCatalogue(gamePage.CurrentPage + 1));

            if (gamePage.HasPreviousPage)
                menuOptions.Add("Página Anterior", () => BrowseCatalogue(gamePage.CurrentPage - 1));

            menuOptions.Add("Volver al Menu Principal", () => MainMenu());

            CliMenu.showMenu(menuOptions, $"Catalogo de Juegos Pagina {gamePage.CurrentPage}");
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
            menuOptions.Add("Desgargar Caratula", () => MainMenu());
            menuOptions.Add("Volver al Menu Del Juego", () => ShowGameInfo(id));
            menuOptions.Add("Volver al Menu Principal", () => MainMenu());

            CliMenu.showMenu(menuOptions, "");

        }
      
        private void ShowBuyGameMenu(int gameID)
        {
            var commandHandler = (BuyGame)CommandFactory.GetCommandHandler(Command.BUY_GAME, networkStreamHandler);
            string message = commandHandler.SendRequest(gameID);
            ShowServerMessage(message);
        }

        private void ShowWriteReviewMenu(int gameID)
        {
            Console.WriteLine("Escriba una puntuación: (del 1 al 5)");
            string textRating = Console.ReadLine();
            int rating = int.Parse(textRating);

          
            Console.WriteLine("Escriba un comentario ");
            string comment = Console.ReadLine();

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
                Console.WriteLine($"{review.User.Name} ({review.Rating}/5):"); // TODO el 10 que sea una constante, en Common, pero no parte del protocolo
                Console.WriteLine($"{review.Text}"); // TODO ver si implementamos un "Ver mas" si es muy larga 
                Console.WriteLine();
            }

            if (reviewPage.HasNextPage)
                menuOptions.Add("Siguiene Página", () => ShowBrowseReviewsMenu(reviewPage.CurrentPage + 1, gameId));

            if (reviewPage.HasPreviousPage)
                menuOptions.Add("Página Anterior", () => ShowBrowseReviewsMenu(reviewPage.CurrentPage - 1, gameId));

            menuOptions.Add("Volver al Menu Principal", () => MainMenu());

            CliMenu.showMenu(menuOptions, "");
        }

        private void Publish()
        {
            PublishGame commandHandler = (PublishGame)CommandFactory.GetCommandHandler(Command.PUBLISH_GAME, networkStreamHandler);

            Console.WriteLine("Escriba el titulo del juego:");
            string stringTitle = Console.ReadLine();
            bool isValidTitle = stringTitle.Length > 0; //y que no incluya '/'
            while (!isValidTitle)
            {
                Console.WriteLine("Escriba un titulo valido");
                stringTitle = Console.ReadLine();
                isValidTitle = stringTitle.Length > 0;
            }
            Console.WriteLine("Escriba la sinopsis del juego:");
            string stringSyn = Console.ReadLine();
            bool isValidSyn = stringSyn.Length > 0;
            while (!isValidSyn)
            {
                Console.WriteLine("Escriba una sinopsis valida");
                stringSyn = Console.ReadLine();
                isValidSyn = stringSyn.Length > 0;
            }
            Console.WriteLine("Elija el ESRBrating del juego:");
            var possibleESRB = Enum.GetValues(typeof(ESRBRating)).Cast<ESRBRating>().ToList();
            for (int i = 0; i < possibleESRB.Count; i++)
            {
                Console.WriteLine($"{ i + 1}.{ possibleESRB.ElementAt(i)}");
            }
            int intESRB = 0;
            bool isANumber = int.TryParse(Console.ReadLine(), out intESRB);
            while (!isANumber)
            {
                Console.WriteLine($"Elija un numero entre 1 y {possibleESRB.Count}");
                isANumber = int.TryParse(Console.ReadLine(), out intESRB);
            }
            bool isValidESRB = intESRB > 0 && intESRB <= possibleESRB.Count;
            while (!isValidESRB)
            {
                Console.WriteLine($"Elija un numero entre 1 y {possibleESRB.Count}");
                isANumber = int.TryParse(Console.ReadLine(), out intESRB);
                isValidESRB = intESRB > 0 && intESRB <=  possibleESRB.Count;
            }
            Console.WriteLine("Escriba el genero del juego:");
            var stringGenre = Console.ReadLine();
            bool isValidGenre = stringGenre.Length > 0;
            while (!isValidGenre)
            {
                Console.WriteLine("Escriba un titulo valido");
                stringGenre = Console.ReadLine();
                isValidGenre = stringGenre.Length > 0;
            }

            Game newGame = new Game
            {
                Title = stringTitle, 
                Synopsis = stringSyn,
                ESRBRating = (Common.ESRBRating)intESRB, 
                Genre = stringGenre
            };
            string returnMessage = commandHandler.SendRequest(newGame);
            ShowServerMessage(returnMessage);
        }

        public void ShowServerMessage(string message)
        {
            Console.WriteLine(message);
            MainMenu();
        }


    }
}
