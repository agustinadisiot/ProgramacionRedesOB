using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;

namespace AdminServer.Controllers
{
    [Route("admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AdminBusinessLogic businessLogic;

        public AdminController(AdminBusinessLogic newBusinessLogic)
        {
            businessLogic = newBusinessLogic;
        }

        [HttpPost("games")]
        public async Task<ActionResult> PostGame()
        {
            //Ok result?
            return await businessLogic.PostGame();
        }

        [HttpGet("games/{id}")]
        public async Task<ActionResult<Game>> GetGame([FromRoute] int id)
        {
            //Ok result?
            return await businessLogic.GetGame(id);
        }

        [HttpGet("games")]
        public async Task<ActionResult<IEnumerable<Game>>> GetGames()
        {
            return await businessLogic.GetGames();
        }

        [HttpPut("games/{id}")]
        public async Task<ActionResult> UpdateGame([FromRoute] int id)
        {
            //Ok result?
            return await businessLogic.UpdateGame(id);
        }

        [HttpDelete("games/{id}")]
        public async Task<ActionResult> DeleteGame([FromRoute] int id)
        {
            //Ok result?
            return await businessLogic.DeleteGame(id);
        }


        [HttpPost("users")]
        public async Task<ActionResult> PostUser()
        {
            //Ok result?
            return await businessLogic.PostUser();
        }

        [HttpGet("users/{id}")]
        public async Task<ActionResult<User>> GetUser([FromRoute] int id)
        {
            //Ok result?
            return await businessLogic.GetUser(id);
        }

        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await businessLogic.GetUsers();
        }

        [HttpPut("users/{id}")]
        public async Task<ActionResult> UpdateUser([FromRoute] int id)
        {
            //Ok result?
            return await businessLogic.UpdateUser(id);
        }

        [HttpDelete("users/{id}")]
        public async Task<ActionResult> DeleteUser([FromRoute] int id)
        {
            //Ok result?
            return await businessLogic.DeleteUser(id);
        }

        [HttpPost("games/{gameId}/users{userId}")]
        public async Task<ActionResult> AssociateGameWithUser([FromRoute] int gameId, [FromRoute] int userId)
        {
            //Ok result?
            return await businessLogic.AssociateGameWithUser(gameId, userId);
        }
    }
}
