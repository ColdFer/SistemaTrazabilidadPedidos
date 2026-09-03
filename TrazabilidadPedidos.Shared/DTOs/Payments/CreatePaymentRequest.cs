using System.ComponentModel.DataAnnotations;

namespace TrazabilidadPedidos.Shared.DTOs.Payments
{
    public class CreatePaymentRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Pedido requerido.")]
        public int OrderId { get; set; }

        [Required]
        [Range(1, 4, ErrorMessage = "Metodo de pago invalido.")]
        public int Method { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Monto debe ser mayor a 0.")]
        public decimal Amount { get; set; }

        public string? ProofImage { get; set; }
    }
}
