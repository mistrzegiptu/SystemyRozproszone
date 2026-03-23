using CineMatch.Models;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.WebUtilities;

namespace CineMatch.Services
{
    public class MovieApiService : IMovieApiService
    {
        private readonly HttpClient _tmdbClient;
        private readonly HttpClient _omdbClient;

        public MovieApiService(IHttpClientFactory httpClientFactory)
        {
            _tmdbClient = httpClientFactory.CreateClient("TmdbClient");
            _omdbClient = httpClientFactory.CreateClient("OmdbClient");
        }

        public async Task<TmdbPagedResponse?> GetDiscoverMoviesAsync(Dictionary<string, string?> queryParams)
        {
            string url = QueryHelpers.AddQueryString("discover/movie", queryParams);

            var response = await _tmdbClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;

            var data = await response.Content.ReadFromJsonAsync<TmdbPagedResponse>();

            if (data?.Results == null || !data.Results.Any())
                return null;

            return data;
        }

        public async Task<string> GetOmdbRatingAsync(Dictionary<string, string?> queryParams)
        {
            string url = QueryHelpers.AddQueryString("", queryParams);
            var response = await _omdbClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return string.Empty;

            var data = await response.Content.ReadFromJsonAsync<OmdbMovieResponse>();

            if (data?.ResponseStatus != "True" || string.IsNullOrEmpty(data.ImdbRating))
                return string.Empty;

            return data.ImdbRating;
        }

        public async Task<TmdbActorSearchRs?> SearchActorAsync(Dictionary<string, string?> queryParams)
        {
            string url = QueryHelpers.AddQueryString("search/person", queryParams);
            var response = await _tmdbClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<TmdbActorSearchRs>();
        }
    }
}
