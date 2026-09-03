namespace TrazabilidadPedidos.Shared.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public Customer? Customer { get; set; }
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
