using Money.Data.Entities;

namespace Money.BL.Models.Account;

public class CreateMoneyAccountModel
{
    public string Name { get; set; }
    public decimal Balance { get; set; }
}
