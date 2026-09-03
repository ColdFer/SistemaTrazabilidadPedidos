using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrazabilidadPedidos.Server.Data;
using TrazabilidadPedidos.Server.Services;
using TrazabilidadPedidos.Shared.DTOs.Payments;

namespace TrazabilidadPedidos.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly AppDbContext _context;

        public PaymentsController(
            IPaymentService paymentService,
            AppDbContext context)
        {
            _paymentService = paymentService;
            _context = context;
        }

        [HttpGet]
        [Authorize(Policy = "VerifyPayments")]
        public async Task<ActionResult<List<PaymentDto>>> GetAll()
        {
            return Ok(await _paymentService.GetAllAsync());
        }

        [HttpGet("pending")]
        [Authorize(Policy = "VerifyPayments")]
        public async Task<ActionResult<List<PaymentDto>>> GetPending()
        {
            return Ok(await _paymentService.GetPendingAsync());
        }

        [HttpGet("my-payments")]
        [Authorize(Policy = "ViewOwnOrders")]
        public async Task<ActionResult<List<PaymentDto>>> GetMyPayments()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (customer == null) return Unauthorized();

            return Ok(await _paymentService.GetByCustomerIdAsync(customer.Id));
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "VerifyPayments")]
        public async Task<ActionResult<PaymentDto>> GetById(int id)
        {
            var payment = await _paymentService.GetByIdAsync(id);
            if (payment == null) return NotFound();
            return Ok(payment);
        }

        [HttpPost]
        [Authorize(Policy = "CreateOrders")]
        public async Task<ActionResult<PaymentDto>> Create(
            CreatePaymentRequest request)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _paymentService.CreateAsync(request, userId.Value);
            if (result == null)
                return BadRequest(new { message = "No se pudo registrar el pago." });

            return Ok(result);
        }

        [HttpPut("{id:int}/verify")]
        [Authorize(Policy = "VerifyPayments")]
        public async Task<ActionResult<PaymentDto>> Verify(
            int id, VerifyPaymentRequest request)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _paymentService.VerifyAsync(id, request, userId.Value);
            if (result == null) return NotFound();

            return Ok(result);
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
