using System.ComponentModel.DataAnnotations;
using TrazabilidadPedidos.Shared.Entities;
using TrazabilidadPedidos.Shared.Enums;

namespace TrazabilidadPedidos.Shared.DTOs.Inventory
{
    public class CreateInventoryMovementRequest
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public MovementType Type { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [MaxLength(250)]
        public string Reason { get; set; } = string.Empty;
    }
}