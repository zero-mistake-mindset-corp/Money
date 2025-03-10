using System.ComponentModel.DataAnnotations;

namespace Money.BL.Models.UserAccount;

public class UpdateAccountModel
{
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; }
}
