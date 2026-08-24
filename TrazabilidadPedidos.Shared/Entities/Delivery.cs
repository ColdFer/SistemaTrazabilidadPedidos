using TrazabilidadPedidos.Shared.Enums;

namespace TrazabilidadPedidos.Shared.Entities
{
    public class Delivery
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int AddressId { get; set; }

        public int? DeliveryDriverId { get; set; }

        public DateTime ScheduledDate { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public string ContactPhone { get; set; } = string.Empty;

        public string RecipientName { get; set; } = string.Empty;

        public DeliveryStatus Status { get; set; } = DeliveryStatus.Pending;

        public DateTime? DepartureDate { get; set; }

        public DateTime? DeliveredDate { get; set; }

        public string? Observation { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public Order? Order { get; set; }

        public Address? Address { get; set; }

        public DeliveryDriver? DeliveryDriver { get; set; }

        public ICollection<Incident> Incidents { get; set; }
            = new List<Incident>();
    }
}