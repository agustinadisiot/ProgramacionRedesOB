using Client.Commands;
using Common.Domain;
using Common.NetworkUtils;
using Common.Protocol;
using System;
using System.Collections.Generic;
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
            Dictionary<string, Action> opciones = new Dictionary<string, Action>();
            opciones.Add("Iniciar Sesión", () => Login());
            CliMenu.showMenu(opciones, "Menu Inicial");

        }
        public void MainMenu()
        {
            Dictionary<string, Action> opciones = new Dictionary<string, Action>();
            opciones.Add("Ver catalogo", () => BrowseCatalogue());
            opciones.Add("Publicar Juego", () => Publish());
            opciones.Add("Buscar por titulo", () => SearchByTitle());
            opciones.Add("Logout", () => Logout());
            opciones.Add("Comprar Juego (sacar)", () => ShowBuyGameMenu());
            opciones.Add("Ver Juego", () => ShowGameInfo());
            opciones.Add("Escribir review", () => ShowWriteReviewMenu());
            opciones.Add("Ver review", () => ShowBrowseReviewsMenu());
            opciones.Add("Salir", () => Console.WriteLine("seguro que quiere salir????!!"));
            opciones.Add("reimprimir", () => CliMenu.showMenu(opciones, "menucito"));
            while (true) // TODO sacar
            {
                CliMenu.showMenu(opciones, "Menuuuu");
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
            Dictionary<string, Action> opciones = new Dictionary<string, Action>();

            foreach (string gameTitle in gamePage.GamesTitles)
            {
                opciones.Add(gameTitle, () => MainMenu());
            }

            if (gamePage.HasNextPage)
                opciones.Add("Siguiene Página", () => ShowSearchByTitlePage(title, gamePage.CurrentPage + 1));

            if (gamePage.HasPreviousPage)
                opciones.Add("Página Anterior", () => ShowSearchByTitlePage(title, gamePage.CurrentPage - 1));

            opciones.Add("Volver al Menu Principal", () => MainMenu());

            CliMenu.showMenu(opciones, $"Juegos con \"{title}\" - Página {gamePage.CurrentPage}");
        }

        private void ShowCataloguePage(GamePage gamePage)
        {
            Dictionary<string, Action> opciones = new Dictionary<string, Action>();

            for (int i = 0; i < gamePage.GamesTitles.Count; i++)
            {
                int idIndex = i;
                opciones.Add(gamePage.GamesTitles[i], () => ShowGameInfo(gamePage.GamesIDs[idIndex]));
            }

            if (gamePage.HasNextPage)
                opciones.Add("Siguiene Página", () => BrowseCatalogue(gamePage.CurrentPage + 1));

            if (gamePage.HasPreviousPage)
                opciones.Add("Página Anterior", () => BrowseCatalogue(gamePage.CurrentPage - 1));

            opciones.Add("Volver al Menu Principal", () => MainMenu());

            CliMenu.showMenu(opciones, $"Catalogo de Juegos Pagina {gamePage.CurrentPage}");
        }

        private void ShowGameInfo(int id)
        {
            Console.WriteLine($"ID del juego {id}");
            MainMenu();
        }

        private void ShowBuyGameMenu(int gameID = 1)
        {
            // TODO sacar writeLine y poner adentro de showGame
            Console.WriteLine("ID del juego: ");
            string TextId = Console.ReadLine();
            gameID = int.Parse(TextId);
            var commandHandler = (BuyGame)CommandFactory.GetCommandHandler(Command.BUY_GAME, networkStreamHandler);
            string message = commandHandler.SendRequest(gameID);
            ShowServerMessage(message);
        }

        private void ShowWriteReviewMenu(int gameID = 1)
        {
            // TODO poner adentro de showGame
            Console.WriteLine("ID del juego: ");
            string textId = Console.ReadLine();
            gameID = int.Parse(textId);

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
        private void ShowBrowseReviewsMenu(int pageNumber = 1, int gameId = 0)
        {
            var commandHandler = (BrowseReviews)CommandFactory.GetCommandHandler(Command.BROWSE_REVIEWS, networkStreamHandler);
            ReviewPage newReviewPage = commandHandler.SendRequest(pageNumber, gameId);
            ShowReviewPage(newReviewPage, gameId);
        }

        private void ShowReviewPage(ReviewPage reviewPage, int gameId)
        {
            Console.WriteLine($"Calificaciones - Página {reviewPage.CurrentPage}");
            Console.WriteLine();// TODO capaz poner el nombre del juego
            Dictionary<string, Action> opciones = new Dictionary<string, Action>();

            foreach (Review review in reviewPage.Reviews)
            {
                Console.WriteLine($"{review.User.Name} ({review.Rating}/5):"); // TODO el 10 que sea una constante, en Common, pero no parte del protocolo
                Console.WriteLine($"{review.Text}"); // TODO ver si implementamos un "Ver mas" si es muy larga 
                Console.WriteLine();
            }

            if (reviewPage.HasNextPage)
                opciones.Add("Siguiene Página", () => ShowBrowseReviewsMenu(reviewPage.CurrentPage + 1, gameId));

            if (reviewPage.HasPreviousPage)
                opciones.Add("Página Anterior", () => ShowBrowseReviewsMenu(reviewPage.CurrentPage - 1, gameId));

            opciones.Add("Volver al Menu Principal", () => MainMenu());

            CliMenu.showMenu(opciones, "");
        }

        private void Publish()
        {
            PublishGame commandHandler = (PublishGame)CommandFactory.GetCommandHandler(Command.PUBLISH_GAME, networkStreamHandler);

            Console.WriteLine("Escriba el Titulo del juego:");
            var word = Console.ReadLine();
            // TODO agregar el resto de los datos y validarlos 


            Game newGame = new Game
            {
                Title = word
                // todo agregar los nuevos datos
            };
            string returnMessage = commandHandler.SendRequest(newGame);
            ShowServerMessage(returnMessage);
        }

        private void ShowGameInfo()
        {
            Console.WriteLine("Escriba el id del juego: ");
            string id = Console.ReadLine();
            ViewGame commandHandler = (ViewGame)CommandFactory.GetCommandHandler(Command.VIEW_GAME, networkStreamHandler);
            commandHandler.SendRequest(id);
            ShowServerMessage("This is the game info");

        }

        public void ShowServerMessage(string message)
        {
            Console.WriteLine(message);
            MainMenu();
        }


    }
}
