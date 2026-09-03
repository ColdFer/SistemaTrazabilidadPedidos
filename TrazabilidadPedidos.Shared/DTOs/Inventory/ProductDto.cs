namespace TrazabilidadPedidos.Shared.DTOs.Inventory
{
    public class ProductDto
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int CurrentStock { get; set; }

        public string? Image { get; set; }

        public bool IsActive { get; set; }
    }
}