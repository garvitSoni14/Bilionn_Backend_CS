namespace Bilionn_backend.Repositories;

using Bilionn_backend.Models;
using global::Bilionn_backend.Models;
public interface IEmployeeRepository
{
    Task<List<Employee>> GetAllAsync();
    Task<Employee?> GetByCodeAsync(string code);
    Task<bool> CodeExistsAsync(string code);
    Task<bool> PanExistsAsync(string pan, long? excludeId = null);
    Task<bool> AadhaarExistsAsync(string aadhaar, long? excludeId = null);
    Task AddAsync(Employee employee);
    Task SaveAsync();
    Task DeleteAsync(Employee employee);
}
