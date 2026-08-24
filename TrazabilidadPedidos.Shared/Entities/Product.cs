namespace TrazabilidadPedidos.Shared.Entities
{
    public class Product
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }

        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int CurrentStock { get; set; }

        public string? Image { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public Category? Category { get; set; }
    }
}