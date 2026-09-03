namespace TrazabilidadPedidos.Shared.DTOs.Dispatches
{
    public class DeliveryDriverDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public bool IsAvailable { get; set; }
    }
}