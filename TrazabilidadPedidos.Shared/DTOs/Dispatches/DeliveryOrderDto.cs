namespace TrazabilidadPedidos.Shared.DTOs.Dispatches
{
    public class DeliveryOrderDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? DeliveryReference { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerFullName { get; set; }
        public double? DeliveryLatitude { get; set; }
        public double? DeliveryLongitude { get; set; }
    }
}
