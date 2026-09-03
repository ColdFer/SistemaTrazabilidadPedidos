using System.ComponentModel.DataAnnotations;

namespace TrazabilidadPedidos.Shared.DTOs.Dispatches
{
    public class UpdateDeliveryRequest
    {
        [Required]
        public int AddressId { get; set; }

        public int? DeliveryDriverId { get; set; }

        [Required]
        public DateTime ScheduledDate { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        [Required]
        [MaxLength(30)]
        public string ContactPhone { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string RecipientName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Observation { get; set; }
    }
}