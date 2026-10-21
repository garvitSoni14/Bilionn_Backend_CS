using Bilionn_backend.DTOs.Auth;
using Bilionn_backend.Models;

namespace Bilionn_backend.Mappings;

public static class UserMappings
{
    public static UserResponse ToResponse(this User u) =>
        new(u.UserId!, u.Name ?? string.Empty, u.Email, u.Role.ToString(),
            u.AccountStatus.ToString(), u.CreatedAt, u.UpdatedAt);
}
