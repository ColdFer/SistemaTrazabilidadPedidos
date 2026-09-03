using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.JSInterop;

namespace TrazabilidadPedidos.Client.Services
{
    public class PermissionClientService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _js;

        public PermissionClientService(HttpClient httpClient, IJSRuntime js)
        {
            _httpClient = httpClient;
            _js = js;
        }

        public async Task<List<PermissionDto>> GetAllAsync()
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "api/Permissions");
            await AddAuthorizationAsync(request);
            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<PermissionDto>>() ?? new();
        }

        public async Task<PermissionDto?> CreateAsync(string name, string? description)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/Permissions");
            await AddAuthorizationAsync(httpRequest);
            httpRequest.Content = JsonContent.Create(new { Name = name, Description = description });
            var response = await _httpClient.SendAsync(httpRequest);
            CheckUnauthorized(response);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<PermissionDto>();
        }

        public async Task<bool> UpdateAsync(int id, string name, string? description)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"api/Permissions/{id}");
            await AddAuthorizationAsync(httpRequest);
            httpRequest.Content = JsonContent.Create(new { Name = name, Description = description });
            var response = await _httpClient.SendAsync(httpRequest);
            CheckUnauthorized(response);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"api/Permissions/{id}");
            await AddAuthorizationAsync(httpRequest);
            var response = await _httpClient.SendAsync(httpRequest);
            CheckUnauthorized(response);
            return response.IsSuccessStatusCode;
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
