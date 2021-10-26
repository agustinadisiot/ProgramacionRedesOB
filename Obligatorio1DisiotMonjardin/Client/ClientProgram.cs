using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Client
{
    public static class ClientProgram
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Client starting...");
            ClientPresentation client = new ClientPresentation();
            Console.WriteLine("Trying to connect to server");
            await client.StartConnection();
            await client.StartMenu();
        }

    }


}
