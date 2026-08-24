using TrazabilidadPedidos.Shared.Enums;

namespace TrazabilidadPedidos.Shared.Entities
{
    public class Payment
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int? VerifiedByUserId { get; set; }

        public PaymentMethod Method { get; set; }

        public decimal Amount { get; set; }

        public string? Receipt { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public string? Observation { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public Order? Order { get; set; }

        public User? VerifiedByUser { get; set; }
    }
}