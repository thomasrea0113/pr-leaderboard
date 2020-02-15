using System.ComponentModel.DataAnnotations;

namespace Leaderboard.ViewModels
{
    public class ContactViewModel
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        
        [Required]
        
        public string Subject { get; set; }

        [Required]

        [DataType(DataType.MultilineText)]
        public string Message { get; set; }
    }
}