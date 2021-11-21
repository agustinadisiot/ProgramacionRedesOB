using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;

namespace AdminServer.Controllers
{
    [Route("admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private Greeter.GreeterClient client;
        public AdminController()
        {
            AppContext.SetSwitch(
                    "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        }

        [HttpPost("games")]
        public async Task<ActionResult> PostGame([FromBody] GameDTO game)
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5007");
            client = new Greeter.GreeterClient(channel);
            var reply = await client.PostGameAsync(game);
            return Ok(reply.Message);
        }

        [HttpPut("games")]
        public async Task<ActionResult> UpdateGame([FromBody] GameDTO game)
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5007");
            client = new Greeter.GreeterClient(channel);
            var reply = await client.UpdateGameAsync(game);
            return Ok(reply.Message);
        }

        [HttpDelete("games/{id}")]
        public async Task<ActionResult> DeleteGame([FromRoute] int id)
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5007");
            client = new Greeter.GreeterClient(channel);
            var reply = await client.DeleteGameAsync(new Id { Id_ = id });
            return Ok(reply.Message);
        }


        [HttpPost("users")]
        public async Task<ActionResult> PostUser([FromBody] UserDTO user)
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5007");
            client = new Greeter.GreeterClient(channel);
            var reply = await client.PostUserAsync(user);
            return Ok(reply.Message);
        }

        [HttpPut("users")]
        public async Task<ActionResult> UpdateUser([FromBody] UserDTO user)
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5007");
            client = new Greeter.GreeterClient(channel);
            var reply = await client.UpdateUserAsync(user);
            return Ok(reply.Message);
        }

        [HttpDelete("users/{id}")]
        public async Task<ActionResult> DeleteUser([FromRoute] int id)
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5007");
            client = new Greeter.GreeterClient(channel);
            var reply = await client.DeleteUserAsync(new Id { Id_ = id });
            return Ok(reply.Message);
        }

        [HttpPost("games/{gameId}/users{userId}")]
        public async Task<ActionResult> AssociateGameWithUser([FromRoute] int gameId, [FromRoute] int userId)
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5007");
            client = new Greeter.GreeterClient(channel);
            var reply = await client.AssociateGameWithUserAsync(new Purchase { IdUser = userId, IdGame = gameId });
            return Ok(reply.Message);
        }
    }
}
