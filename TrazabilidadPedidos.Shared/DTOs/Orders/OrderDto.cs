namespace TrazabilidadPedidos.Shared.DTOs.Orders
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal Total { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string? Observation { get; set; }
        public double? DeliveryLatitude { get; set; }
        public double? DeliveryLongitude { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? DeliveryReference { get; set; }
        public List<OrderDetailDto> Details { get; set; } = new();
    }
}
