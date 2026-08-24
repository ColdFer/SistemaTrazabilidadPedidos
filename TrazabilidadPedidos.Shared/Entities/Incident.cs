namespace TrazabilidadPedidos.Shared.Entities
{
    public class Incident
    {
        public int Id { get; set; }

        public int DeliveryId { get; set; }

        public int UserId { get; set; }

        public string Type { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime IncidentDate { get; set; } = DateTime.Now;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Delivery? Delivery { get; set; }

        public User? User { get; set; }
    }
}