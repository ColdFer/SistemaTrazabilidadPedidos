using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.JSInterop;
using TrazabilidadPedidos.Shared.Entities;

namespace TrazabilidadPedidos.Client.Services
{
    public class AuditClientService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _js;

        public AuditClientService(HttpClient httpClient, IJSRuntime js)
        {
            _httpClient = httpClient;
            _js = js;
        }

        public async Task<(List<AuditLog> logs, int total, int totalPages)> GetAllAsync(
            int? userId = null, string? action = null, string? entity = null, int page = 1, int pageSize = 20)
        {
            var url = $"api/Audit?page={page}&pageSize={pageSize}";
            if (userId.HasValue) url += $"&userId={userId}";
            if (!string.IsNullOrEmpty(action)) url += $"&action={action}";
            if (!string.IsNullOrEmpty(entity)) url += $"&entity={entity}";

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            await AddAuthorizationAsync(request);
            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<AuditResponse>();
            return (result?.logs ?? new(), result?.total ?? 0, result?.totalPages ?? 0);
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

        private class AuditResponse
        {
            public List<AuditLog> logs { get; set; } = new();
            public int total { get; set; }
            public int totalPages { get; set; }
        }
    }
}
