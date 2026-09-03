using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.JSInterop;
using TrazabilidadPedidos.Shared.DTOs.Orders;

namespace TrazabilidadPedidos.Client.Services
{
    public class OrderClientService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _js;

        public OrderClientService(
            HttpClient httpClient,
            IJSRuntime js)
        {
            _httpClient = httpClient;
            _js = js;
        }

        public async Task<List<OrderDto>> GetAllAsync()
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get, "api/Orders");

            await AddAuthorizationAsync(request);
            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);
            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<List<OrderDto>>()
                ?? new List<OrderDto>();
        }

        public async Task<List<OrderDto>> GetMyOrdersAsync()
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get, "api/Orders/my-orders");

            await AddAuthorizationAsync(request);
            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);
            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<List<OrderDto>>()
                ?? new List<OrderDto>();
        }

        public async Task<OrderDto?> GetByIdAsync(int id)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get, $"api/Orders/{id}");

            await AddAuthorizationAsync(request);
            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<OrderDto>();
        }

        public async Task<OrderDto?> CreateAsync(CreateOrderRequest order)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post, "api/Orders");

            await AddAuthorizationAsync(request);
            request.Content = JsonContent.Create(order);

            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<OrderDto>();
        }

        public async Task<bool> UpdateStatusAsync(
            int orderId, int statusId, string? observation)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Put, $"api/Orders/{orderId}/status");

            await AddAuthorizationAsync(request);
            request.Content = JsonContent.Create(new
            {
                StatusId = statusId,
                Observation = observation
            });

            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);

            return response.IsSuccessStatusCode;
        }

        public async Task<List<OrderStatusDto>> GetStatusesAsync()
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get, "api/Orders/statuses");

            await AddAuthorizationAsync(request);
            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);
            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<List<OrderStatusDto>>()
                ?? new List<OrderStatusDto>();
        }

        public async Task<OrderDto?> CreateFromCartAsync(CreateFromCartRequest? body = null)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post, "api/Orders/from-cart");

            await AddAuthorizationAsync(request);
            request.Content = JsonContent.Create(body ?? new CreateFromCartRequest());

            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<OrderDto>();
        }

        public async Task<bool> AcceptAsync(int orderId)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post, $"api/Orders/{orderId}/accept");

            await AddAuthorizationAsync(request);

            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);

            return response.IsSuccessStatusCode;
        }

        public async Task<List<OrderStatusHistoryDto>> GetHistoryAsync(int orderId)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get, $"api/Orders/{orderId}/history");

            await AddAuthorizationAsync(request);
            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);
            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<List<OrderStatusHistoryDto>>()
                ?? new List<OrderStatusHistoryDto>();
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

