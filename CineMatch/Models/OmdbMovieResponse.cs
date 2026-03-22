using System.Text.Json.Serialization;

namespace CineMatch.Models
{
    public record OmdbMovieResponse(
        [property: JsonPropertyName("Response")] string ReponseStatus,
        [property: JsonPropertyName("imdbRating")] string ImdbRating,
        [property: JsonPropertyName("BoxOffice")] string BoxOffice
    );
}
