using Money.Common.Exceptions;
using System.Text.RegularExpressions;

namespace Money.Common.Helpers;

public static class BaseValidator
{
    public static void ValidateString(string input, int maxLength = 50)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new InvalidInputException("String is null or whitespace.");
        }

        if (input.Length > maxLength)
        {
            throw new InvalidInputException($"String length exceeds {maxLength}.");
        }
    }
    
    public static void ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidInputException("Password is null or whitespace.");
        }

        string pattern = @"^(?=.*[A-Z])(?=.*\d)[A-Za-z\d]{8,}$";
        Regex regex = new Regex(pattern);

        if (!regex.IsMatch(password))
        {
            throw new InvalidInputException("Password does not meet complexity requirements.");
        }
    }

    public static void ValidatePagination(int pageIndex, int pageSize)
    {
        if (pageIndex < 1 || pageSize < 1 || pageSize > 100)
        {
            throw new InvalidInputException("Invalid pagination values.");
        }
    }

    public static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new InvalidInputException("Email is null or whitespace.");
        }

        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        Regex regex = new Regex(pattern);

        if (!regex.IsMatch(email))
        {
            throw new InvalidInputException("Invalid email format.");
        }
    }
}