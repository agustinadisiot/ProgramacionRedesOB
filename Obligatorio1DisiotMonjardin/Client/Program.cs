﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Common;
using Common.Protocol;

namespace Client
{
    public static class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, Action> opciones = new Dictionary<string, Action>();
            opciones.Add("ver", () => Console.WriteLine("opcion ver XD"));
            opciones.Add("comprar", () => Console.WriteLine("no compre este juego"));
            opciones.Add("eliminar", () => Console.WriteLine("seguro que lo quiere borrar"));
            opciones.Add("reimprimir", () => CliMenu.showMenu(opciones, "menucito"));
            opciones.Add("explota", () => Main(args));
            opciones.Add("TestMandarServer", () => TestMandarAlServer());

            CliMenu.showMenu(opciones, "Menuuuu");
        }


        private static void TestMandarAlServer()
        {
            Console.WriteLine("Client starting...");
            var clientIpEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0); // Puerto 0 -> usa el primer puerto disponible
            var tcpClient = new TcpClient(clientIpEndPoint);
            // if this is the same pc or network adapter as the server, we must use a different port.
            var serverIpEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6000);
            // This has to be the same IpEndPoint as the server.
            Console.WriteLine("Trying to connect to server");
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
        // TODO ELIMINAR
        // JUEGO1$IDJUEGO1#JUEGO2$IDJUE/GO2#JUEGO3$IDJUEGO3#_##1#0

        // REQ/02/303/JUEGO1ID
    }
}
