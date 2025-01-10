namespace Money.BL.Models.Auth;

public class SignInWithCodeModel
{
    public string EmailOrUsername { get; set; }
    public string Password { get; set; }
    public string Code { get; set; }
}
