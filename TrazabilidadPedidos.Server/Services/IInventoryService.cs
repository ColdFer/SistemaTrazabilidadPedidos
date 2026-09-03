using TrazabilidadPedidos.Shared.DTOs.Inventory;

namespace TrazabilidadPedidos.Server.Services
{
    public interface IInventoryService
    {
        // Categorías
        Task<List<CategoryDto>> GetCategoriesAsync();
        Task<CategoryDto?> CreateCategoryAsync(CreateCategoryRequest request);

        // Productos
        Task<List<ProductDto>> GetProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<ProductDto?> CreateProductAsync(CreateProductRequest request);
        Task<ProductDto?> UpdateProductAsync(int id, UpdateProductRequest request);
        Task<bool> DeactivateProductAsync(int id);

        // Movimientos
        Task<List<InventoryMovementDto>> GetMovementsAsync();

        Task<InventoryMovementDto?> CreateMovementAsync(
            CreateInventoryMovementRequest request,
            int userId);
    }
}