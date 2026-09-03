using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.JSInterop;
using TrazabilidadPedidos.Shared.DTOs.Payments;

namespace TrazabilidadPedidos.Client.Services
{
    public class PaymentClientService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _js;

        public PaymentClientService(HttpClient httpClient, IJSRuntime js)
        {
            _httpClient = httpClient;
            _js = js;
        }

        public async Task<PaymentDto?> GetByIdAsync(int id)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get, $"api/Payments/{id}");
            await AddAuthorizationAsync(request);
            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PaymentDto>();
        }

        public async Task<List<PaymentDto>> GetAllAsync()
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get, "api/Payments");
            await AddAuthorizationAsync(request);
            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<PaymentDto>>()
                ?? new List<PaymentDto>();
        }

        public async Task<List<PaymentDto>> GetPendingAsync()
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get, "api/Payments/pending");
            await AddAuthorizationAsync(request);
            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<PaymentDto>>()
                ?? new List<PaymentDto>();
        }

        public async Task<List<PaymentDto>> GetMyPaymentsAsync()
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get, "api/Payments/my-payments");
            await AddAuthorizationAsync(request);
            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<PaymentDto>>()
                ?? new List<PaymentDto>();
        }

        public async Task<PaymentDto?> CreateAsync(CreatePaymentRequest item)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post, "api/Payments");
            await AddAuthorizationAsync(request);
            request.Content = JsonContent.Create(item);
            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<PaymentDto>();
        }

        public async Task<PaymentDto?> VerifyAsync(
            int paymentId, VerifyPaymentRequest request)
        {
            using var httpRequest = new HttpRequestMessage(
                HttpMethod.Put, $"api/Payments/{paymentId}/verify");
            await AddAuthorizationAsync(httpRequest);
            httpRequest.Content = JsonContent.Create(request);
            var response = await _httpClient.SendAsync(httpRequest);
            CheckUnauthorized(response);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<PaymentDto>();
        }

        private async Task AddAuthorizationAsync(HttpRequestMessage request)
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
