namespace Bilionn_backend.Services;

using Bilionn_backend.DTOs.Cashiers;
public interface ICashierService
{
    Task<EmployeeResponse> CreateAsync(CreateCashierRequest request, string adminUserId);
    Task<List<EmployeeResponse>> GetAllAsync();
    Task<EmployeeResponse> GetByCodeAsync(string code);
    Task<EmployeeResponse> UpdateAsync(string code, UpdateCashierRequest request);
    Task DeleteAsync(string code);
}
