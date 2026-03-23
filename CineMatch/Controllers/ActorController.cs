using CineMatch.Attributes;
using CineMatch.Models;
using CineMatch.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CineMatch.Controllers
{
    [Route("actors")]
    [ApiController]
    [EnableRateLimiting("Fixed")]
    [ApiKey]
    public class ActorController : ControllerBase
    {
        private IMovieApiService _movieService;

        public ActorController(IMovieApiService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet("top-films")]
        public async Task<IActionResult> GetTopFilmsByActor([FromQuery] ActorTopFilmsRq request)
        {
            var queryParams = new Dictionary<string, string?>
            {
                ["query"] = request.FullName
            };

            var actors = await _movieService.SearchActorAsync(queryParams);

            if (actors?.Results == null || !actors.Results.Any())
                return NotFound($"No actor found with given name {request.FullName}");

            var actor = actors.Results.FirstOrDefault();

            var discoverParams = new Dictionary<string, string?>
            {
                ["with_cast"] = actor?.Id.ToString(),
                ["sort_by"] = "vote_average.desc",
                ["vote_count.gte"] = "500",
                ["language"] = "pl-PL"
            };

            var filmsData = await _movieService.GetDiscoverMoviesAsync(discoverParams);

            if (filmsData?.Results == null || !filmsData.Results.Any())
                return NotFound("No movies found");

            var responseMovieList = new List<MovieResponseDto>();
            foreach (var film in filmsData.Results.Take(request.Limit))
            {
                int filmYear = 0;

                if(!string.IsNullOrEmpty(film.ReleaseDate) && film.ReleaseDate.Length >= 4)
                {
                    string year = film.ReleaseDate.Substring(0, 4);
                    int.TryParse(year, out filmYear);
                }

                var ratingParams = new Dictionary<string, string?>
                {
                    ["t"] = film.OriginalTitle,
                    ["y"] = filmYear.ToString()
                };

                var rating = await _movieService.GetOmdbRatingAsync(ratingParams);
                responseMovieList.Add(new MovieResponseDto(film.Title, film.Id, rating, filmYear.ToString()));
            }

            return Ok(responseMovieList);
        }
    }
}
