using Microsoft.EntityFrameworkCore;
using TrazabilidadPedidos.Server.Data;
using TrazabilidadPedidos.Shared.DTOs.Reports;
using TrazabilidadPedidos.Shared.Enums;

namespace TrazabilidadPedidos.Server.Services
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardReportDto> GetDashboardReportAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.Product)
                .ToListAsync();

            var products = await _context.Products.ToListAsync();
            var customers = await _context.Customers.ToListAsync();

            var deliveredStatusId = await _context.OrderStatuses
                .Where(s => s.Name == "Entregado")
                .Select(s => s.Id)
                .FirstOrDefaultAsync();

            var pendingStatusId = await _context.OrderStatuses
                .Where(s => s.Name == "Pendiente")
                .Select(s => s.Id)
                .FirstOrDefaultAsync();

            var totalOrders = orders.Count;
            var deliveredOrders = orders.Count(o => o.CurrentStatusId == deliveredStatusId);
            var pendingOrders = orders.Count(o => o.CurrentStatusId == pendingStatusId);
            var totalSales = orders
                .Where(o => o.CurrentStatusId == deliveredStatusId)
                .Sum(o => o.Total);

            var salesByMonth = orders
                .Where(o => o.CurrentStatusId == deliveredStatusId)
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new SalesByMonthDto
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Total = g.Sum(o => o.Total),
                    OrderCount = g.Count()
                })
                .OrderBy(s => s.Month)
                .Take(12)
                .ToList();

            var topProducts = orders
                .SelectMany(o => o.OrderDetails)
                .GroupBy(d => d.Product!.Name)
                .Select(g => new TopProductDto
                {
                    ProductName = g.Key,
                    TotalSold = g.Sum(d => d.Quantity),
                    Revenue = g.Sum(d => d.Quantity * d.UnitPrice)
                })
                .OrderByDescending(p => p.TotalSold)
                .Take(10)
                .ToList();

            var lowStockItems = products
                .Where(p => p.CurrentStock <= 5 && p.IsActive)
                .Select(p => new LowStockProductDto
                {
                    ProductName = p.Name,
                    ProductCode = p.Code,
                    CurrentStock = p.CurrentStock,
                    CategoryName = ""
                })
                .ToList();

            return new DashboardReportDto
            {
                TotalOrders = totalOrders,
                PendingOrders = pendingOrders,
                DeliveredOrders = deliveredOrders,
                TotalProducts = products.Count(p => p.IsActive),
                LowStockProducts = lowStockItems.Count,
                TotalCustomers = customers.Count,
                TotalSales = totalSales,
                SalesByMonth = salesByMonth,
                TopProducts = topProducts,
                LowStockItems = lowStockItems
            };
        }

        public async Task<SalesByPeriodReportDto> GetSalesByPeriodAsync(DateTime start, DateTime end)
        {
            var deliveredStatusId = await _context.OrderStatuses
                .Where(s => s.Name == "Entregado")
                .Select(s => s.Id)
                .FirstOrDefaultAsync();

            var orders = await _context.Orders
                .Where(o => o.CurrentStatusId == deliveredStatusId
                    && o.OrderDate >= start && o.OrderDate <= end)
                .ToListAsync();

            var salesByDay = orders
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new SalesByMonthDto
                {
                    Month = g.Key.ToString("dd/MM/yyyy"),
                    Total = g.Sum(o => o.Total),
                    OrderCount = g.Count()
                })
                .OrderBy(s => s.Month)
                .ToList();

            return new SalesByPeriodReportDto
            {
                StartDate = start,
                EndDate = end,
                TotalSales = orders.Sum(o => o.Total),
                TotalOrders = orders.Count,
                AverageTicket = orders.Any() ? orders.Average(o => o.Total) : 0,
                SalesByDay = salesByDay
            };
        }

        public async Task<TopProductsReportDto> GetTopProductsAsync(int limit)
        {
            var products = await _context.OrderDetails
                .Include(d => d.Product)
                .GroupBy(d => new { d.ProductId, d.Product!.Name })
                .Select(g => new TopProductDto
                {
                    ProductName = g.Key.Name,
                    TotalSold = g.Sum(d => d.Quantity),
                    Revenue = g.Sum(d => d.Quantity * d.UnitPrice)
                })
                .OrderByDescending(p => p.TotalSold)
                .Take(limit)
                .ToListAsync();

            return new TopProductsReportDto
            {
                Limit = limit,
                Products = products
            };
        }

        public async Task<OrdersByStatusReportDto> GetOrdersByStatusAsync()
        {
            var total = await _context.Orders.CountAsync();

            var items = await _context.OrderStatusHistories
                .Include(h => h.OrderStatus)
                .GroupBy(h => h.OrderStatus!.Name)
                .Select(g => new OrdersByStatusItemDto
                {
                    StatusName = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            foreach (var item in items)
            {
                item.Percentage = total > 0
                    ? Math.Round((decimal)item.Count / total * 100, 1)
                    : 0;
            }

            return new OrdersByStatusReportDto
            {
                Items = items,
                Total = total
            };
        }

        public async Task<TopCustomersReportDto> GetTopCustomersAsync(int limit)
        {
            var customers = await _context.Customers
                .Include(c => c.User)
                .GroupJoin(
                    _context.Orders,
                    c => c.Id,
                    o => o.CustomerId,
                    (c, orders) => new { Customer = c, Orders = orders })
                .Select(x => new TopCustomerItemDto
                {
                    CustomerName = x.Customer.User != null
                        ? $"{x.Customer.User.FirstName} {x.Customer.User.LastName}"
                        : "Sin nombre",
                    TotalOrders = x.Orders.Count(),
                    TotalSpent = x.Orders.Sum(o => o.Total)
                })
                .OrderByDescending(c => c.TotalSpent)
                .Take(limit)
                .ToListAsync();

            return new TopCustomersReportDto { Customers = customers };
        }

        public async Task<DriverPerformanceReportDto> GetDriverPerformanceAsync()
        {
            var drivers = await _context.DeliveryDrivers
                .Include(d => d.User)
                .GroupJoin(
                    _context.Deliveries,
                    d => d.Id,
                    del => del.DeliveryDriverId,
                    (d, dels) => new { Driver = d, Deliveries = dels })
                .Select(x => new DriverPerformanceItemDto
                {
                    DriverName = x.Driver.User != null
                        ? $"{x.Driver.User.FirstName} {x.Driver.User.LastName}"
                        : "Sin nombre",
                    TotalDeliveries = x.Deliveries.Count(),
                    Completed = x.Deliveries.Count(del => del.Status == Shared.Enums.DeliveryStatus.Delivered),
                    Failed = x.Deliveries.Count(del => del.Status == Shared.Enums.DeliveryStatus.Failed)
                })
                .ToListAsync();

            foreach (var d in drivers)
            {
                d.SuccessRate = d.TotalDeliveries > 0
                    ? Math.Round((decimal)d.Completed / d.TotalDeliveries * 100, 1)
                    : 0;
            }

            return new DriverPerformanceReportDto
            {
                Drivers = drivers.OrderByDescending(d => d.SuccessRate).ToList()
            };
        }

        public async Task<InventoryReportDto> GetInventoryReportAsync()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .Select(p => new InventoryItemDto
                {
                    ProductName = p.Name,
                    ProductCode = p.Code,
                    CategoryName = p.Category != null ? p.Category.Name : "",
                    CurrentStock = p.CurrentStock,
                    MinStock = 5,
                    Status = p.CurrentStock == 0 ? "Agotado"
                           : p.CurrentStock <= 5 ? "Bajo"
                           : "Normal"
                })
                .OrderBy(p => p.CurrentStock)
                .ToListAsync();

            return new InventoryReportDto
            {
                Items = products,
                TotalProducts = products.Count,
                LowStockCount = products.Count(p => p.Status == "Bajo"),
                OutOfStockCount = products.Count(p => p.Status == "Agotado")
            };
        }
    }
}
