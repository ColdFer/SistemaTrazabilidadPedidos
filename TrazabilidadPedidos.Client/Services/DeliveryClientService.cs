using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.JSInterop;
using TrazabilidadPedidos.Shared.DTOs.Dispatches;

namespace TrazabilidadPedidos.Client.Services
{
    public class DeliveryClientService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _js;

        public DeliveryClientService(
            HttpClient httpClient,
            IJSRuntime js)
        {
            _httpClient = httpClient;
            _js = js;
        }


        // =====================================================
        // DESPACHOS
        // =====================================================

        public async Task<List<DeliveryDto>> GetAllAsync()
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                "api/Deliveries");

            await AddAuthorizationAsync(request);

            var response = await _httpClient.SendAsync(request);

            CheckUnauthorized(response);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<List<DeliveryDto>>()
                ?? new List<DeliveryDto>();
        }

        public async Task<List<DeliveryDto>> GetMyDeliveriesAsync()
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                "api/Deliveries/my-deliveries");

            await AddAuthorizationAsync(request);

            var response = await _httpClient.SendAsync(request);

            CheckUnauthorized(response);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<List<DeliveryDto>>()
                ?? new List<DeliveryDto>();
        }


        public async Task<DeliveryDto?> GetByIdAsync(int id)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"api/Deliveries/{id}");

            await AddAuthorizationAsync(request);

            var response = await _httpClient.SendAsync(request);

            CheckUnauthorized(response);

            if (response.StatusCode ==
                HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<DeliveryDto>();
        }


        public async Task<DeliveryDto?> CreateAsync(
            CreateDeliveryRequest delivery)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "api/Deliveries");

            await AddAuthorizationAsync(request);

            request.Content =
                JsonContent.Create(delivery);

            var response =
                await _httpClient.SendAsync(request);

            CheckUnauthorized(response);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<DeliveryDto>();
        }


        public async Task<DeliveryDto?> UpdateAsync(
            int id,
            UpdateDeliveryRequest delivery)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Put,
                $"api/Deliveries/{id}");

            await AddAuthorizationAsync(request);

            request.Content =
                JsonContent.Create(delivery);

            var response =
                await _httpClient.SendAsync(request);

            CheckUnauthorized(response);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<DeliveryDto>();
        }


        public async Task<DeliveryDto?> ChangeStatusAsync(
            int id,
            ChangeDeliveryStatusRequest status)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Put,
                $"api/Deliveries/{id}/status");

            await AddAuthorizationAsync(request);

            request.Content =
                JsonContent.Create(status);

            var response =
                await _httpClient.SendAsync(request);

            CheckUnauthorized(response);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<DeliveryDto>();
        }


        // =====================================================
        // SELECTORES
        // =====================================================

        public async Task<List<DeliveryOrderDto>>
            GetOrdersAsync()
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                "api/Deliveries/orders");

            await AddAuthorizationAsync(request);

            var response =
                await _httpClient.SendAsync(request);

            CheckUnauthorized(response);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<List<DeliveryOrderDto>>()
                ?? new List<DeliveryOrderDto>();
        }


        public async Task<List<DeliveryAddressDto>>
            GetAddressesAsync()
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                "api/Deliveries/addresses");

            await AddAuthorizationAsync(request);

            var response =
                await _httpClient.SendAsync(request);

            CheckUnauthorized(response);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<List<DeliveryAddressDto>>()
                ?? new List<DeliveryAddressDto>();
        }


        public async Task<List<DeliveryDriverDto>>
            GetDriversAsync()
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                "api/Deliveries/drivers");

            await AddAuthorizationAsync(request);

            var response =
                await _httpClient.SendAsync(request);

            CheckUnauthorized(response);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<List<DeliveryDriverDto>>()
                ?? new List<DeliveryDriverDto>();
        }


        // =====================================================
        // JWT
        // =====================================================

        private async Task AddAuthorizationAsync(
            HttpRequestMessage request)
        {
            var token =
                await _js.InvokeAsync<string>(
                    "localStorage.getItem",
                    "authToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                throw new UnauthorizedAccessException();
            }

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