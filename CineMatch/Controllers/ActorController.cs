using Microsoft.AspNetCore.Mvc;

namespace CineMatch.Controllers
{
    [Route("actors")]
    public class ActorController : ControllerBase
    {
        private HttpClient _tmdbClient;

        public ActorController(HttpClient tmdbClient)
        {
            _tmdbClient = tmdbClient;
        }

        public async Task<IActionResult> GetTopFilmsByActor([FromQuery] string firstName, [FromQuery] string lastName)
        {
            return Ok();
        }
    }
}
