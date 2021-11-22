using System;
using System.Threading.Tasks;
using Common;
using Common.Interfaces;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;

namespace AdminServer.Controllers
{
    [Route("admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private Admin.AdminClient client;
        private string grpcURL;

        static readonly ISettingsManager SettingsMgr = new SettingsManager();
        public AdminController()
        {
            AppContext.SetSwitch(
                    "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var serverIpAddress = SettingsMgr.ReadSetting(ServerConfig.GrpcURL);
        }

        [HttpPost("games")]
        public async Task<ActionResult> PostGame([FromBody] GameDTO game)
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5007");
            client = new Admin.AdminClient(channel);
            var reply = await client.PostGameAsync(game);
            return Ok(reply.Message);
        }

        [HttpGet("games")]
        public async Task<ActionResult> GetGames()
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5007");
            client = new Admin.AdminClient(channel);
            var reply = await client.GetGamesAsync(new GamesRequest { });
            return Ok(reply);
        }

        [HttpGet("games/{id}")]
        public async Task<ActionResult> GetGameById([FromRoute] int id)
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5007");
            client = new Admin.AdminClient(channel);
            var reply = await client.GetGameByIdAsync(new Id { Id_ = id });
            return Ok(reply);
        }

        [HttpPut("games")]
        public async Task<ActionResult> UpdateGame([FromBody] GameDTO game)
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5007");
            client = new Admin.AdminClient(channel);
            var reply = await client.UpdateGameAsync(game);
            return Ok(reply.Message);
        }

        [HttpDelete("games/{id}")]
        public async Task<ActionResult> DeleteGame([FromRoute] int id)
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5007");
            client = new Admin.AdminClient(channel);
            var reply = await client.DeleteGameAsync(new Id { Id_ = id });
            return Ok(reply.Message);
        }


        [HttpPost("users")]
        public async Task<ActionResult> PostUser([FromBody] UserDTO user)
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5007");
            client = new Admin.AdminClient(channel);
            var reply = await client.PostUserAsync(user);
            return Ok(reply.Message);
        }

        [HttpGet("users")]
        public async Task<ActionResult> GetUsers()
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5007");
            client = new Admin.AdminClient(channel);
            var reply = await client.GetUsersAsync(new UsersRequest { });
            return Ok(reply);
        }

        [HttpGet("users/{id}")]
        public async Task<ActionResult> GetUserById([FromRoute] int id)
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5007");
            client = new Admin.AdminClient(channel);
            var reply = await client.GetUserByIdAsync(new Id { Id_ = id });
            return Ok(reply);
        }

        [HttpPut("users")]
        public async Task<ActionResult> UpdateUser([FromBody] UserDTO user)
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5007");
            client = new Admin.AdminClient(channel);
            var reply = await client.UpdateUserAsync(user);
            return Ok(reply.Message);
        }

        [HttpDelete("users/{id}")]
        public async Task<ActionResult> DeleteUser([FromRoute] int id)
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5007");
            client = new Admin.AdminClient(channel);
            var reply = await client.DeleteUserAsync(new Id { Id_ = id });
            return Ok(reply.Message);
        }

        [HttpPost("games/{gameId}/users/{userId}")]
        public async Task<ActionResult> AssociateGameWithUser([FromRoute] int gameId, [FromRoute] int userId)
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5007");
            client = new Admin.AdminClient(channel);
            var reply = await client.AssociateGameWithUserAsync(new Purchase { IdUser = userId, IdGame = gameId });
            return Ok(reply.Message);
        }

        [HttpDelete("games/{gameId}/users/{userId}")]
        public async Task<ActionResult> DisassociateGameWithUser([FromRoute] int gameId, [FromRoute] int userId)
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5007");
            client = new Admin.AdminClient(channel);
            var reply = await client.DisassociateGameWithUserAsync(new Purchase { IdUser = userId, IdGame = gameId });
            return Ok(reply.Message);
        }
    }
}
