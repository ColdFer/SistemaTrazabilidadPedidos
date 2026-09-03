namespace TrazabilidadPedidos.Shared.Entities
{
    public class Address
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public string AddressLine { get; set; } = string.Empty;

        public string? Reference { get; set; }

        public string? Label { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public Customer? Customer { get; set; }
    }
}