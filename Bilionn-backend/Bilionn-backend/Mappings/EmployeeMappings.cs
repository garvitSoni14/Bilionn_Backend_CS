namespace Bilionn_backend.Mappings;

using Bilionn_backend.DTOs.Cashiers;
using Bilionn_backend.Models;

public static class EmployeeMappings
{
    public static Address ToModel(this AddressDto d) => new()
    {
        HouseNo = d.HouseNo?.Trim(),
        Street = d.Street.Trim(),
        Area = d.Area?.Trim(),
        City = d.City.Trim(),
        State = d.State.Trim(),
        Pincode = d.Pincode.Trim()
    };

    public static BankDetails ToModel(this BankDetailsDto d) => new()
    {
        AccountHolderName = d.AccountHolderName.Trim(),
        AccountNumber = d.AccountNumber.Trim(),
        IfscCode = d.IfscCode.Trim().ToUpperInvariant(),
        BankName = d.BankName.Trim()
    };

    public static EmergencyContact ToModel(this EmergencyContactDto d) => new()
    {
        Name = d.Name.Trim(),
        MobileNumber = d.MobileNumber.Trim(),
        Relationship = d.Relationship.Trim()
    };

    private static AddressDto? ToDto(Address? a) => a is null ? null :
        new(a.HouseNo, a.Street ?? "", a.Area, a.City ?? "", a.State ?? "", a.Pincode ?? "");

    private static BankDetailsDto? ToDto(BankDetails? b) => b is null ? null :
        new(b.AccountHolderName ?? "", b.AccountNumber ?? "", b.IfscCode ?? "", b.BankName ?? "");

    private static EmergencyContactDto? ToDto(EmergencyContact? c) => c is null ? null :
        new(c.Name ?? "", c.MobileNumber ?? "", c.Relationship ?? "");

    public static EmployeeResponse ToResponse(this Employee e)
    {
        var u = e.UserAccount;
        return new EmployeeResponse(
            e.Id, e.EmployeeCode, e.FullName, e.DateOfBirth, e.MobileNumber,
            ToDto(e.Address), e.JoiningDate, e.Salary, e.PanNumber, e.AadhaarNumber,
            ToDto(e.BankDetails), ToDto(e.EmergencyContact),
            u.Email, u.Role.ToString(), u.AccountStatus.ToString(), u.UserId!,
            e.CreatedAt, e.UpdatedAt);
    }
}
