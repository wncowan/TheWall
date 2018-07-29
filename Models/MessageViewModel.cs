
using System.ComponentModel.DataAnnotations;

namespace TheWall.Models
{
    public class MessageViewModel : BaseEntity
    {

        [Required(ErrorMessage = "You must enter something")]
        [MinLength(2, ErrorMessage = "Message must be at least 3 characters")]
        [MaxLength(25, ErrorMessage = "Message cannot be more than 25 characters")]
        public string Content { get; set; }

    }
}