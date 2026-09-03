using System.ComponentModel.DataAnnotations;

namespace TrazabilidadPedidos.Shared.DTOs.Cart
{
    public class AddToCartRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Producto requerido.")]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Cantidad debe ser entre 1 y 100.")]
        public int Quantity { get; set; } = 1;
    }
}
