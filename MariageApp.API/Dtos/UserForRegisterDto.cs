using System.ComponentModel.DataAnnotations;

namespace MariageApp.API.Dtos
{
    public class UserForRegisterDto
    {[Required]
   
        public string  Username { get; set; }
        [StringLength(8,MinimumLength =4,ErrorMessage ="Au min 4 lettres et au max 8 lettres")]
        [Required]
        public string  Password { get; set; }
    }
}