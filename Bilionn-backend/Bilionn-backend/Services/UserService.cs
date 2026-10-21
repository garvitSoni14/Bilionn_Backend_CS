using Bilionn_backend.DTOs.Auth;
using Bilionn_backend.Mappings;
using Bilionn_backend.Repositories;

namespace Bilionn_backend.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _users;

    public UserService(IUserRepository users) => _users = users;

    public async Task<List<UserResponse>> GetAllAsync() =>
        (await _users.GetAllAsync()).Select(u => u.ToResponse()).ToList();
}
