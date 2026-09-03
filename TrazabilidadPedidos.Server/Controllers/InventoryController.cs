using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrazabilidadPedidos.Server.Services;
using TrazabilidadPedidos.Shared.DTOs.Inventory;

namespace TrazabilidadPedidos.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(
            IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }


        // =====================================================
        // CATEGORÍAS
        // =====================================================

        // GET: api/Inventory/categories
        [HttpGet("categories")]
        public async Task<ActionResult<List<CategoryDto>>>
            GetCategories()
        {
            var categories =
                await _inventoryService.GetCategoriesAsync();

            return Ok(categories);
        }


        // POST: api/Inventory/categories
        [HttpPost("categories")]
        [Authorize(Policy = "ManageCatalog")]
        public async Task<ActionResult<CategoryDto>>
            CreateCategory(
                CreateCategoryRequest request)
        {
            var category =
                await _inventoryService
                    .CreateCategoryAsync(request);

            if (category == null)
            {
                return BadRequest(new
                {
                    message =
                        "No se pudo registrar la categoría. Verifique que el nombre no esté registrado."
                });
            }

            return Ok(category);
        }


        // =====================================================
        // PRODUCTOS
        // =====================================================

        // GET: api/Inventory/products
        [HttpGet("products")]
        public async Task<ActionResult<List<ProductDto>>>
            GetProducts()
        {
            var products =
                await _inventoryService.GetProductsAsync();

            return Ok(products);
        }


        // GET: api/Inventory/products/5
        [HttpGet("products/{id:int}")]
        public async Task<ActionResult<ProductDto>>
            GetProductById(int id)
        {
            var product =
                await _inventoryService
                    .GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound(new
                {
                    message = "Producto no encontrado."
                });
            }

            return Ok(product);
        }


        // POST: api/Inventory/products
        [HttpPost("products")]
        [Authorize(Policy = "ManageInventory")]
        public async Task<ActionResult<ProductDto>>
            CreateProduct(
                CreateProductRequest request)
        {
            var product =
                await _inventoryService
                    .CreateProductAsync(request);

            if (product == null)
            {
                return BadRequest(new
                {
                    message =
                        "No se pudo registrar el producto. Verifique la categoría y que el código no esté registrado."
                });
            }

            return Ok(product);
        }


        // PUT: api/Inventory/products/5
        [HttpPut("products/{id:int}")]
        [Authorize(Policy = "ManageInventory")]
        public async Task<ActionResult<ProductDto>>
            UpdateProduct(
                int id,
                UpdateProductRequest request)
        {
            var existing =
                await _inventoryService
                    .GetProductByIdAsync(id);

            if (existing == null)
            {
                return NotFound(new
                {
                    message = "Producto no encontrado."
                });
            }

            var product =
                await _inventoryService
                    .UpdateProductAsync(
                        id,
                        request);

            if (product == null)
            {
                return BadRequest(new
                {
                    message =
                        "No se pudo actualizar el producto. Verifique la categoría y el código."
                });
            }

            return Ok(product);
        }


        // DELETE: api/Inventory/products/5
        [HttpDelete("products/{id:int}")]
        [Authorize(Policy = "ManageInventory")]
        public async Task<IActionResult>
            DeactivateProduct(int id)
        {
            var result =
                await _inventoryService
                    .DeactivateProductAsync(id);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Producto no encontrado."
                });
            }

            return NoContent();
        }


        // =====================================================
        // MOVIMIENTOS
        // =====================================================

        // GET: api/Inventory/movements
        [HttpGet("movements")]
        public async Task<
            ActionResult<List<InventoryMovementDto>>>
            GetMovements()
        {
            var movements =
                await _inventoryService
                    .GetMovementsAsync();

            return Ok(movements);
        }


        // POST: api/Inventory/movements
        [HttpPost("movements")]
        [Authorize(Policy = "ManageInventory")]
        public async Task<ActionResult<InventoryMovementDto>>
            CreateMovement(
                CreateInventoryMovementRequest request)
        {
            var userIdValue =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            if (!int.TryParse(
                    userIdValue,
                    out var userId))
            {
                return Unauthorized(new
                {
                    message =
                        "No se pudo identificar al usuario autenticado."
                });
            }

            try
            {
                var movement =
                    await _inventoryService
                        .CreateMovementAsync(
                            request,
                            userId);

                return Ok(movement);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}