namespace Bilionn_backend.Validators;

using Bilionn_backend.DTOs.Cashiers;
using FluentValidation;

public class AddressDtoValidator : AbstractValidator<AddressDto>
{
    public AddressDtoValidator()
    {
        RuleFor(x => x.HouseNo).MaximumLength(50);
        RuleFor(x => x.Street).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Area).MaximumLength(150);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.State).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Pincode).Matches(@"^[1-9]\d{5}$").WithMessage("Pincode must be 6 digits.");
    }
}

public class BankDetailsDtoValidator : AbstractValidator<BankDetailsDto>
{
    public BankDetailsDtoValidator()
    {
        RuleFor(x => x.AccountHolderName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.AccountNumber).Matches(@"^\d{9,18}$").WithMessage("Account number must contain 9 to 18 digits.");
        RuleFor(x => x.IfscCode).Matches(@"^[A-Za-z]{4}0[A-Za-z0-9]{6}$").WithMessage("Enter a valid IFSC code.");
        RuleFor(x => x.BankName).NotEmpty().MaximumLength(100);
    }
}

public class EmergencyContactDtoValidator : AbstractValidator<EmergencyContactDto>
{
    public EmergencyContactDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.MobileNumber).IndianMobile();
        RuleFor(x => x.Relationship).NotEmpty().MaximumLength(50);
    }
}

public class CreateCashierRequestValidator : AbstractValidator<CreateCashierRequest>
{
    public CreateCashierRequestValidator()
    {
        RuleFor(x => x.EmployeeCode).MaximumLength(30)
            .Matches(@"^[A-Za-z0-9-]+$").WithMessage("Employee code may contain only letters, numbers and hyphens.")
            .When(x => !string.IsNullOrWhiteSpace(x.EmployeeCode));

        RuleFor(x => x.FullName).NotEmpty().Length(2, 100);

        RuleFor(x => x.DateOfBirth)
            .Must(d => d.Year > 1900 && d <= DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Enter a valid date of birth (not in the future).");

        RuleFor(x => x.MobileNumber).IndianMobile();
        RuleFor(x => x.Address).SetValidator(new AddressDtoValidator());

        RuleFor(x => x.JoiningDate).Must(d => d != default).WithMessage("Joining date is required.");

        RuleFor(x => x.Salary).GreaterThan(0)
            .PrecisionScale(12, 2, true).WithMessage("Salary can have at most 10 integer digits and 2 decimals.");

        RuleFor(x => x.PanNumber).Matches(@"^[A-Za-z]{5}\d{4}[A-Za-z]$")
            .WithMessage("PAN must be in the format ABCDE1234F.")
            .When(x => !string.IsNullOrWhiteSpace(x.PanNumber));

        RuleFor(x => x.AadhaarNumber).Matches(@"^\d{12}$")
            .WithMessage("Aadhaar must be 12 digits.")
            .When(x => !string.IsNullOrWhiteSpace(x.AadhaarNumber));

        RuleFor(x => x.BankDetails).SetValidator(new BankDetailsDtoValidator());
        RuleFor(x => x.EmergencyContact).SetValidator(new EmergencyContactDtoValidator());
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(120);
        RuleFor(x => x.Password).StrongPassword();
    }
}

public class UpdateCashierRequestValidator : AbstractValidator<UpdateCashierRequest>
{
    public UpdateCashierRequestValidator()
    {
        RuleFor(x => x.FullName).Length(2, 100).When(x => x.FullName is not null);
        RuleFor(x => x.DateOfBirth)
            .Must(d => d!.Value.Year > 1900 && d.Value <= DateOnly.FromDateTime(DateTime.Today))
            .When(x => x.DateOfBirth is not null)
            .WithMessage("Enter a valid date of birth (not in the future).");
        RuleFor(x => x.MobileNumber!).IndianMobile().When(x => x.MobileNumber is not null);
        RuleFor(x => x.Address!).SetValidator(new AddressDtoValidator()).When(x => x.Address is not null);
        RuleFor(x => x.Salary).GreaterThan(0).PrecisionScale(12, 2, true).When(x => x.Salary is not null);
        RuleFor(x => x.PanNumber).Matches(@"^[A-Za-z]{5}\d{4}[A-Za-z]$")
            .When(x => !string.IsNullOrWhiteSpace(x.PanNumber));
        RuleFor(x => x.AadhaarNumber).Matches(@"^\d{12}$")
            .When(x => !string.IsNullOrWhiteSpace(x.AadhaarNumber));
        RuleFor(x => x.BankDetails!).SetValidator(new BankDetailsDtoValidator()).When(x => x.BankDetails is not null);
        RuleFor(x => x.EmergencyContact!).SetValidator(new EmergencyContactDtoValidator()).When(x => x.EmergencyContact is not null);
        RuleFor(x => x.Email).EmailAddress().MaximumLength(120).When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.AccountStatus).IsInEnum().When(x => x.AccountStatus is not null);
    }
}