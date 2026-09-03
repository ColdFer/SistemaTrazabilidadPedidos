using Microsoft.EntityFrameworkCore.Storage;
using TrazabilidadPedidos.Shared.Entities;

namespace TrazabilidadPedidos.Server.Repositories
{
    public interface IInventoryRepository
    {
        // CATEGORÍAS
        Task<List<Category>> GetCategoriesAsync();

        Task<Category?> GetCategoryByIdAsync(int id);

        Task<bool> CategoryNameExistsAsync(string name);

        Task AddCategoryAsync(Category category);


        // PRODUCTOS
        Task<List<Product>> GetProductsAsync();

        Task<Product?> GetProductByIdAsync(int id);

        Task<bool> ProductCodeExistsAsync(
            string code,
            int? excludeProductId = null);

        Task AddProductAsync(Product product);


        // MOVIMIENTOS
        Task<List<InventoryMovement>> GetMovementsAsync();

        Task AddMovementAsync(
            InventoryMovement movement);


        // TRANSACCIONES Y GUARDADO
        Task<IDbContextTransaction> BeginTransactionAsync();

        Task SaveChangesAsync();
    }
}