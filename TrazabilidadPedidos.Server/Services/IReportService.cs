using TrazabilidadPedidos.Shared.DTOs.Reports;

namespace TrazabilidadPedidos.Server.Services
{
    public interface IReportService
    {
        Task<DashboardReportDto> GetDashboardReportAsync();
        Task<SalesByPeriodReportDto> GetSalesByPeriodAsync(DateTime start, DateTime end);
        Task<TopProductsReportDto> GetTopProductsAsync(int limit);
        Task<OrdersByStatusReportDto> GetOrdersByStatusAsync();
        Task<TopCustomersReportDto> GetTopCustomersAsync(int limit);
        Task<DriverPerformanceReportDto> GetDriverPerformanceAsync();
        Task<InventoryReportDto> GetInventoryReportAsync();
    }
}
