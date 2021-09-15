using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Client.Commands;
using Common;
using Common.NetworkUtils;
using Common.Protocol;
using Server.SteamHelpers;

namespace Client
{
    public static class ClientProgram
    {
        private static NetworkStream networkStream;
        static void Main(string[] args)
        {
            Console.WriteLine("Client starting...");
            var clientIpEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0); // Puerto 0 -> usa el primer puerto disponible
            var tcpClient = new TcpClient(clientIpEndPoint);
            // if this is the same pc or network adapter as the server, we must use a different port.
            var serverIpEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6000);
            // This has to be the same IpEndPoint as the server.
            Console.WriteLine("Trying to connect to server"); // TODO catcher excepciones de cuando el server se apaga 
            tcpClient.Connect(serverIpEndPoint);
            networkStream = tcpClient.GetStream();

            MainMenu();


        }

        private static void MainMenu()
        {
            Dictionary<string, Action> opciones = new Dictionary<string, Action>();
            opciones.Add("Ver catalogo", () => BrowseCatalogue());
            opciones.Add("Publicar Juego", () => Publish());
            opciones.Add("Salir", () => Console.WriteLine("seguro que quiere salir????!!"));
            opciones.Add("reimprimir", () => CliMenu.showMenu(opciones, "menucito"));
            opciones.Add("TestMandarServer", () => TestMandarAlServer());
            while (true)
            {
                CliMenu.showMenu(opciones, "Menuuuu");
            }
        }



        private static void BrowseCatalogue()
        {
            NetworkStreamHandler networkStreamHandler = new NetworkStreamHandler(networkStream);
            BrowseCatalogue commandHandler = (BrowseCatalogue) CommandFactory.GetCommandHandler(Command.BROWSE_CATALOGUE, networkStreamHandler);
            commandHandler.SendRequest(1); //magic number
        }

        internal static void ShowCataloguePage(GamePage gamePage)
        {
            Console.WriteLine(gamePage.GamesTitles[0]);
        }

        private static void Publish()
        {
            byte[] header = Encoding.UTF8.GetBytes("REQ");
            ushort command = (ushort)Command.PUBLISH_GAME;
            byte[] cmd = BitConverter.GetBytes(command);

            Console.WriteLine("Escriba el Titulo del juego:");
            var word = Console.ReadLine();
            byte[] data = Encoding.UTF8.GetBytes(word);
            byte[] dataLength = BitConverter.GetBytes(data.Length);

            networkStream.Write(header, 0, Specification.HeaderLength); // Header
            networkStream.Write(cmd, 0, Specification.CmdLength); // CMD
            networkStream.Write(dataLength, 0, Specification.dataSizeLength); // Largo
            networkStream.Write(data, 0, data.Length); // Datos

        }

        private static void TestMandarAlServer()
        {
            Console.WriteLine("Client starting...");
            var clientIpEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0); // Puerto 0 -> usa el primer puerto disponible
            var tcpClient = new TcpClient(clientIpEndPoint);
            // if this is the same pc or network adapter as the server, we must use a different port.
            var serverIpEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6000);
            // This has to be the same IpEndPoint as the server.
            Console.WriteLine("Trying to connect to server"); // TODO catcher excepciones de cuando el server se apaga 
            tcpClient.Connect(serverIpEndPoint);
            var keepConnection = true;

            using (var networkStream = tcpClient.GetStream())
            {
                while (keepConnection)
                {
                    byte[] header = Encoding.UTF8.GetBytes("REQ");
                    ushort command = 5;
                    byte[] cmd = BitConverter.GetBytes(command);

                    var word = Console.ReadLine();
                    byte[] data = Encoding.UTF8.GetBytes(word);
                    byte[] dataLength = BitConverter.GetBytes(data.Length);

                    networkStream.Write(header, 0, Specification.HeaderLength); // Header
                    networkStream.Write(cmd, 0, Specification.CmdLength); // CMD
                    networkStream.Write(dataLength, 0, Specification.dataSizeLength); // Largo
                    networkStream.Write(data, 0, data.Length); // Datos


                    //networkStream.Write(dataLength, 0, Protocol.WordLength); // Enivamos XXXX
                    //networkStream.Write(data, 0, data.Length); // Enviamos DATA
                    if (word.Equals("exit"))
                    {
                        keepConnection = false;
                    }
                }
            }
            tcpClient.Close();
        }
    }


}
