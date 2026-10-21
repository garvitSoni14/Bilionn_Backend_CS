using FluentValidation;

using Bilionn_backend.DTOs.Auth;

namespace Bilionn_backend.Validators;

public class BootstrapAdminRequestValidator : AbstractValidator<BootstrapAdminRequest>
{
    public BootstrapAdminRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(255);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(72)
            .Matches("[A-Z]").WithMessage("Password needs an uppercase letter.")
            .Matches("[a-z]").WithMessage("Password needs a lowercase letter.")
            .Matches("[0-9]").WithMessage("Password needs a digit.")
            .Matches("[^A-Za-z0-9]").WithMessage("Password needs a special character.");
    }
}

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}
