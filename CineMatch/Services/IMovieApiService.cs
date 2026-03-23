using CineMatch.Models;

namespace CineMatch.Services
{
    public interface IMovieApiService
    {
        Task<TmdbPagedResponse?> GetDiscoverMoviesAsync(Dictionary<string, string?> queryParams);

        Task<string> GetOmdbRatingAsync(Dictionary<string, string?> queryParams);

        Task<TmdbActorSearchRs?> SearchActorAsync(Dictionary<string, string?> queryParams);
    }
}
