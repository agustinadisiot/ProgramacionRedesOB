using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Client.Commands;
using Common;
using Common.Domain;
using Common.NetworkUtils;
using Common.Protocol;

namespace Client
{
    public static class ClientProgram
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Client starting...");
            Client client = new Client();
            Console.WriteLine("Trying to connect to server"); // TODO catcher excepciones de cuando el server se apaga 
            client.StartConnection();
            client.MainMenu();
            
        }
        
    }


}
