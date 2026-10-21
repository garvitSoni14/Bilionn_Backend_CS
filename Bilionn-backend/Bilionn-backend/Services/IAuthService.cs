using Bilionn_backend.DTOs.Auth;

namespace Bilionn_backend.Services;

public interface IAuthService
{
    Task<UserResponse> BootstrapAdminAsync(BootstrapAdminRequest request, string? setupKey);
    Task<UserResponse> LoginAsync(LoginRequest request);

    Task<UserResponse> GetCurrentUserAsync(string email);

}
