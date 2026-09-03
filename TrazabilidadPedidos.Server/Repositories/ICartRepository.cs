using TrazabilidadPedidos.Shared.Entities;

namespace TrazabilidadPedidos.Server.Repositories
{
    public interface ICartRepository
    {
        Task<Cart?> GetByCustomerIdAsync(int customerId);
        Task<CartItem?> GetCartItemAsync(int cartId, int productId);
        Task AddToCartAsync(CartItem item);
        void RemoveCartItem(CartItem item);
        Task SaveChangesAsync();
    }
}
