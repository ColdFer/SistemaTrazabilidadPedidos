using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.JSInterop;
using TrazabilidadPedidos.Shared.DTOs.Cart;

namespace TrazabilidadPedidos.Client.Services
{
    public class CartClientService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _js;

        public CartClientService(
            HttpClient httpClient,
            IJSRuntime js)
        {
            _httpClient = httpClient;
            _js = js;
        }

        public async Task<CartDto?> GetCartAsync()
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get, "api/Cart");

            await AddAuthorizationAsync(request);
            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);
            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<CartDto>();
        }

        public async Task<CartDto?> AddToCartAsync(AddToCartRequest item)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post, "api/Cart/items");

            await AddAuthorizationAsync(request);
            request.Content = JsonContent.Create(item);

            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<CartDto>();
        }

        public async Task<CartDto?> UpdateQuantityAsync(
            int itemId, UpdateCartItemRequest request)
        {
            using var httpRequest = new HttpRequestMessage(
                HttpMethod.Put, $"api/Cart/items/{itemId}");

            await AddAuthorizationAsync(httpRequest);
            httpRequest.Content = JsonContent.Create(request);

            var response = await _httpClient.SendAsync(httpRequest);
            CheckUnauthorized(response);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<CartDto>();
        }

        public async Task<CartDto?> RemoveFromCartAsync(int itemId)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Delete, $"api/Cart/items/{itemId}");

            await AddAuthorizationAsync(request);
            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<CartDto>();
        }

        public async Task<CartDto?> ClearCartAsync()
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Delete, "api/Cart");

            await AddAuthorizationAsync(request);
            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<CartDto>();
        }

        private async Task AddAuthorizationAsync(
            HttpRequestMessage request)
        {
            var token = await _js.InvokeAsync<string>(
                "localStorage.getItem", "authToken");

            if (string.IsNullOrWhiteSpace(token))
                throw new UnauthorizedAccessException();

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        private static void CheckUnauthorized(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}

