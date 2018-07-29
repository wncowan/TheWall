
using System.ComponentModel.DataAnnotations;

namespace TheWall.Models
{
    public class LoginViewModel : BaseEntity
    {

        [Required(ErrorMessage = "You must enter an email")]
        [EmailAddress]
        [MinLength(3, ErrorMessage = "Email must be at least 3 characters")]
        [MaxLength(20, ErrorMessage = "Email cannot be more than 20 characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "You must enter a password")]
        [MinLength(8, ErrorMessage = "Password cannot be less than 8 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}