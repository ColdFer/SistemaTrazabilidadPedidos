using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.JSInterop;
using TrazabilidadPedidos.Shared.DTOs.Profile;

namespace TrazabilidadPedidos.Client.Services
{
    public class ProfileClientService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _js;

        public ProfileClientService(
            HttpClient httpClient,
            IJSRuntime js)
        {
            _httpClient = httpClient;
            _js = js;
        }

        public async Task<ProfileDto?> GetProfileAsync()
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get, "api/Profile");

            await AddAuthorizationAsync(request);
            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<ProfileDto>();
        }

        public async Task<bool> UpdateProfileAsync(
            UpdateProfileRequest profile)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Put, "api/Profile");

            await AddAuthorizationAsync(request);
            request.Content = JsonContent.Create(profile);

            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);

            return response.IsSuccessStatusCode;
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

