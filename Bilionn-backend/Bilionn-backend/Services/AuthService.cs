using System.Security.Cryptography;
using System.Text;
using Bilionn_backend.DTOs.Auth;
using Bilionn_backend.Exceptions;
using Bilionn_backend.Mappings;
using Bilionn_backend.Models;
using Bilionn_backend.Repositories;

namespace Bilionn_backend.Services;

public class AuthService : IAuthService
{

    private readonly IUserRepository userRepository;
    private readonly IConfiguration _config;
  
    public AuthService(IUserRepository users, IConfiguration config)
    {
        userRepository = users;
        _config = config;
    }


    public async Task<UserResponse> LoginAsync(LoginRequest request)
    {
        var user = await userRepository.GetByEmailAsync(NormalizeEmail(request.Email));

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw AppException.Unauthorized("Email or password is incorrect.");

        if (user.AccountStatus != AccountStatus.ACTIVE)
            throw AppException.Forbidden($"This account is {user.AccountStatus.ToString().ToLower()}.");

        return user.ToResponse();
    }

   

    public async Task<UserResponse> BootstrapAdminAsync(BootstrapAdminRequest request, string? setupKey)
    {
        // Optional guard: if Setup:Key is configured, the caller must send it.
        var expected = _config["Setup:Key"];
        if (!string.IsNullOrEmpty(expected) &&
            !CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(expected), Encoding.UTF8.GetBytes(setupKey ?? "")))
            throw AppException.Forbidden("Invalid setup key.");

        if (await userRepository.AdminExistsAsync())
            throw AppException.Conflict("An admin account already exists; sign in to create additional accounts.");

        var email = NormalizeEmail(request.Email);
        if (await userRepository.EmailExistsAsync(email))
            throw AppException.Conflict("Email is already in use.");

        var admin = new User
        {
            UserId = Guid.NewGuid().ToString(),
            Name = request.Name.Trim(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = Role.ADMIN,
            AccountStatus = AccountStatus.ACTIVE
        };

        await userRepository.AddAsync(admin);
        return admin.ToResponse();
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    public async Task<UserResponse> GetCurrentUserAsync(string email)
    {
        var user = await userRepository.GetByEmailAsync(NormalizeEmail(email))
            ?? throw AppException.Unauthorized("User not found.");
        return user.ToResponse();
    }
}
