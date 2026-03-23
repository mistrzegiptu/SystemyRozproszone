using System.Text.Json.Serialization;

namespace CineMatch.Models
{
    public record TmdbMovie(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("title")] string Title,
        [property: JsonPropertyName("original_title")] string OriginalTitle,
        [property: JsonPropertyName("release_date")] string ReleaseDate
    );
}
