using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.JSInterop;
using TrazabilidadPedidos.Shared.Entities;

namespace TrazabilidadPedidos.Client.Services
{
    public class NotificationClientService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _js;

        public NotificationClientService(HttpClient httpClient, IJSRuntime js)
        {
            _httpClient = httpClient;
            _js = js;
        }

        public async Task<List<Notification>> GetAllAsync()
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "api/Notifications");
            await AddAuthorizationAsync(request);
            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Notification>>()
                ?? new List<Notification>();
        }

        public async Task<int> GetUnreadCountAsync()
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "api/Notifications/unread-count");
            await AddAuthorizationAsync(request);
            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<int>();
        }

        public async Task MarkAsReadAsync(int id)
        {
            using var request = new HttpRequestMessage(HttpMethod.Put, $"api/Notifications/{id}/read");
            await AddAuthorizationAsync(request);
            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);
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
