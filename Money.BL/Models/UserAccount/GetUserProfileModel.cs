namespace Money.BL.Models.UserAccount;

public class GetUserProfileModel
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public bool Is2FAEnabled { get; set; }
}
