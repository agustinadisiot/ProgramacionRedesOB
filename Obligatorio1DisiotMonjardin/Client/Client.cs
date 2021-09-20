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
            ViewGame commandHandler = (ViewGame)CommandFactory.GetCommandHandler(Command.VIEW_GAME, networkStreamHandler);
            string gameID = id.ToString();
            GameView gameInfo = commandHandler.SendRequest(gameID);
            Console.WriteLine($"Titulo: {gameInfo.Game.Title}");
            Console.WriteLine($"Sinopsis: {gameInfo.Game.Synopsis}");
            if (gameInfo.Game.ReviewsRating == 0) { Console.WriteLine($"Calificacion: -"); }
            else { Console.WriteLine($"Calificacion: {gameInfo.Game.ReviewsRating}"); }
            Console.WriteLine($"Clasificacion ESRB: {gameInfo.Game.ESRBRating}");
            Console.WriteLine($"Genero: {gameInfo.Game.Genre}");
            Console.WriteLine($"Publicado por: {gameInfo.Game.Publisher}");
            Dictionary<string, Action> opciones = new Dictionary<string, Action>();
            if (!gameInfo.IsOwned) opciones.Add("Comprar Juego", () => ShowBuyGameMenu(id));
            opciones.Add("Ver Reviews", () => MainMenu()); //todooooo
            if (gameInfo.IsOwned) opciones.Add("Escribir Review", () => ShowWriteReviewMenu(id));
            if (gameInfo.IsPublisher) {
                opciones.Add("Modificar Juego", () => MainMenu()); //todo
                opciones.Add("Eliminar Juego", () => MainMenu()); //todo
            }
            opciones.Add("Volver al Menu Principal", () => MainMenu());

            CliMenu.showMenu(opciones, "");
        }
      
        private void ShowBuyGameMenu(int gameID = 1)
        {
            var commandHandler = (BuyGame)CommandFactory.GetCommandHandler(Command.BUY_GAME, networkStreamHandler);
            string message = commandHandler.SendRequest(gameID);
            ShowServerMessage(message);
        }

        private void ShowWriteReviewMenu(int gameID = 1)
        {
            Console.WriteLine("Escriba una puntuación: (del 1 al 5)");
            string Text = Console.ReadLine();
            int rating = int.Parse(Text);
          
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

        private void ShowBrowseReviewsMenu(int pageNumber = 1)
        {
            BrowseCatalogue commandHandler = (BrowseCatalogue)CommandFactory.GetCommandHandler(Command.BROWSE_CATALOGUE, networkStreamHandler);
            GamePage newGamePage = commandHandler.SendRequest(pageNumber);
            ShowCataloguePage(newGamePage);
        }

        /*private void ShowReviewPage(ReviewPage reviewPage)
        {
            Dictionary<string, Action> opciones = new Dictionary<string, Action>();

            for (int i = 0; i < reviewPage.GamesTitles.Count; i++)
            {
                int idIndex = i;
                opciones.Add(reviewPage.GamesTitles[i], () => ShowGameInfo(gamePage.GamesIDs[idIndex]));
            }

            if (gamePage.HasNextPage)
                opciones.Add("Siguiene Página", () => BrowseCatalogue(gamePage.CurrentPage + 1));

            if (gamePage.HasPreviousPage)
                opciones.Add("Página Anterior", () => BrowseCatalogue(gamePage.CurrentPage - 1));

            opciones.Add("Volver al Menu Principal", () => MainMenu());

            CliMenu.showMenu(opciones, $"Catalogo de Juegos Pagina {gamePage.CurrentPage}");
        }*/

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
