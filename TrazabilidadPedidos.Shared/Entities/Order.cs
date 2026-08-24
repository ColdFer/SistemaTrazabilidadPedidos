namespace TrazabilidadPedidos.Shared.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int CurrentStatusId { get; set; }

        public string Code { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public decimal Total { get; set; }

        public string? Observation { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public Customer? Customer { get; set; }

        public OrderStatus? CurrentStatus { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }
            = new List<OrderDetail>();
    }
}