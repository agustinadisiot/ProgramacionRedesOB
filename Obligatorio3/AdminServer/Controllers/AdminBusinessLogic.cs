using Grpc.Net.Client;
using GrpcServer;
using System;
using System.Threading.Tasks;

namespace AdminServer.Controllers
{
    public class AdminBusinessLogic
    {
        public async Task<string> PostGame()
        {
            AppContext.SetSwitch(
                "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            // The port number(5001) must match the port of the gRPC server.
            using var channel = GrpcChannel.ForAddress("http://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var reply = await client.SayHelloAsync(
                new HelloRequest { Name = "GreeterClient" });
            Console.WriteLine("Greeting: " + reply.Message);
            return reply.Message;
        }
    }
}