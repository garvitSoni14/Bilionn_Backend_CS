namespace Bilionn_backend.Validators;

using FluentValidation;

public static class ValidatorExtensions
{
    public static IRuleBuilderOptions<T, string> StrongPassword<T>(this IRuleBuilder<T, string> rule) =>
          rule.NotEmpty().MinimumLength(8).MaximumLength(72)
              .Matches("[A-Z]").WithMessage("Password needs an uppercase letter.")
              .Matches("[a-z]").WithMessage("Password needs a lowercase letter.")
              .Matches("[0-9]").WithMessage("Password needs a digit.")
              .Matches("[^A-Za-z0-9]").WithMessage("Password needs a special character.");

    public static IRuleBuilderOptions<T, string> IndianMobile<T>(this IRuleBuilder<T, string> rule) =>
        rule.NotEmpty().Matches(@"^[6-9]\d{9}$").WithMessage("Enter a valid 10-digit mobile number.");
}
