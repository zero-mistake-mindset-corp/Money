using System.ComponentModel.DataAnnotations;

namespace Money.BL.Models.MoneyAccount;

public class CreateMoneyAccountModel
{
    [Required(ErrorMessage = "Account name is required")]
    public string Name { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Balance must be > 0")]
    public decimal Balance { get; set; }
}
