using TrazabilidadPedidos.Server.Repositories;
using TrazabilidadPedidos.Shared.DTOs.Inventory;
using TrazabilidadPedidos.Shared.Entities;
using TrazabilidadPedidos.Shared.Enums;

namespace TrazabilidadPedidos.Server.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;

        public InventoryService(
            IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }


        // =====================================================
        // CATEGORÍAS
        // =====================================================

        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            var categories =
                await _inventoryRepository.GetCategoriesAsync();

            return categories
                .Select(MapCategoryToDto)
                .ToList();
        }


        public async Task<CategoryDto?> CreateCategoryAsync(
            CreateCategoryRequest request)
        {
            var name = request.Name.Trim();

            if (await _inventoryRepository
                .CategoryNameExistsAsync(name))
            {
                return null;
            }

            var now = DateTime.Now;

            var category = new Category
            {
                Name = name,
                Description = request.Description?.Trim(),
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            };

            await _inventoryRepository
                .AddCategoryAsync(category);

            await _inventoryRepository
                .SaveChangesAsync();

            return MapCategoryToDto(category);
        }


        // =====================================================
        // PRODUCTOS
        // =====================================================

        public async Task<List<ProductDto>> GetProductsAsync()
        {
            var products =
                await _inventoryRepository.GetProductsAsync();

            return products
                .Select(MapProductToDto)
                .ToList();
        }


        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product =
                await _inventoryRepository.GetProductByIdAsync(id);

            if (product == null)
                return null;

            return MapProductToDto(product);
        }


        public async Task<ProductDto?> CreateProductAsync(
            CreateProductRequest request)
        {
            var category =
                await _inventoryRepository
                    .GetCategoryByIdAsync(request.CategoryId);

            if (category == null || !category.IsActive)
                return null;

            var code = request.Code.Trim();

            if (await _inventoryRepository
                .ProductCodeExistsAsync(code))
            {
                return null;
            }

            var now = DateTime.Now;

            var product = new Product
            {
                CategoryId = category.Id,
                Category = category,

                Code = code,
                Name = request.Name.Trim(),
                Description = request.Description?.Trim(),

                Price = request.Price,

                // El stock NO se registra manualmente.
                // Debe cambiar mediante movimientos.
                CurrentStock = 0,

                Image = request.Image?.Trim(),
                IsActive = true,

                CreatedAt = now,
                UpdatedAt = now
            };

            await _inventoryRepository
                .AddProductAsync(product);

            await _inventoryRepository
                .SaveChangesAsync();

            return MapProductToDto(product);
        }


        public async Task<ProductDto?> UpdateProductAsync(
            int id,
            UpdateProductRequest request)
        {
            var product =
                await _inventoryRepository.GetProductByIdAsync(id);

            if (product == null)
                return null;

            var category =
                await _inventoryRepository
                    .GetCategoryByIdAsync(request.CategoryId);

            if (category == null || !category.IsActive)
                return null;

            var code = request.Code.Trim();

            if (await _inventoryRepository.ProductCodeExistsAsync(
                code,
                product.Id))
            {
                return null;
            }

            product.CategoryId = category.Id;
            product.Category = category;

            product.Code = code;
            product.Name = request.Name.Trim();
            product.Description = request.Description?.Trim();

            product.Price = request.Price;
            product.Image = request.Image?.Trim();
            product.IsActive = request.IsActive;

            product.UpdatedAt = DateTime.Now;

            // IMPORTANTE:
            // CurrentStock no se modifica aquí.

            await _inventoryRepository.SaveChangesAsync();

            return MapProductToDto(product);
        }


        public async Task<bool> DeactivateProductAsync(int id)
        {
            var product =
                await _inventoryRepository.GetProductByIdAsync(id);

            if (product == null)
                return false;

            product.IsActive = false;
            product.UpdatedAt = DateTime.Now;

            await _inventoryRepository.SaveChangesAsync();

            return true;
        }


        // =====================================================
        // MOVIMIENTOS DE INVENTARIO
        // =====================================================

        public async Task<List<InventoryMovementDto>>
            GetMovementsAsync()
        {
            var movements =
                await _inventoryRepository.GetMovementsAsync();

            return movements
                .Select(MapMovementToDto)
                .ToList();
        }


        public async Task<InventoryMovementDto?>
            CreateMovementAsync(
                CreateInventoryMovementRequest request,
                int userId)
        {
            var product =
                await _inventoryRepository
                    .GetProductByIdAsync(request.ProductId);

            if (product == null || !product.IsActive)
            {
                throw new InvalidOperationException(
                    "El producto no existe o está inactivo.");
            }

            if (request.Quantity <= 0)
            {
                throw new InvalidOperationException(
                    "La cantidad debe ser mayor a cero.");
            }

            /*
             * IMPORTANTE:
             *
             * Entry:
             * suma unidades al stock.
             *
             * Exit:
             * resta unidades del stock.
             *
             * Adjustment:
             * establece el stock físico real indicado
             * en Quantity.
             */

            switch (request.Type)
            {
                case MovementType.Entry:
                    break;

                case MovementType.Exit:

                    if (product.CurrentStock < request.Quantity)
                    {
                        throw new InvalidOperationException(
                            "Stock insuficiente para realizar la salida.");
                    }

                    break;

                case MovementType.Adjustment:
                    break;

                default:
                    throw new InvalidOperationException(
                        "Tipo de movimiento no válido.");
            }


            // ================================================
            // TRANSACCIÓN SEGURA
            // ================================================

            await using var transaction =
                await _inventoryRepository
                    .BeginTransactionAsync();

            try
            {
                switch (request.Type)
                {
                    case MovementType.Entry:

                        product.CurrentStock += request.Quantity;

                        break;


                    case MovementType.Exit:

                        product.CurrentStock -= request.Quantity;

                        break;


                    case MovementType.Adjustment:

                        product.CurrentStock = request.Quantity;

                        break;
                }


                product.UpdatedAt = DateTime.Now;


                var movement = new InventoryMovement
                {
                    ProductId = product.Id,
                    Product = product,

                    UserId = userId,

                    Type = request.Type,

                    Quantity = request.Quantity,

                    Reason = request.Reason.Trim(),

                    MovementDate = DateTime.Now
                };


                await _inventoryRepository
                    .AddMovementAsync(movement);


                await _inventoryRepository
                    .SaveChangesAsync();


                await transaction.CommitAsync();


                return new InventoryMovementDto
                {
                    Id = movement.Id,

                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductCode = product.Code,

                    UserId = userId,

                    Type = movement.Type,
                    Quantity = movement.Quantity,
                    Reason = movement.Reason,

                    MovementDate = movement.MovementDate
                };
            }
            catch
            {
                await transaction.RollbackAsync();

                throw;
            }
        }


        // =====================================================
        // MAPEOS
        // =====================================================

        private static CategoryDto MapCategoryToDto(
            Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive
            };
        }


        private static ProductDto MapProductToDto(
            Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                CategoryId = product.CategoryId,

                CategoryName =
                    product.Category?.Name
                    ?? string.Empty,

                Code = product.Code,
                Name = product.Name,
                Description = product.Description,

                Price = product.Price,
                CurrentStock = product.CurrentStock,

                Image = product.Image,
                IsActive = product.IsActive
            };
        }


        private static InventoryMovementDto MapMovementToDto(
            InventoryMovement movement)
        {
            return new InventoryMovementDto
            {
                Id = movement.Id,

                ProductId = movement.ProductId,

                ProductName =
                    movement.Product?.Name
                    ?? string.Empty,

                ProductCode =
                    movement.Product?.Code
                    ?? string.Empty,

                UserId = movement.UserId,

                UserName = movement.User == null
                    ? string.Empty
                    : $"{movement.User.FirstName} {movement.User.LastName}",

                Type = movement.Type,
                Quantity = movement.Quantity,

                Reason = movement.Reason,

                MovementDate = movement.MovementDate
            };
        }
    }
}