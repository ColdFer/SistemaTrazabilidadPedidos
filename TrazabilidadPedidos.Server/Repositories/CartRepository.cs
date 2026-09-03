using Microsoft.EntityFrameworkCore;
using TrazabilidadPedidos.Server.Data;
using TrazabilidadPedidos.Shared.Entities;

namespace TrazabilidadPedidos.Server.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetByCustomerIdAsync(int customerId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p!.Category)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        public async Task<CartItem?> GetCartItemAsync(int cartId, int productId)
        {
            return await _context.CartItems
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i =>
                    i.CartId == cartId && i.ProductId == productId);
        }

        public async Task AddToCartAsync(CartItem item)
        {
            await _context.CartItems.AddAsync(item);
        }

        public void RemoveCartItem(CartItem item)
        {
            _context.CartItems.Remove(item);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
