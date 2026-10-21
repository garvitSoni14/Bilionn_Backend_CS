using Bilionn_backend.Data;
using Bilionn_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Bilionn_backend.Repositories
{
    public class UserRepository: IUserRepository
    {
        private readonly AppDbContext appDbContext;
        public UserRepository(AppDbContext context)
        {
            appDbContext = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await appDbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> EmailExistsAsync(string email, long? excludeId = null)
        {
            return await appDbContext.Users.AnyAsync(u => u.Email == email && u.Id != excludeId);
        }

        public async Task<User?> GetByUserIdAsync(string userId)
        {
            return await appDbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task AddAsync(User user)
        {
            appDbContext.Users.Add(user);
            await appDbContext.SaveChangesAsync();
        }
        public async Task<bool> AdminExistsAsync() =>
    await appDbContext.Users.AnyAsync(u => u.Role == Role.ADMIN);

        public async Task<List<User>> GetAllAsync() =>
            await appDbContext.Users.AsNoTracking().OrderBy(u => u.CreatedAt).ToListAsync();
    }
}
