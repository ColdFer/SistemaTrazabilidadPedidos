using System.ComponentModel.DataAnnotations;

namespace TrazabilidadPedidos.Shared.DTOs.Payments
{
    public class VerifyPaymentRequest
    {
        [Required]
        public int Status { get; set; }

        public string? Observation { get; set; }
    }
}
