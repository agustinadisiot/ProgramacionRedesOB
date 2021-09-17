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
            opciones.Add("Salir", () => Console.WriteLine("seguro que quiere salir????!!"));
            opciones.Add("reimprimir", () => CliMenu.showMenu(opciones, "menucito"));
            while (true)
            {
                CliMenu.showMenu(opciones, "Menuuuu");
            }
        }



        private  void BrowseCatalogue(int pageNumber = 1)
        {
            BrowseCatalogue commandHandler = (BrowseCatalogue)CommandFactory.GetCommandHandler(Command.BROWSE_CATALOGUE, networkStreamHandler);
            GamePage newGamePage = commandHandler.SendRequest(pageNumber);
            ShowCataloguePage(newGamePage);
        }

        internal  void ShowCataloguePage(GamePage gamePage)
        {
            Dictionary<string, Action> opciones = new Dictionary<string, Action>();

            foreach (string gameTitle in gamePage.GamesTitles)
            {
                opciones.Add(gameTitle, () => MainMenu());
            }

            if (gamePage.HasNextPage)
                opciones.Add("Siguiene Página", () => BrowseCatalogue(gamePage.CurrentPage + 1));

            if (gamePage.HasPreviousPage)
                opciones.Add("Página Anterior", () => BrowseCatalogue(gamePage.CurrentPage - 1));

            opciones.Add("Volver al Menu Principal", () => MainMenu());

            CliMenu.showMenu(opciones, $"Catalogo de Juegos Pagina {gamePage.CurrentPage}");
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
