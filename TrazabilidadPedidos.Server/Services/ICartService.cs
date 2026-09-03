using TrazabilidadPedidos.Shared.DTOs.Cart;

namespace TrazabilidadPedidos.Server.Services
{
    public interface ICartService
    {
        Task<CartDto?> GetCartAsync(int customerId);
        Task<CartDto?> AddToCartAsync(int customerId, AddToCartRequest request);
        Task<CartDto?> UpdateQuantityAsync(int customerId, int cartItemId, UpdateCartItemRequest request);
        Task<CartDto?> RemoveFromCartAsync(int customerId, int cartItemId);
        Task<CartDto?> ClearCartAsync(int customerId);
    }
}
