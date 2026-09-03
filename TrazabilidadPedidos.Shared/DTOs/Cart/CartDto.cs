namespace TrazabilidadPedidos.Shared.DTOs.Cart
{
    public class CartDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public List<CartItemDto> Items { get; set; } = new();
        public decimal Total { get; set; }
        public int TotalItems { get; set; }
    }
}
