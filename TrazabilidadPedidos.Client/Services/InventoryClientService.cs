using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.JSInterop;
using TrazabilidadPedidos.Shared.DTOs.Inventory;

namespace TrazabilidadPedidos.Client.Services
{
    public class InventoryClientService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _js;

        public InventoryClientService(
            HttpClient httpClient,
            IJSRuntime js)
        {
            _httpClient = httpClient;
            _js = js;
        }


        // =====================================================
        // CATEGORÍAS
        // =====================================================

        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                "api/Inventory/categories");

            await AddAuthorizationAsync(request);

            var response = await _httpClient.SendAsync(request);

            CheckUnauthorized(response);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<List<CategoryDto>>()
                ?? new List<CategoryDto>();
        }


        public async Task<CategoryDto?> CreateCategoryAsync(
            CreateCategoryRequest category)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "api/Inventory/categories");

            await AddAuthorizationAsync(request);

            request.Content = JsonContent.Create(category);

            var response = await _httpClient.SendAsync(request);

            CheckUnauthorized(response);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<CategoryDto>();
        }


        // =====================================================
        // PRODUCTOS
        // =====================================================

        public async Task<List<ProductDto>> GetProductsAsync()
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                "api/Inventory/products");

            await AddAuthorizationAsync(request);

            var response = await _httpClient.SendAsync(request);

            CheckUnauthorized(response);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<List<ProductDto>>()
                ?? new List<ProductDto>();
        }


        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"api/Inventory/products/{id}");

            await AddAuthorizationAsync(request);

            var response = await _httpClient.SendAsync(request);

            CheckUnauthorized(response);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<ProductDto>();
        }


        public async Task<ProductDto?> CreateProductAsync(
            CreateProductRequest product)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "api/Inventory/products");

            await AddAuthorizationAsync(request);

            request.Content = JsonContent.Create(product);

            var response = await _httpClient.SendAsync(request);

            CheckUnauthorized(response);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<ProductDto>();
        }


        public async Task<ProductDto?> UpdateProductAsync(
            int id,
            UpdateProductRequest product)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Put,
                $"api/Inventory/products/{id}");

            await AddAuthorizationAsync(request);

            request.Content = JsonContent.Create(product);

            var response = await _httpClient.SendAsync(request);

            CheckUnauthorized(response);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<ProductDto>();
        }


        public async Task<bool> DeactivateProductAsync(int id)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Delete,
                $"api/Inventory/products/{id}");

            await AddAuthorizationAsync(request);

            var response = await _httpClient.SendAsync(request);

            CheckUnauthorized(response);

            return response.IsSuccessStatusCode;
        }


        // =====================================================
        // MOVIMIENTOS DE INVENTARIO
        // =====================================================

        public async Task<List<InventoryMovementDto>>
            GetMovementsAsync()
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                "api/Inventory/movements");

            await AddAuthorizationAsync(request);

            var response = await _httpClient.SendAsync(request);

            CheckUnauthorized(response);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<List<InventoryMovementDto>>()
                ?? new List<InventoryMovementDto>();
        }


        public async Task<InventoryMovementDto?> CreateMovementAsync(
            CreateInventoryMovementRequest movement)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "api/Inventory/movements");

            await AddAuthorizationAsync(request);

            request.Content = JsonContent.Create(movement);

            var response = await _httpClient.SendAsync(request);

            CheckUnauthorized(response);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<InventoryMovementDto>();
        }


        // =====================================================
        // JWT
        // =====================================================

        private async Task AddAuthorizationAsync(
            HttpRequestMessage request)
        {
            var token = await _js.InvokeAsync<string>(
                "localStorage.getItem",
                "authToken");

            if (string.IsNullOrWhiteSpace(token))
                throw new UnauthorizedAccessException();

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token);
        }


        private static void CheckUnauthorized(
            HttpResponseMessage response)
        {
            if (response.StatusCode ==
                HttpStatusCode.Unauthorized ||
                response.StatusCode ==
                HttpStatusCode.Forbidden)
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}