using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrazabilidadPedidos.Server.Data;
using TrazabilidadPedidos.Server.Services;
using TrazabilidadPedidos.Shared.DTOs.Dispatches;

namespace TrazabilidadPedidos.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DeliveriesController : ControllerBase
    {
        private readonly IDeliveryService _deliveryService;
        private readonly AppDbContext _context;

        public DeliveriesController(
            IDeliveryService deliveryService,
            AppDbContext context)
        {
            _deliveryService = deliveryService;
            _context = context;
        }

        [HttpGet]
        [Authorize(Policy = "ScheduleDeliveries")]
        public async Task<ActionResult<List<DeliveryDto>>>
            GetAll()
        {
            var deliveries =
                await _deliveryService.GetAllAsync();

            return Ok(deliveries);
        }

        [HttpGet("my-deliveries")]
        [Authorize(Policy = "ViewAssignedDeliveries")]
        public async Task<ActionResult<List<DeliveryDto>>>
            GetMyDeliveries()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var driver = await _context.DeliveryDrivers
                .FirstOrDefaultAsync(d => d.UserId == userId.Value);

            if (driver == null)
                return Ok(new List<DeliveryDto>());

            var allDeliveries = await _deliveryService.GetAllAsync();
            var myDeliveries = allDeliveries
                .Where(d => d.DeliveryDriverId == driver.Id)
                .ToList();

            return Ok(myDeliveries);
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "ScheduleDeliveries")]
        public async Task<ActionResult<DeliveryDto>>
            GetById(int id)
        {
            var delivery =
                await _deliveryService.GetByIdAsync(id);

            if (delivery == null)
            {
                return NotFound(new
                {
                    message = "Despacho no encontrado."
                });
            }

            return Ok(delivery);
        }

        [HttpPost]
        [Authorize(Policy = "ScheduleDeliveries")]
        public async Task<ActionResult<DeliveryDto>>
            Create(CreateDeliveryRequest request)
        {
            var delivery =
                await _deliveryService.CreateAsync(request);

            if (delivery == null)
            {
                return BadRequest(new
                {
                    message =
                        "No se pudo programar el despacho. Verifique el pedido, la dirección, el repartidor y que el pedido no tenga otro despacho."
                });
            }

            return Ok(delivery);
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "ScheduleDeliveries")]
        public async Task<ActionResult<DeliveryDto>>
            Update(
                int id,
                UpdateDeliveryRequest request)
        {
            var existing =
                await _deliveryService.GetByIdAsync(id);

            if (existing == null)
            {
                return NotFound(new
                {
                    message = "Despacho no encontrado."
                });
            }

            var delivery =
                await _deliveryService.UpdateAsync(
                    id,
                    request);

            if (delivery == null)
            {
                return BadRequest(new
                {
                    message =
                        "No se pudo actualizar el despacho. Verifique la dirección y el repartidor."
                });
            }

            return Ok(delivery);
        }

        [HttpPut("{id:int}/status")]
        [Authorize(Policy = "UpdateDeliveryStatus")]
        public async Task<ActionResult<DeliveryDto>>
            ChangeStatus(
                int id,
                ChangeDeliveryStatusRequest request)
        {
            try
            {
                var delivery =
                    await _deliveryService
                        .ChangeStatusAsync(
                            id,
                            request);

                if (delivery == null)
                {
                    return BadRequest(new
                    {
                        message =
                            "No se pudo cambiar el estado del despacho."
                    });
                }

                return Ok(delivery);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet("orders")]
        [Authorize(Policy = "ScheduleDeliveries")]
        public async Task<ActionResult<List<DeliveryOrderDto>>>
            GetOrders()
        {
            var orders =
                await _deliveryService.GetOrdersAsync();

            return Ok(orders);
        }

        [HttpGet("addresses")]
        [Authorize(Policy = "ScheduleDeliveries")]
        public async Task<ActionResult<List<DeliveryAddressDto>>>
            GetAddresses()
        {
            var addresses =
                await _deliveryService.GetAddressesAsync();

            return Ok(addresses);
        }

        [HttpGet("drivers")]
        [Authorize(Policy = "AssignDeliveryDrivers")]
        public async Task<ActionResult<List<DeliveryDriverDto>>>
            GetDrivers()
        {
            var drivers =
                await _deliveryService.GetDriversAsync();

            return Ok(drivers);
        }

        private int? GetUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(claim, out var userId))
                return userId;
            return null;
        }
    }
}
