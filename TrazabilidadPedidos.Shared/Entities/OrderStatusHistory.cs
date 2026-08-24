namespace TrazabilidadPedidos.Shared.Entities
{
    public class OrderStatusHistory
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int OrderStatusId { get; set; }

        public int UserId { get; set; }

        public DateTime StatusDate { get; set; } = DateTime.Now;

        public string? Observation { get; set; }

        public Order? Order { get; set; }

        public OrderStatus? OrderStatus { get; set; }

        public User? User { get; set; }
    }
}