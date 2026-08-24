using TrazabilidadPedidos.Shared.DTOs;

namespace TrazabilidadPedidos.Server.Services
{
    public interface IAuthService
    {
        Task<AuthResponse?> LoginAsync(LoginRequest request);
        Task<AuthResponse?> RegisterAsync(RegisterRequest request);
    }
}

