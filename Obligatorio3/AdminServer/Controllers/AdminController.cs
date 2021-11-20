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

        /*        [HttpGet("games/{id}")]
                public async Task<ActionResult<GameDTO>> GetGame([FromRoute] int id)
                {
                    return await Ok(businessLogic.GetGame(id));
                }

                [HttpGet("games")]
                public async Task<ActionResult<IEnumerable<GameDTO>>> GetGames()
                {
                    return await Ok(businessLogic.GetGames());
                }*/



        /*        [HttpDelete("games/{id}")]
                public async Task<ActionResult> DeleteGame([FromRoute] int id)
                {
                    return await Ok(businessLogic.DeleteGame(id));
                }


                [HttpPost("users")]
                public async Task<ActionResult> PostUser()
                {
                    return await Ok(businessLogic.PostUser());
                }

                [HttpGet("users/{id}")]
                public async Task<ActionResult<UserDTO>> GetUser([FromRoute] int id)
                {
                    return await Ok(businessLogic.GetUser(id));
                }

                [HttpGet("users")]
                public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
                {
                    return await Ok(businessLogic.GetUsers());
                }

                [HttpPut("users/{id}")]
                public async Task<ActionResult> UpdateUser([FromRoute] int id)
                {
                    //return await Ok(businessLogic.UpdateUser(id));
                }

                [HttpDelete("users/{id}")]
                public async Task<ActionResult> DeleteUser([FromRoute] int id)
                {
                    //return await Ok(businessLogic.DeleteUser(id));
                }

                [HttpPost("games/{gameId}/users{userId}")]
                public async Task<ActionResult> AssociateGameWithUser([FromRoute] int gameId, [FromRoute] int userId)
                {
                   // return await Ok(businessLogic.AssociateGameWithUser(gameId, userId));
                }*/
    }
}
