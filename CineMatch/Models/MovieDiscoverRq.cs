using System.ComponentModel.DataAnnotations;

namespace CineMatch.Models
{
    public class MovieDiscoverRq
    {
        [Range(1900, 2026)]
        public int Year { get; set; }

        public MovieGenre Genre { get; set; }

        [Range(1, 10, ErrorMessage = "Limit must be withing (1, 10) range")]
        public int Limit { get; set; } = 3;
    }
}
