namespace TrazabilidadPedidos.Shared.DTOs.Dispatches
{
    public class DeliveryDto
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public string OrderCode { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;

        public int? AddressId { get; set; }

        public string Address { get; set; } = string.Empty;

        public int? DeliveryDriverId { get; set; }

        public string DeliveryDriverName { get; set; } = string.Empty;

        public DateTime ScheduledDate { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public string ContactPhone { get; set; } = string.Empty;

        public string RecipientName { get; set; } = string.Empty;

        public int Status { get; set; }

        public string StatusName { get; set; } = string.Empty;

        public DateTime? DepartureDate { get; set; }

        public DateTime? DeliveredDate { get; set; }

        public string? Observation { get; set; }
    }
}