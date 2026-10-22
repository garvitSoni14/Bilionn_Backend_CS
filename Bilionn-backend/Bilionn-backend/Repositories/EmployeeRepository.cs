namespace Bilionn_backend.Repositories;

using Bilionn_backend.Data;
using Bilionn_backend.Models;
using Microsoft.EntityFrameworkCore;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext appDbContext;
    public EmployeeRepository(AppDbContext context) => appDbContext = context;

    public async Task<List<Employee>> GetAllAsync() =>
        await appDbContext.Employees.AsNoTracking()
            .Include(e => e.UserAccount)
            .OrderBy(e => e.EmployeeCode)
            .ToListAsync();

    public async Task<Employee?> GetByCodeAsync(string code) =>
        await appDbContext.Employees.Include(e => e.UserAccount)
            .FirstOrDefaultAsync(e => e.EmployeeCode == code);

    public async Task<bool> CodeExistsAsync(string code) =>
        await appDbContext.Employees.AnyAsync(e => e.EmployeeCode == code);

    public async Task<bool> PanExistsAsync(string pan, long? excludeId = null) =>
        await appDbContext.Employees.AnyAsync(e => e.PanNumber == pan && e.Id != excludeId);

    public async Task<bool> AadhaarExistsAsync(string aadhaar, long? excludeId = null) =>
        await appDbContext.Employees.AnyAsync(e => e.AadhaarNumber == aadhaar && e.Id != excludeId);

    public async Task AddAsync(Employee employee)
    {
        appDbContext.Employees.Add(employee);   
        await appDbContext.SaveChangesAsync();
    }

    public async Task SaveAsync() => await appDbContext.SaveChangesAsync();

    public async Task DeleteAsync(Employee employee)
    {
        appDbContext.Employees.Remove(employee);
        await appDbContext.SaveChangesAsync();
    }
}
