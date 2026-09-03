namespace TrazabilidadPedidos.Shared.DTOs.Payments
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public string? TransactionCode { get; set; }
        public string Method { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? Receipt { get; set; }
        public string? ProofImage { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Observation { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
