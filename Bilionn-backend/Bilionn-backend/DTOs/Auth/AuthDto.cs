using Bilionn_backend.Models;

namespace Bilionn_backend.DTOs.Auth;
public record BootstrapAdminRequest(string Name, string Email, string Password);
public record LoginRequest(string Email, string Password);
//public record AuthResponse(string Email, string Token, string Role, DateTime ExpiresAt);
public record UserResponse(
    string UserId, string Name, string Email, string Role,
    string AccountStatus, DateTime CreatedAt, DateTime UpdatedAt);
