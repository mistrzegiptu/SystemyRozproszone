using System.Text.Json.Serialization;

namespace CineMatch.Models
{
    public record TmdbActor(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("name")] string Name
    );
}
