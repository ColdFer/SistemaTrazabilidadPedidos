using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.JSInterop;
using TrazabilidadPedidos.Shared.DTOs.Users;

namespace TrazabilidadPedidos.Client.Services
{
    public class ManagedUserClientService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _js;

        public ManagedUserClientService(
            HttpClient httpClient,
            IJSRuntime js)
        {
            _httpClient = httpClient;
            _js = js;
        }


        public async Task<List<ManagedUserDto>>
            GetAllAsync()
        {
            using var request =
                new HttpRequestMessage(
                    HttpMethod.Get,
                    "api/ManagedUsers");

            await AddAuthorizationAsync(request);

            var response =
                await _httpClient.SendAsync(request);

            CheckUnauthorized(response);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<List<ManagedUserDto>>()
                ?? new List<ManagedUserDto>();
        }


        public async Task<ManagedUserDto?> CreateAsync(
            CreateManagedUserRequest user)
        {
            using var request =
                new HttpRequestMessage(
                    HttpMethod.Post,
                    "api/ManagedUsers");

            await AddAuthorizationAsync(request);

            request.Content =
                JsonContent.Create(user);

            var response =
                await _httpClient.SendAsync(request);

            CheckUnauthorized(response);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<ManagedUserDto>();
        }


        public async Task<ManagedUserDto?> UpdateAsync(
            int id,
            UpdateManagedUserRequest user)
        {
            using var request =
                new HttpRequestMessage(
                    HttpMethod.Put,
                    $"api/ManagedUsers/{id}");

            await AddAuthorizationAsync(request);

            request.Content =
                JsonContent.Create(user);

            var response =
                await _httpClient.SendAsync(request);

            CheckUnauthorized(response);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<ManagedUserDto>();
        }


        public async Task<bool> DeactivateAsync(int id)
        {
            using var request =
                new HttpRequestMessage(
                    HttpMethod.Delete,
                    $"api/ManagedUsers/{id}");

            await AddAuthorizationAsync(request);

            var response =
                await _httpClient.SendAsync(request);

            CheckUnauthorized(response);

            return response.IsSuccessStatusCode;
        }


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


        private static void CheckUnauthorized(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}