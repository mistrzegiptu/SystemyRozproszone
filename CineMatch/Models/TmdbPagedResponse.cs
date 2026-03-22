using System.Text.Json.Serialization;

namespace CineMatch.Models
{
    public record TmdbPagedResponse(
        [property: JsonPropertyName("results")] List<TmdbMovie> Results
    );
}
