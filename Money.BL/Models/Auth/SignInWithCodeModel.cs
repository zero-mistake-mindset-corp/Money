namespace Money.BL.Models.Auth;

public class SignInWithCodeModel
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string Code { get; set; }
}
