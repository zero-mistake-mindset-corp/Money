using System.ComponentModel.DataAnnotations;

namespace Money.BL.Models.MoneyAccount;

public class UpdateMoneyAccountModel
{
    [Required(ErrorMessage = "Id is required")]
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [MaxLength(100, ErrorMessage = "Max length of account name is 100 symbols")]
    public string Name { get; set; }
}
