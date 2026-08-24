namespace TrazabilidadPedidos.Shared.Entities
{
    public class DeliveryDriver
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Phone { get; set; } = string.Empty;

        public bool IsAvailable { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public User? User { get; set; }
    }
}