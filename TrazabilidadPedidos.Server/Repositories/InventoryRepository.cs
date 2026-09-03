using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TrazabilidadPedidos.Server.Data;
using TrazabilidadPedidos.Shared.Entities;

namespace TrazabilidadPedidos.Server.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly AppDbContext _context;

        public InventoryRepository(
            AppDbContext context)
        {
            _context = context;
        }


        // =========================
        // CATEGORÍAS
        // =========================

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(
            int id)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> CategoryNameExistsAsync(
            string name)
        {
            return await _context.Categories
                .AnyAsync(c => c.Name == name);
        }

        public async Task AddCategoryAsync(
            Category category)
        {
            await _context.Categories
                .AddAsync(category);
        }


        // =========================
        // PRODUCTOS
        // =========================

        public async Task<List<Product>> GetProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(
            int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> ProductCodeExistsAsync(
            string code,
            int? excludeProductId = null)
        {
            return await _context.Products
                .AnyAsync(p =>
                    p.Code == code &&
                    (!excludeProductId.HasValue ||
                     p.Id != excludeProductId.Value));
        }

        public async Task AddProductAsync(
            Product product)
        {
            await _context.Products
                .AddAsync(product);
        }


        // =========================
        // MOVIMIENTOS
        // =========================

        public async Task<List<InventoryMovement>>
            GetMovementsAsync()
        {
            return await _context.InventoryMovements
                .Include(m => m.Product)
                .Include(m => m.User)
                .OrderByDescending(m => m.MovementDate)
                .ToListAsync();
        }

        public async Task AddMovementAsync(
            InventoryMovement movement)
        {
            await _context.InventoryMovements
                .AddAsync(movement);
        }


        // =========================
        // TRANSACCIONES
        // =========================

        public async Task<IDbContextTransaction>
            BeginTransactionAsync()
        {
            return await _context.Database
                .BeginTransactionAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}