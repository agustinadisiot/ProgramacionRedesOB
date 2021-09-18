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
        private  NetworkStreamHandler networkStreamHandler;
        private readonly TcpClient tcpClient;
        private readonly IPEndPoint serverIpEndPoint;

        public Client() { 
            var clientIpEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0); // Puerto 0 -> usa el primer puerto disponible
            tcpClient = new TcpClient(clientIpEndPoint);
            serverIpEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6000); // TODO usar config files
        
        }
        public void StartConnection()
        {
            tcpClient.Connect(serverIpEndPoint);
            networkStreamHandler = new NetworkStreamHandler(tcpClient.GetStream());
        }

        
        public void MainMenu()
        {
            Dictionary<string, Action> opciones = new Dictionary<string, Action>();
            opciones.Add("Ver catalogo", () => BrowseCatalogue());
            opciones.Add("Publicar Juego", () => Publish());
            opciones.Add("Buscar por titulo", () => SearchByTitle());
            opciones.Add("Iniciar Sesión", () => Login());
            opciones.Add("Salir", () => Console.WriteLine("seguro que quiere salir????!!"));
            opciones.Add("reimprimir", () => CliMenu.showMenu(opciones, "menucito"));
            while (true)
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

        private  void BrowseCatalogue(int pageNumber = 1)
        {
            BrowseCatalogue commandHandler = (BrowseCatalogue)CommandFactory.GetCommandHandler(Command.BROWSE_CATALOGUE, networkStreamHandler);
            GamePage newGamePage = commandHandler.SendRequest(pageNumber);
            ShowCataloguePage(newGamePage);
        }

        private void SearchByTitle() {
            Console.WriteLine("Escriba el titulo del juego: ");
            string title = Console.ReadLine();
            // TODO pedir titulo devuelta si es vacio
            ShowSearchByTitlePage(title);
        }
        public void ShowSearchByTitlePage(string title, int pageNumber = 1) {
            var commandHandler = (SearchByTitle)CommandFactory.GetCommandHandler(Command.SEARCH_BY_TITLE, networkStreamHandler);
            GamePage newGamePage = commandHandler.SendRequest(pageNumber, title);
            ShowSearchByTitlePage(newGamePage, title);
        }

        private void ShowSearchByTitlePage(GamePage gamePage,string title)
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

        private  void ShowCataloguePage(GamePage gamePage)
        {
            Dictionary<string, Action> opciones = new Dictionary<string, Action>();

            for(int i = 0; i < gamePage.GamesTitles.Count; i++)
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

        private  void Publish()
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

        public void ShowServerMessage(string message)
        {
            Console.WriteLine(message);
            MainMenu();
        }

    }
}
