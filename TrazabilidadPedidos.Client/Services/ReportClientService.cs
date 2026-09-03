using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.JSInterop;
using TrazabilidadPedidos.Shared.DTOs.Reports;

namespace TrazabilidadPedidos.Client.Services
{
    public class ReportClientService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _js;

        public ReportClientService(
            HttpClient httpClient,
            IJSRuntime js)
        {
            _httpClient = httpClient;
            _js = js;
        }

        public async Task<DashboardReportDto?> GetDashboardReportAsync()
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get, "api/Reports/dashboard");

            await AddAuthorizationAsync(request);
            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<DashboardReportDto>();
        }

        public async Task<SalesByPeriodReportDto?> GetSalesByPeriodAsync(DateTime start, DateTime end)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get, $"api/Reports/sales-by-period?start={start:yyyy-MM-dd}&end={end:yyyy-MM-dd}");

            await AddAuthorizationAsync(request);
            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<SalesByPeriodReportDto>();
        }

        public async Task<TopProductsReportDto?> GetTopProductsAsync(int limit = 10)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get, $"api/Reports/top-products?limit={limit}");

            await AddAuthorizationAsync(request);
            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<TopProductsReportDto>();
        }

        public async Task<OrdersByStatusReportDto?> GetOrdersByStatusAsync()
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get, "api/Reports/orders-by-status");

            await AddAuthorizationAsync(request);
            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<OrdersByStatusReportDto>();
        }

        public async Task<TopCustomersReportDto?> GetTopCustomersAsync(int limit = 10)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get, $"api/Reports/top-customers?limit={limit}");

            await AddAuthorizationAsync(request);
            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<TopCustomersReportDto>();
        }

        public async Task<DriverPerformanceReportDto?> GetDriverPerformanceAsync()
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get, "api/Reports/driver-performance");

            await AddAuthorizationAsync(request);
            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<DriverPerformanceReportDto>();
        }

        public async Task<InventoryReportDto?> GetInventoryReportAsync()
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get, "api/Reports/inventory");

            await AddAuthorizationAsync(request);
            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<InventoryReportDto>();
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

