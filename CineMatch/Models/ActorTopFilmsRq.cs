using System.ComponentModel.DataAnnotations;

namespace CineMatch.Models
{
    public class ActorTopFilmsRq
    {
        [Required(ErrorMessage = "Fullname is needed")]
        [MinLength(2, ErrorMessage = "Fullname must be longet than 2 character")]
        [MaxLength(50, ErrorMessage = "Fullname must be longet than 2 character")]
        public string FullName { get; set; } = string.Empty;

        [Range(1, 10, ErrorMessage = "Limit must be within (1, 10) range")]
        public int Limit { get; set; } = 5;
    }
}
