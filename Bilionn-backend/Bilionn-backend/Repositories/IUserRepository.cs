using Bilionn_backend.Models;

namespace Bilionn_backend.Repositories;
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email, long? excludeId = null);
    Task AddAsync(User user);
        Task<bool> AdminExistsAsync();
        Task<List<User>> GetAllAsync();

    Task<User?> GetByUserIdAsync(string userId);
}