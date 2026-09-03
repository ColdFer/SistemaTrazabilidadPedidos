using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.JSInterop;
using TrazabilidadPedidos.Shared.Entities;

namespace TrazabilidadPedidos.Client.Services
{
    public class InvoiceClientService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _js;

        public InvoiceClientService(HttpClient httpClient, IJSRuntime js)
        {
            _httpClient = httpClient;
            _js = js;
        }

        public async Task<Invoice?> GetByIdAsync(int id)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"api/Invoices/{id}");
            await AddAuthorizationAsync(request);
            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Invoice>();
        }

        public async Task<List<Invoice>> GetAllAsync()
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "api/Invoices");
            await AddAuthorizationAsync(request);
            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Invoice>>()
                ?? new List<Invoice>();
        }

        public async Task<List<Invoice>> GetMyInvoicesAsync()
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "api/Invoices/my-invoices");
            await AddAuthorizationAsync(request);
            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Invoice>>()
                ?? new List<Invoice>();
        }

        public async Task<Invoice?> GenerateAsync(int orderId)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, $"api/Invoices/generate/{orderId}");
            await AddAuthorizationAsync(request);
            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<Invoice>();
        }

        public async Task DownloadPdfAsync(int invoiceId)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"api/Invoices/{invoiceId}/pdf");
            await AddAuthorizationAsync(request);
            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);
            response.EnsureSuccessStatusCode();

            var bytes = await response.Content.ReadAsByteArrayAsync();
            var fileName = $"Factura-{invoiceId}.pdf";
            await _js.InvokeVoidAsync("downloadFile", fileName, bytes);
        }

        public async Task DownloadPdfByOrderAsync(int orderId)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"api/Invoices/order/{orderId}/pdf");
            await AddAuthorizationAsync(request);
            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);
            response.EnsureSuccessStatusCode();

            var bytes = await response.Content.ReadAsByteArrayAsync();
            var fileName = $"Factura-Pedido-{orderId}.pdf";
            await _js.InvokeVoidAsync("downloadFile", fileName, bytes);
        }

        private async Task AddAuthorizationAsync(HttpRequestMessage request)
        {
            var token = await _js.InvokeAsync<string>("localStorage.getItem", "authToken");
            if (string.IsNullOrWhiteSpace(token)) throw new UnauthorizedAccessException();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
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
