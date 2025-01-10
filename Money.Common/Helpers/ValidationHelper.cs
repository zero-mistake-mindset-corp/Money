using Money.Common.Exceptions;

namespace Money.Common.Helpers;

public static class ValidationHelper
{
    public static void ValidateSignInData(string passwordHash, string password)
    {
        if (!InformationHasher.VerifyText(password, passwordHash))
        {
            throw new InvalidInputException("Incorrect username or password.");
        }
    }

    public static void ValidateSignUpData(string username, string password, string email)
    {
        BaseValidator.ValidateString(username, maxLength: 50);
        BaseValidator.ValidatePassword(password);
        BaseValidator.ValidateEmail(email);
    }

    public static void EnsureEntityFound<T>(T entity) where T : class
    {
        if (entity == null)
        {
            throw new NotFoundException("Not found.");
        }
    }

    public static void ValidateRefreshToken(DateTime refreshTokenExpiration)
    {
        if (refreshTokenExpiration < DateTime.UtcNow)
        {
            throw new InvalidInputException("Invalid refresh token.");
        }
    }

    public static void VerifyOldAndNewPassword(string oldPassword, string newPassword, string passwordHash)
    {
        if (!InformationHasher.VerifyText(oldPassword, passwordHash))
        {
            throw new InvalidInputException("Incorrect username or password.");
        }

        if (InformationHasher.VerifyText(newPassword, passwordHash))
        {
            throw new InvalidInputException("New password must not match the old password.");
        }
    }

    public static void ValidateNonNegative(decimal balance)
    {
        if (balance < 0)
        {
            throw new InvalidInputException("Money value cannot be negative.");
        }
    }
}
