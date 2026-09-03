using System.ComponentModel.DataAnnotations;

namespace TrazabilidadPedidos.Shared.DTOs.Orders
{
    public class CreateFromCartRequest
    {
        public double? DeliveryLatitude { get; set; }

        public double? DeliveryLongitude { get; set; }

        [MaxLength(300)]
        public string? DeliveryAddress { get; set; }

        [MaxLength(300)]
        public string? DeliveryReference { get; set; }
    }
}
