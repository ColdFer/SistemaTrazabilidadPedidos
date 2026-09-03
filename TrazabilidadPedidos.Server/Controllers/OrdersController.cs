using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrazabilidadPedidos.Server.Data;
using TrazabilidadPedidos.Server.Services;
using TrazabilidadPedidos.Shared.DTOs.Orders;
using TrazabilidadPedidos.Shared.Enums;

namespace TrazabilidadPedidos.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly AppDbContext _context;

        public OrdersController(
            IOrderService orderService,
            AppDbContext context)
        {
            _orderService = orderService;
            _context = context;
        }

        [HttpGet]
        [Authorize(Policy = "ManageOrders")]
        public async Task<ActionResult<List<OrderDto>>> GetAll()
        {
            var orders = await _orderService.GetAllAsync();
            return Ok(orders);
        }

        [HttpGet("my-orders")]
        public async Task<ActionResult<List<OrderDto>>> GetMyOrders()
        {
            var customerId = await GetCustomerIdAsync();
            if (customerId == null) return Unauthorized();

            var orders = await _orderService.GetByCustomerIdAsync(customerId.Value);
            return Ok(orders);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDto>> GetById(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound(new { message = "Pedido no encontrado." });

            if (User.IsInRole("Customer"))
            {
                var customerId = await GetCustomerIdAsync();
                if (customerId == null || order.CustomerId != customerId.Value)
                    return Forbid();
            }

            return Ok(order);
        }

        [HttpPost]
        [Authorize(Policy = "CreateOrders")]
        public async Task<ActionResult<OrderDto>> Create(CreateOrderRequest request)
        {
            var customerId = await GetCustomerIdAsync();
            if (customerId == null) return Unauthorized();

            request.CustomerId = customerId.Value;

            var order = await _orderService.CreateAsync(request);
            if (order == null)
                return BadRequest(new { message = "No se pudo crear el pedido. Verifique el stock disponible." });

            return Ok(order);
        }

        [HttpPut("{id:int}/status")]
        [Authorize(Policy = "ManageOrders")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusRequest request)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _orderService.UpdateStatusAsync(
                id, request.StatusId, request.Observation, userId.Value);

            if (!result)
                return BadRequest(new { message = "No se pudo actualizar el estado del pedido." });

            return Ok(new { message = "Estado actualizado correctamente." });
        }

        [HttpGet("statuses")]
        public async Task<ActionResult<List<OrderStatusDto>>> GetStatuses()
        {
            var statuses = await _orderService.GetStatusesAsync();
            return Ok(statuses);
        }

        [HttpPost("{id:int}/accept")]
        [Authorize(Policy = "ManageOrders")]
        public async Task<IActionResult> Accept(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _orderService.AcceptOrderAsync(id, userId.Value);
            if (!result)
                return BadRequest(new { message = "No se pudo aceptar el pedido." });

            return Ok(new { message = "Pedido aceptado. Inventario descontado." });
        }

        [HttpPost("from-cart")]
        [Authorize(Policy = "CreateOrders")]
        public async Task<ActionResult<OrderDto>> CreateFromCart(
            [FromBody] CreateFromCartRequest body)
        {
            var customerId = await GetCustomerIdAsync();
            if (customerId == null) return Unauthorized();

            var cart = await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId.Value);

            if (cart == null || !cart.Items.Any())
                return BadRequest(new { message = "El carrito está vacío." });

            var request = new CreateOrderRequest
            {
                CustomerId = customerId.Value,
                DeliveryLatitude = body.DeliveryLatitude,
                DeliveryLongitude = body.DeliveryLongitude,
                DeliveryAddress = body.DeliveryAddress,
                DeliveryReference = body.DeliveryReference,
                Items = cart.Items.Select(i => new OrderItemRequest
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            };

            var order = await _orderService.CreateAsync(request);
            if (order == null)
                return BadRequest(new { message = "No se pudo crear el pedido." });

            foreach (var item in cart.Items.ToList())
            {
                _context.CartItems.Remove(item);
            }
            cart.Items.Clear();
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();

            return Ok(order);
        }

        [HttpGet("{id}/history")]
        public async Task<ActionResult<List<OrderStatusHistoryDto>>> GetHistory(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            var customerId = await GetCustomerIdAsync();
            if (User.IsInRole("Customer") && order.CustomerId != customerId)
                return Forbid();

            var history = await _context.OrderStatusHistories
                .Include(h => h.OrderStatus)
                .Include(h => h.User)
                .Where(h => h.OrderId == id)
                .OrderBy(h => h.StatusDate)
                .Select(h => new OrderStatusHistoryDto
                {
                    Id = h.Id,
                    StatusName = h.OrderStatus!.Name,
                    StatusDate = h.StatusDate,
                    Observation = h.Observation,
                    UserName = h.User != null
                        ? $"{h.User.FirstName} {h.User.LastName}"
                        : "Sistema"
                })
                .ToListAsync();

            return Ok(history);
        }

        private int? GetUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(claim, out var userId))
                return userId;
            return null;
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

    public class UpdateOrderStatusRequest
    {
        public int StatusId { get; set; }
        public string? Observation { get; set; }
    }
}
