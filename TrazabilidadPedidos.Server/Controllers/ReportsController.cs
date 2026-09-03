using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrazabilidadPedidos.Server.Services;
using TrazabilidadPedidos.Shared.DTOs.Reports;

namespace TrazabilidadPedidos.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "GenerateReports")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardReportDto>> GetDashboardReport()
        {
            var report = await _reportService.GetDashboardReportAsync();
            return Ok(report);
        }

        [HttpGet("sales-by-period")]
        public async Task<ActionResult<SalesByPeriodReportDto>> GetSalesByPeriod(
            [FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var report = await _reportService.GetSalesByPeriodAsync(start, end);
            return Ok(report);
        }

        [HttpGet("top-products")]
        public async Task<ActionResult<TopProductsReportDto>> GetTopProducts(
            [FromQuery] int limit = 10)
        {
            var report = await _reportService.GetTopProductsAsync(limit);
            return Ok(report);
        }

        [HttpGet("orders-by-status")]
        public async Task<ActionResult<OrdersByStatusReportDto>> GetOrdersByStatus()
        {
            var report = await _reportService.GetOrdersByStatusAsync();
            return Ok(report);
        }

        [HttpGet("top-customers")]
        public async Task<ActionResult<TopCustomersReportDto>> GetTopCustomers(
            [FromQuery] int limit = 10)
        {
            var report = await _reportService.GetTopCustomersAsync(limit);
            return Ok(report);
        }

        [HttpGet("driver-performance")]
        public async Task<ActionResult<DriverPerformanceReportDto>> GetDriverPerformance()
        {
            var report = await _reportService.GetDriverPerformanceAsync();
            return Ok(report);
        }

        [HttpGet("inventory")]
        public async Task<ActionResult<InventoryReportDto>> GetInventoryReport()
        {
            var report = await _reportService.GetInventoryReportAsync();
            return Ok(report);
        }
    }
}
