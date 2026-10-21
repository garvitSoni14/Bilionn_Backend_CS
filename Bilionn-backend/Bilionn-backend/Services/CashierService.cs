namespace Bilionn_backend.Services;

using Bilionn_backend.DTOs.Cashiers;
using Bilionn_backend.Exceptions;
using Bilionn_backend.Mappings;
using Bilionn_backend.Models;
using Bilionn_backend.Repositories;


public class CashierService : ICashierService
{
    private readonly IEmployeeRepository employeeRepository;
    private readonly IUserRepository userRepository;

    public CashierService(IEmployeeRepository employees, IUserRepository users)
    {
        employeeRepository = employees;
        userRepository = users;
    }

    public async Task<EmployeeResponse> CreateAsync(CreateCashierRequest r, string adminUserId)
    {
        var email = NormalizeEmail(r.Email);
        if (await userRepository.EmailExistsAsync(email))
            throw AppException.Conflict("Email is already in use.");

        var code = string.IsNullOrWhiteSpace(r.EmployeeCode)
            ? "EMP-" + Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()
            : r.EmployeeCode.Trim();
        if (await employeeRepository.CodeExistsAsync(code))
            throw AppException.Conflict("Employee code is already in use.");

        var pan = Clean(r.PanNumber)?.ToUpperInvariant();
        var aadhaar = Clean(r.AadhaarNumber);
        await EnsureUniqueIdsAsync(pan, aadhaar, null);

        var admin = await userRepository.GetByUserIdAsync(adminUserId)
            ?? throw AppException.Unauthorized("Signed-in administrator was not found.");

        var employee = new Employee
        {
            EmployeeCode = code,
            FullName = r.FullName.Trim(),
            DateOfBirth = r.DateOfBirth,
            MobileNumber = r.MobileNumber.Trim(),
            Address = r.Address.ToModel(),
            JoiningDate = r.JoiningDate,
            Salary = r.Salary,
            PanNumber = pan,
            AadhaarNumber = aadhaar,
            BankDetails = r.BankDetails.ToModel(),
            EmergencyContact = r.EmergencyContact.ToModel(),
            CreatedByAdmin = admin,
            UserAccount = new User
            {
                UserId = Guid.NewGuid().ToString(),
                Name = r.FullName.Trim(),
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(r.Password),
                Role = Role.CASHIER,
                AccountStatus = AccountStatus.ACTIVE
            }
        };

        await employeeRepository.AddAsync(employee);   // saves user + employee together
        return employee.ToResponse();
    }

    public async Task<List<EmployeeResponse>> GetAllAsync() =>
        (await employeeRepository.GetAllAsync()).Select(e => e.ToResponse()).ToList();

    public async Task<EmployeeResponse> GetByCodeAsync(string code) =>
        (await GetOrThrowAsync(code)).ToResponse();

    public async Task<EmployeeResponse> UpdateAsync(string code, UpdateCashierRequest r)
    {
        var employee = await GetOrThrowAsync(code);
        var account = employee.UserAccount;

        if (r.FullName is not null)
        {
            employee.FullName = r.FullName.Trim();
            account.Name = r.FullName.Trim();
        }
        if (r.DateOfBirth is not null) employee.DateOfBirth = r.DateOfBirth.Value;
        if (r.MobileNumber is not null) employee.MobileNumber = r.MobileNumber.Trim();
        if (r.Address is not null) employee.Address = r.Address.ToModel();
        if (r.JoiningDate is not null) employee.JoiningDate = r.JoiningDate.Value;
        if (r.Salary is not null) employee.Salary = r.Salary.Value;
        if (r.BankDetails is not null) employee.BankDetails = r.BankDetails.ToModel();
        if (r.EmergencyContact is not null) employee.EmergencyContact = r.EmergencyContact.ToModel();

        if (!string.IsNullOrWhiteSpace(r.Email))
        {
            var email = NormalizeEmail(r.Email);
            if (email != account.Email)
            {
                if (await userRepository.EmailExistsAsync(email, account.Id))
                    throw AppException.Conflict("Email is already in use.");
                account.Email = email;
            }
        }

        if (r.AccountStatus is not null) account.AccountStatus = r.AccountStatus.Value;

        if (r.PanNumber is not null)
        {
            var pan = Clean(r.PanNumber)?.ToUpperInvariant();
            await EnsureUniqueIdsAsync(pan, null, employee.Id);
            employee.PanNumber = pan;
        }
        if (r.AadhaarNumber is not null)
        {
            var aadhaar = Clean(r.AadhaarNumber);
            await EnsureUniqueIdsAsync(null, aadhaar, employee.Id);
            employee.AadhaarNumber = aadhaar;
        }

        await employeeRepository.SaveAsync();
        return employee.ToResponse();
    }

    public async Task DeleteAsync(string code)
    {
        var employee = await GetOrThrowAsync(code);
        employee.UserAccount.AccountStatus = AccountStatus.DISABLED;  // same as your Java delete
        await employeeRepository.DeleteAsync(employee);
    }

    // ---- helpers ----
    private async Task<Employee> GetOrThrowAsync(string code) =>
        await employeeRepository.GetByCodeAsync(code)
        ?? throw AppException.NotFound($"Employee not found: {code}");

    private async Task EnsureUniqueIdsAsync(string? pan, string? aadhaar, long? excludeId)
    {
        if (pan is not null && await employeeRepository.PanExistsAsync(pan, excludeId))
            throw AppException.Conflict("PAN number is already in use.");
        if (aadhaar is not null && await employeeRepository.AadhaarExistsAsync(aadhaar, excludeId))
            throw AppException.Conflict("Aadhaar number is already in use.");
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
    private static string? Clean(string? v) => string.IsNullOrWhiteSpace(v) ? null : v.Trim();
}
