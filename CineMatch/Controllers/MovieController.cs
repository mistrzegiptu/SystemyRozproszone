using CineMatch.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace CineMatch.Controllers
{
    [Route("movies")]
    public class MovieController : ControllerBase
    {
        private HttpClient _tmdbClient;
        private HttpClient _omdbClient;

        public MovieController(IHttpClientFactory httpClientFactory)
        {
            _tmdbClient = httpClientFactory.CreateClient("TmdbClient");
            _omdbClient = httpClientFactory.CreateClient("OmdbClient");
        }

        [HttpGet("discover")]
        public async Task<IActionResult> DiscoverMovies([FromQuery] int? year, [FromQuery] string genreId, [FromQuery] int limit = 3)
        {
            int searchYear = year ?? DateTime.Now.Year;

            var queryParams = new Dictionary<string, string?>
            {
                ["with_genres"] = genreId,
                ["primary_release_year"] = searchYear.ToString(),
                ["sort_by"] = "popularity.desc",
                ["language"] = "pl-PL"
            };
            string url = QueryHelpers.AddQueryString("discover/movie", queryParams);

            var tmdbResponse = await _tmdbClient.GetAsync(url);

            if (!tmdbResponse.IsSuccessStatusCode)
                return StatusCode(500, "TMDB API Error");

            var tmdbData = await tmdbResponse.Content.ReadFromJsonAsync<TmdbPagedResponse>();

            if (tmdbData?.Results == null || !tmdbData.Results.Any())
                return NotFound("No movies found");

            var responseMovieList = new List<MovieResponseDto>();
            foreach(var movie in tmdbData.Results.Take(limit))
            {
                var rating = await GetOmdbRating(movie.Title, searchYear);
                responseMovieList.Add(new MovieResponseDto(movie.Title, movie.Id, rating));
            }

            return Ok(responseMovieList);
        }

        private async Task<string> GetOmdbRating(string title, int releaseYear)
        {
            var omdbParams = new Dictionary<string, string?>
            {
                ["t"] = title,
                ["y"] = releaseYear.ToString()
            };

            string omdbUrl = QueryHelpers.AddQueryString("", omdbParams);
            var omdbResponse = await _omdbClient.GetAsync(omdbUrl);

            if (!omdbResponse.IsSuccessStatusCode)
                return string.Empty;

            var omdbData = await omdbResponse.Content.ReadFromJsonAsync<OmdbMovieResponse>();

            if (omdbData?.ReponseStatus != "True" || string.IsNullOrEmpty(omdbData.ImdbRating))
                return string.Empty;

            return omdbData.ImdbRating;
        }
    }
}
