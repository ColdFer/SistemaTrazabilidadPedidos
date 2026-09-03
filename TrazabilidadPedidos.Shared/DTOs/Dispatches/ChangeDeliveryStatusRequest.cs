using System.ComponentModel.DataAnnotations;

namespace TrazabilidadPedidos.Shared.DTOs.Dispatches
{
    public class ChangeDeliveryStatusRequest
    {
        [Range(1, 7)]
        public int Status { get; set; }

        [MaxLength(500)]
        public string? Observation { get; set; }
    }
}