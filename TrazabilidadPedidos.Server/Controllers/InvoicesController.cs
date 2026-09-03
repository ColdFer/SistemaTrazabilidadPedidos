using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrazabilidadPedidos.Server.Data;
using TrazabilidadPedidos.Server.Services;
using TrazabilidadPedidos.Shared.Entities;

namespace TrazabilidadPedidos.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        private readonly AppDbContext _context;

        public InvoicesController(
            IInvoiceService invoiceService,
            AppDbContext context)
        {
            _invoiceService = invoiceService;
            _context = context;
        }

        [HttpGet]
        [Authorize(Policy = "ManageOrders")]
        public async Task<ActionResult<List<Invoice>>> GetAll()
        {
            return Ok(await _invoiceService.GetAllAsync());
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "ViewOwnOrders")]
        public async Task<ActionResult<Invoice>> GetById(int id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);
            if (invoice == null) return NotFound();
            return Ok(invoice);
        }

        [HttpGet("order/{orderId:int}")]
        [Authorize(Policy = "ViewOwnOrders")]
        public async Task<ActionResult<Invoice>> GetByOrderId(int orderId)
        {
            var invoice = await _invoiceService.GetByOrderIdAsync(orderId);
            if (invoice == null) return NotFound();
            return Ok(invoice);
        }

        [HttpGet("my-invoices")]
        [Authorize(Policy = "ViewOwnOrders")]
        public async Task<ActionResult<List<Invoice>>> GetMyInvoices()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (customer == null) return Unauthorized();

            return Ok(await _invoiceService.GetByCustomerIdAsync(customer.Id));
        }

        [HttpPost("generate/{orderId:int}")]
        [Authorize(Policy = "ManageOrders")]
        public async Task<ActionResult<Invoice>> Generate(int orderId)
        {
            var invoice = await _invoiceService.GenerateForOrderAsync(orderId);
            if (invoice == null)
                return BadRequest(new { message = "No se pudo generar la factura." });
            return Ok(invoice);
        }

        [HttpGet("{id:int}/pdf")]
        [Authorize(Policy = "ViewOwnOrders")]
        public async Task<IActionResult> DownloadPdf(int id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);
            if (invoice == null) return NotFound();

            var pdfBytes = _invoiceService.GeneratePdf(invoice);
            return File(pdfBytes, "application/pdf", $"Factura-{invoice.Code}.pdf");
        }

        [HttpGet("order/{orderId:int}/pdf")]
        [Authorize(Policy = "ViewOwnOrders")]
        public async Task<IActionResult> DownloadPdfByOrder(int orderId)
        {
            var invoice = await _invoiceService.GetByOrderIdAsync(orderId);
            if (invoice == null) return NotFound();

            var pdfBytes = _invoiceService.GeneratePdf(invoice);
            return File(pdfBytes, "application/pdf", $"Factura-{invoice.Code}.pdf");
        }

        private int? GetUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(claim, out var userId)) return userId;
            return null;
        }
    }
}
