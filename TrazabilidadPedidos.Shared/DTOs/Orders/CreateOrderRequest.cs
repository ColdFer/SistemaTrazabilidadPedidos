using System.ComponentModel.DataAnnotations;

namespace TrazabilidadPedidos.Shared.DTOs.Orders
{
    public class CreateOrderRequest
    {
        [Required]
        public int CustomerId { get; set; }

        [MaxLength(500)]
        public string? Observation { get; set; }

        public double? DeliveryLatitude { get; set; }

        public double? DeliveryLongitude { get; set; }

        [MaxLength(300)]
        public string? DeliveryAddress { get; set; }

        [MaxLength(300)]
        public string? DeliveryReference { get; set; }

        public List<OrderItemRequest> Items { get; set; } = new();
    }

    public class OrderItemRequest
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
