using System.Text.Json.Serialization;

namespace CineMatch.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MovieGenre
    {
        Action = 28,
        Comedy = 35,
        Horror = 27,
        SciFi = 878,
        Drama = 18,
        Animation = 16,
        Thriller = 53
    }
}
