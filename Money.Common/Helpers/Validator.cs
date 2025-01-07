using Money.Common.Exceptions;
using System.Text.RegularExpressions;

namespace Money.Common.Helpers;

public static class Validator
{
    public static void ValidateNewUserInfo(string email, string password, string username)
    {
        Validate(() => IsValidPassword(password), new InvalidInputException("Invalid password"));
        Validate(() => IsValidString(username), new InvalidInputException("Invalid username"));
    }

    public static void ValidateString(string name)
    {
        Validate(() => IsValidString(name), new InvalidInputException("Invalid string"));
    }

    public static void ValidatePassword(string password)
    {
        Validate(() => IsValidPassword(password), new InvalidInputException("Invalid password"));
    }

    public static void ValidatePagination(int pageIndex, int pageSize)
    {
        Validate(() => IsValidPaginationData(pageIndex, pageSize), new InvalidInputException("Invalid pagination values"));
    }

    private static void Validate(Func<bool> validationRule, Exception exception)
    {
        if (!validationRule())
        {
            throw exception;
        }
    }

    private static bool IsValidPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        string pattern = @"^(?=.*[A-Z])(?=.*\d)[A-Za-z\d]{8,}$";
        Regex regex = new Regex(pattern);

        var isMatch = regex.IsMatch(password);
        return isMatch;
    }

    private static bool IsValidString(string name)
    {
        var isValid = !string.IsNullOrWhiteSpace(name) && name.Length <= 50;
        return isValid;
    }

    private static bool IsValidPaginationData(int pageIndex, int pageSize)
    {
        if (pageIndex < 1 || pageSize < 1 || pageSize > 100)
        {
            return false;
        }

        return true;
    }
}
