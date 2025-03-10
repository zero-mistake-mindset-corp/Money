using System.ComponentModel.DataAnnotations;

namespace Money.BL.Models.Auth
{
    public class SignUpModel
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(100, ErrorMessage = "Maximum username length is 100 symbols")]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
