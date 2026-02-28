using System.ComponentModel.DataAnnotations;

namespace Speculo.Identity.Validation;

public static class AuthValidation
{
    public static List<string> ValidateRegister(string? email, string? password, string? fullName)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(email))
            errors.Add("Email is required.");
        else if (!new EmailAddressAttribute().IsValid(email))
            errors.Add("Invalid email format.");

        if (string.IsNullOrWhiteSpace(password))
            errors.Add("Password is required.");
        else if (password.Length < 8)
            errors.Add("Password must be at least 8 characters long.");

        if (string.IsNullOrWhiteSpace(fullName))
            errors.Add("Full name is required.");

        return errors;
    }

    public static List<string> ValidateLogin(string? email, string? password)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(email))
            errors.Add("Email is required.");

        if (string.IsNullOrWhiteSpace(password))
            errors.Add("Password is required.");

        return errors;
    }
}
