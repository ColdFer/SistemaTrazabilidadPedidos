using TrazabilidadPedidos.Shared.DTOs.Customers;

namespace TrazabilidadPedidos.Server.Services
{
    public interface ICustomerService
    {
        Task<List<CustomerDto>> GetAllAsync();

        Task<CustomerDto?> GetByIdAsync(int id);

        Task<CustomerDto?> CreateAsync(
            CreateCustomerRequest request);

        Task<CustomerDto?> UpdateAsync(
            int id,
            UpdateCustomerRequest request);

        Task<bool> DeactivateAsync(int id);
    }
}