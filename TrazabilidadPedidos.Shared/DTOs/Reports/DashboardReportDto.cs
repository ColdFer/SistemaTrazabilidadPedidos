namespace TrazabilidadPedidos.Shared.DTOs.Reports
{
    public class DashboardReportDto
    {
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int DeliveredOrders { get; set; }
        public int TotalProducts { get; set; }
        public int LowStockProducts { get; set; }
        public int TotalCustomers { get; set; }
        public decimal TotalSales { get; set; }
        public List<SalesByMonthDto> SalesByMonth { get; set; } = new();
        public List<TopProductDto> TopProducts { get; set; } = new();
        public List<LowStockProductDto> LowStockItems { get; set; } = new();
    }

    public class SalesByMonthDto
    {
        public string Month { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public int OrderCount { get; set; }
    }

    public class TopProductDto
    {
        public string ProductName { get; set; } = string.Empty;
        public int TotalSold { get; set; }
        public decimal Revenue { get; set; }
    }

    public class LowStockProductDto
    {
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public int CurrentStock { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
