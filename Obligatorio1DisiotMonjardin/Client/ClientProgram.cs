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
            client.StartConnection();
            try
            {
                Task.Run(()=>client.StartMenu()).Wait();
            }
            catch(SocketException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
            Console.WriteLine("se termino"); //TODO
            

        }

    }


}
