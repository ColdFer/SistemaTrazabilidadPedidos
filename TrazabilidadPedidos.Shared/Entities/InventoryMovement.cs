using TrazabilidadPedidos.Shared.Enums;

namespace TrazabilidadPedidos.Shared.Entities
{
    public class InventoryMovement
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int UserId { get; set; }

        public MovementType Type { get; set; }

        public int Quantity { get; set; }

        public string Reason { get; set; } = string.Empty;

        public DateTime MovementDate { get; set; } = DateTime.Now;

        public Product? Product { get; set; }

        public User? User { get; set; }
    }
}