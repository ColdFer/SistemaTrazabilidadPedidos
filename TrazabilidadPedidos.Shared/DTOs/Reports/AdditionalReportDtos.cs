namespace TrazabilidadPedidos.Shared.DTOs.Reports
{
    public class SalesByPeriodReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageTicket { get; set; }
        public List<SalesByMonthDto> SalesByDay { get; set; } = new();
    }

    public class TopProductsReportDto
    {
        public int Limit { get; set; }
        public List<TopProductDto> Products { get; set; } = new();
    }

    public class OrdersByStatusReportDto
    {
        public List<OrdersByStatusItemDto> Items { get; set; } = new();
        public int Total { get; set; }
    }

    public class OrdersByStatusItemDto
    {
        public string StatusName { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    public class TopCustomersReportDto
    {
        public List<TopCustomerItemDto> Customers { get; set; } = new();
    }

    public class TopCustomerItemDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
    }

    public class DriverPerformanceReportDto
    {
        public List<DriverPerformanceItemDto> Drivers { get; set; } = new();
    }

    public class DriverPerformanceItemDto
    {
        public string DriverName { get; set; } = string.Empty;
        public int TotalDeliveries { get; set; }
        public int Completed { get; set; }
        public int Failed { get; set; }
        public decimal SuccessRate { get; set; }
    }

    public class InventoryReportDto
    {
        public List<InventoryItemDto> Items { get; set; } = new();
        public int TotalProducts { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
    }

    public class InventoryItemDto
    {
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int CurrentStock { get; set; }
        public int MinStock { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
