using System.Text.Json.Serialization;

namespace CineMatch.Models
{
    public record TmdbActorSearchRs(
        [property: JsonPropertyName("results")] List<TmdbActor> Results
    );
}
