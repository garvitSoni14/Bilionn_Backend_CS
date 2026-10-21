using Bilionn_backend.DTOs.Auth;

namespace Bilionn_backend.Services;


public interface IUserService
{
    Task<List<UserResponse>> GetAllAsync();
}
