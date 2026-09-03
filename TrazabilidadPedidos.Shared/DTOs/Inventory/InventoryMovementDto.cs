using TrazabilidadPedidos.Shared.Entities;
using TrazabilidadPedidos.Shared.Enums;

namespace TrazabilidadPedidos.Shared.DTOs.Inventory
{
    public class InventoryMovementDto
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string ProductCode { get; set; } = string.Empty;

        public int UserId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public MovementType Type { get; set; }

        public int Quantity { get; set; }

        public string Reason { get; set; } = string.Empty;

        public DateTime MovementDate { get; set; }
    }
}