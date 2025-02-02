namespace Money.BL.Models.UserAccount;

public class UpdatePasswordModel
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
}