using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdminServer.Controllers
{
    [Route("admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {

        public AdminController() { }

        [HttpPost("games")]
        public async Task<ActionResult> PostGame()
        {
            AdminBusinessLogic businessLogic = new AdminBusinessLogic();
            string result = await businessLogic.PostGame();
            return Ok(result);
        }
/*

        [HttpGet("games/{id}")]
        public async Task<ActionResult<Game>> GetGame([FromRoute] int id)
        {
            return await Ok(businessLogic.GetGame(id));
        }

        [HttpGet("games")]
        public async Task<ActionResult<IEnumerable<Game>>> GetGames()
        {
            return await Ok(businessLogic.GetGames());
        }

        [HttpPut("games/{id}")]
        public async Task<ActionResult> UpdateGame([FromRoute] int id)
        {
            return await Ok(businessLogic.UpdateGame(id));
        }

        [HttpDelete("games/{id}")]
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
        public async Task<ActionResult<User>> GetUser([FromRoute] int id)
        {
            return await Ok(businessLogic.GetUser(id));
        }

        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await Ok(businessLogic.GetUsers());
        }

        [HttpPut("users/{id}")]
        public async Task<ActionResult> UpdateUser([FromRoute] int id)
        {
            return await Ok(businessLogic.UpdateUser(id));
        }

        [HttpDelete("users/{id}")]
        public async Task<ActionResult> DeleteUser([FromRoute] int id)
        {
            return await Ok(businessLogic.DeleteUser(id));
        }

        [HttpPost("games/{gameId}/users{userId}")]
        public async Task<ActionResult> AssociateGameWithUser([FromRoute] int gameId, [FromRoute] int userId)
        {
            return await Ok(businessLogic.AssociateGameWithUser(gameId, userId));
        }*/
    }
}
