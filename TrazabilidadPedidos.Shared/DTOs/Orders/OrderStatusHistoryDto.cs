namespace TrazabilidadPedidos.Shared.DTOs.Orders
{
    public class OrderStatusHistoryDto
    {
        public int Id { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public DateTime StatusDate { get; set; }
        public string? Observation { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
}
