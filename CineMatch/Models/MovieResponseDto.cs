namespace CineMatch.Models
{
    public record MovieResponseDto(
        string Title,
        int TmdbId,
        string ImdbRating,
        string Year
    );
}
