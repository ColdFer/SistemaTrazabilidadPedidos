using TrazabilidadPedidos.Server.Data;
using TrazabilidadPedidos.Server.Repositories;
using TrazabilidadPedidos.Shared.DTOs.Cart;
using TrazabilidadPedidos.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace TrazabilidadPedidos.Server.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly AppDbContext _context;

        public CartService(
            ICartRepository cartRepository,
            AppDbContext context)
        {
            _cartRepository = cartRepository;
            _context = context;
        }

        public async Task<CartDto?> GetCartAsync(int customerId)
        {
            var cart = await _cartRepository.GetByCustomerIdAsync(customerId);
            if (cart == null) return null;
            return MapToDto(cart);
        }

        public async Task<CartDto?> AddToCartAsync(int customerId, AddToCartRequest request)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == request.ProductId);

            if (product == null || !product.IsActive)
                return null;

            if (product.CurrentStock < request.Quantity)
                return null;

            var cart = await _cartRepository.GetByCustomerIdAsync(customerId);

            if (cart == null)
            {
                cart = new Cart
                {
                    CustomerId = customerId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var existingItem = await _cartRepository.GetCartItemAsync(cart.Id, request.ProductId);

            if (existingItem != null)
            {
                var newQty = existingItem.Quantity + request.Quantity;
                if (newQty > product.CurrentStock)
                    return null;

                existingItem.Quantity = newQty;
                existingItem.UpdatedAt = DateTime.Now;
            }
            else
            {
                var newItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                await _cartRepository.AddToCartAsync(newItem);
            }

            await _cartRepository.SaveChangesAsync();
            cart = await _cartRepository.GetByCustomerIdAsync(customerId);
            return cart != null ? MapToDto(cart) : null;
        }

        public async Task<CartDto?> UpdateQuantityAsync(int customerId, int cartItemId, UpdateCartItemRequest request)
        {
            var cart = await _cartRepository.GetByCustomerIdAsync(customerId);
            if (cart == null) return null;

            var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);
            if (item == null) return null;

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == item.ProductId);

            if (product == null || request.Quantity > product.CurrentStock)
                return null;

            item.Quantity = request.Quantity;
            item.UpdatedAt = DateTime.Now;
            await _cartRepository.SaveChangesAsync();

            cart = await _cartRepository.GetByCustomerIdAsync(customerId);
            return cart != null ? MapToDto(cart) : null;
        }

        public async Task<CartDto?> RemoveFromCartAsync(int customerId, int cartItemId)
        {
            var cart = await _cartRepository.GetByCustomerIdAsync(customerId);
            if (cart == null) return null;

            var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);
            if (item == null) return null;

            _cartRepository.RemoveCartItem(item);
            await _cartRepository.SaveChangesAsync();

            cart = await _cartRepository.GetByCustomerIdAsync(customerId);
            return cart != null ? MapToDto(cart) : null;
        }

        public async Task<CartDto?> ClearCartAsync(int customerId)
        {
            var cart = await _cartRepository.GetByCustomerIdAsync(customerId);
            if (cart == null) return null;

            var items = cart.Items.ToList();
            foreach (var item in items)
            {
                _cartRepository.RemoveCartItem(item);
            }

            await _cartRepository.SaveChangesAsync();
            cart = await _cartRepository.GetByCustomerIdAsync(customerId);
            return cart != null ? MapToDto(cart) : null;
        }

        private static CartDto MapToDto(Cart cart)
        {
            var items = cart.Items.Select(i =>
            {
                var unitPrice = i.Product?.Price ?? 0;
                return new CartItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? string.Empty,
                    ProductCode = i.Product?.Code ?? string.Empty,
                    ProductImage = i.Product?.Image,
                    UnitPrice = unitPrice,
                    Quantity = i.Quantity,
                    AvailableStock = i.Product?.CurrentStock ?? 0,
                    Subtotal = unitPrice * i.Quantity
                };
            }).ToList();

            return new CartDto
            {
                Id = cart.Id,
                CustomerId = cart.CustomerId,
                Items = items,
                Total = items.Sum(i => i.UnitPrice * i.Quantity),
                TotalItems = items.Sum(i => i.Quantity)
            };
        }
    }
}
