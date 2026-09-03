using System.ComponentModel.DataAnnotations;

namespace TrazabilidadPedidos.Shared.DTOs.Inventory
{
    public class UpdateProductRequest
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        public string? Image { get; set; }

        public bool IsActive { get; set; }
    }
}