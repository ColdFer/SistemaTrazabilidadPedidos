using TrazabilidadPedidos.Shared.DTOs.Users;

namespace TrazabilidadPedidos.Server.Services
{
    public interface IManagedUserService
    {
        Task<List<ManagedUserDto>> GetAllAsync();

        Task<ManagedUserDto?> GetByIdAsync(int id);

        Task<ManagedUserDto?> CreateAsync(
            CreateManagedUserRequest request);

        Task<ManagedUserDto?> UpdateAsync(
            int id,
            UpdateManagedUserRequest request);

        Task<bool> DeactivateAsync(int id);
    }
}