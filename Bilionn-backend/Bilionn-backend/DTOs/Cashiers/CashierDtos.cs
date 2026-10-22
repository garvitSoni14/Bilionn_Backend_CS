using Bilionn_backend.Models;

namespace Bilionn_backend.DTOs.Cashiers;


public record AddressDto(string? HouseNo, string Street, string? Area, string City, string State, string Pincode);
public record BankDetailsDto(string AccountHolderName, string AccountNumber, string IfscCode, string BankName);
public record EmergencyContactDto(string Name, string MobileNumber, string Relationship);

public record CreateCashierRequest(
    string? EmployeeCode, string FullName, DateOnly DateOfBirth, string MobileNumber,
    AddressDto Address, DateOnly JoiningDate, decimal Salary,
    string? PanNumber, string? AadhaarNumber,
    BankDetailsDto BankDetails, EmergencyContactDto EmergencyContact,
    string Email, string Password);

// Every field optional: null means "leave unchanged". QQQQUUUEEEERRRRYYYY ????????
public record UpdateCashierRequest(
    string? FullName, DateOnly? DateOfBirth, string? MobileNumber, AddressDto? Address,
    DateOnly? JoiningDate, decimal? Salary, string? PanNumber, string? AadhaarNumber,
    BankDetailsDto? BankDetails, EmergencyContactDto? EmergencyContact,
    string? Email, AccountStatus? AccountStatus);

public record EmployeeResponse(
    long Id, string EmployeeCode, string FullName, DateOnly DateOfBirth, string MobileNumber,
    AddressDto? Address, DateOnly JoiningDate, decimal Salary,
    string? PanNumber, string? AadhaarNumber,
    BankDetailsDto? BankDetails, EmergencyContactDto? EmergencyContact,
    string Email, string Role, string AccountStatus, string UserId,
    DateTime CreatedAt, DateTime UpdatedAt);