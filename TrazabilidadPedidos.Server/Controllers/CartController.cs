using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrazabilidadPedidos.Server.Data;
using TrazabilidadPedidos.Server.Services;
using TrazabilidadPedidos.Shared.DTOs.Cart;

namespace TrazabilidadPedidos.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "CreateOrders")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly AppDbContext _context;

        public CartController(
            ICartService cartService,
            AppDbContext context)
        {
            _cartService = cartService;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<CartDto>> GetCart()
        {
            var customerId = await GetCustomerIdAsync();
            if (customerId == null) return Unauthorized();

            var cart = await _cartService.GetCartAsync(customerId.Value);
            if (cart == null)
                return Ok(new CartDto { CustomerId = customerId.Value });

            return Ok(cart);
        }

        [HttpPost("items")]
        public async Task<ActionResult<CartDto>> AddItem(AddToCartRequest request)
        {
            var customerId = await GetCustomerIdAsync();
            if (customerId == null) return Unauthorized();

            var cart = await _cartService.AddToCartAsync(customerId.Value, request);
            if (cart == null)
                return BadRequest(new { message = "No se pudo agregar el producto al carrito." });

            return Ok(cart);
        }

        [HttpPut("items/{itemId:int}")]
        public async Task<ActionResult<CartDto>> UpdateItem(int itemId, UpdateCartItemRequest request)
        {
            var customerId = await GetCustomerIdAsync();
            if (customerId == null) return Unauthorized();

            var cart = await _cartService.UpdateQuantityAsync(customerId.Value, itemId, request);
            if (cart == null)
                return BadRequest(new { message = "No se pudo actualizar la cantidad." });

            return Ok(cart);
        }

        [HttpDelete("items/{itemId:int}")]
        public async Task<ActionResult<CartDto>> RemoveItem(int itemId)
        {
            var customerId = await GetCustomerIdAsync();
            if (customerId == null) return Unauthorized();

            var cart = await _cartService.RemoveFromCartAsync(customerId.Value, itemId);
            if (cart == null)
                return BadRequest(new { message = "No se pudo eliminar el producto del carrito." });

            return Ok(cart);
        }

        [HttpDelete]
        public async Task<ActionResult<CartDto>> ClearCart()
        {
            var customerId = await GetCustomerIdAsync();
            if (customerId == null) return Unauthorized();

            var cart = await _cartService.ClearCartAsync(customerId.Value);
            return Ok(cart);
        }

        private async Task<int?> GetCustomerIdAsync()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(claim, out var userId))
                return null;

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId);

            return customer?.Id;
        }
    }
}
