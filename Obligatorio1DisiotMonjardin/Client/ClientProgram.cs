using System;

namespace Client
{
    public static class ClientProgram
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Client starting...");
            ClientPresentation client = new ClientPresentation();
            Console.WriteLine("Trying to connect to server");  
            client.StartConnection();
            client.StartMenu();

        }

    }


}
