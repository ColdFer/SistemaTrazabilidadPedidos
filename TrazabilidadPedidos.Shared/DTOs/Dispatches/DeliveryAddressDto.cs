namespace TrazabilidadPedidos.Shared.DTOs.Dispatches
{
    public class DeliveryAddressDto
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public string AddressLine { get; set; } = string.Empty;

        public string? Reference { get; set; }

        public string? Label { get; set; }
    }
}