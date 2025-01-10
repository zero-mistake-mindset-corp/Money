using Money.BL.Models.Auth;

namespace Money.BL.Interfaces;

public interface IUserService
{
    Task SignUpAsync(SignUpModel signUpModel);
    Task ConfirmEmailAsync(string email, string code);
    Task ResendEmailConfirmationCodeAsync(string email);
}
