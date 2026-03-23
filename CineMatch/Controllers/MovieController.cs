using CineMatch.Attributes;
using CineMatch.Models;
using CineMatch.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CineMatch.Controllers
{
    [Route("movies")]
    [ApiController]
    [EnableRateLimiting("Fixed")]
    [ApiKey]
    public class MovieController : ControllerBase
    {
        private IMovieApiService _movieService;

        public MovieController(IMovieApiService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet("discover")]
        public async Task<IActionResult> DiscoverMovies([FromQuery] MovieDiscoverRq request)
        {
            var queryParams = new Dictionary<string, string?>
            {
                ["with_genres"] = ((int)request.Genre).ToString(),
                ["primary_release_year"] = request.Year.ToString(),
                ["sort_by"] = "vote_average.desc",
                ["vote_count.gte"] = "500",
                ["language"] = "pl-PL"
            };

            var tmdbData = await _movieService.GetDiscoverMoviesAsync(queryParams);

            if (tmdbData?.Results == null || !tmdbData.Results.Any())
                return NotFound("No movies found");

            var responseMovieList = new List<MovieResponseDto>();
            foreach(var movie in tmdbData.Results.Take(request.Limit))
            {
                var omdbRatingParams = new Dictionary<string, string?>
                {
                    ["t"] = movie.OriginalTitle,
                    ["y"] = request.Year.ToString()
                };

                var rating = await _movieService.GetOmdbRatingAsync(omdbRatingParams);
                responseMovieList.Add(new MovieResponseDto(movie.Title, movie.Id, rating, request.Year.ToString()));
            }

            return Ok(responseMovieList);
        }

        [HttpGet("random")]
        public async Task<IActionResult> GetRandomMovie()
        {
            var randomPage = Random.Shared.Next(1, 501);

            var queryParams = new Dictionary<string, string?>
            {
                ["page"] = randomPage.ToString(),
                ["sort_by"] = "vote_average.desc",
                ["vote_count.gte"] = "500",
                ["language"] = "pl-PL"
            };

            var tmdbData = await _movieService.GetDiscoverMoviesAsync(queryParams);

            if (tmdbData?.Results == null || !tmdbData.Results.Any())
                return NotFound("No movies found");

            var randomMovieIndex = Random.Shared.Next(0, tmdbData.Results.Count);
            var randomMovie = tmdbData.Results[randomMovieIndex];

            int movieYear = 0;
            if (!string.IsNullOrEmpty(randomMovie.ReleaseDate) && randomMovie.ReleaseDate.Length >= 4)
            {
                var year = randomMovie.ReleaseDate.Substring(0, 4);
                int.TryParse(year, out movieYear);
            }

            var omdbRatingParams = new Dictionary<string, string?>
            {
                ["t"] = randomMovie.OriginalTitle,
                ["y"] = movieYear.ToString()
            };

            var rating = await _movieService.GetOmdbRatingAsync(omdbRatingParams);

            var responseMovie = new MovieResponseDto(randomMovie.Title, randomMovie.Id, rating.ToString(), movieYear.ToString());

            return Ok(responseMovie);
        }
    }
}
