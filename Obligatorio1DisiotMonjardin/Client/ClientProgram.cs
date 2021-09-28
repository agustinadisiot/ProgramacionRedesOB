using System;

namespace Client
{
    public static class ClientProgram
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Client starting...");
            ClientPresentation client = new ClientPresentation();
            Console.WriteLine("Trying to connect to server"); // TODO catcher excepciones de cuando el server se apaga 
            client.StartConnection();
            client.StartMenu();

        }

    }


}
