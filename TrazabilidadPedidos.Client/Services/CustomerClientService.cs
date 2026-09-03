using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.JSInterop;
using TrazabilidadPedidos.Shared.DTOs.Customers;

namespace TrazabilidadPedidos.Client.Services
{
    public class CustomerClientService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _js;

        public CustomerClientService(
            HttpClient httpClient,
            IJSRuntime js)
        {
            _httpClient = httpClient;
            _js = js;
        }

        public async Task<List<CustomerDto>> GetAllAsync()
        {
            using var request =
                new HttpRequestMessage(
                    HttpMethod.Get,
                    "api/Customers");

            await AddAuthorizationAsync(request);

            var response =
                await _httpClient.SendAsync(request);

            if (response.StatusCode ==
                HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException();
            }

            if (response.StatusCode ==
                HttpStatusCode.Forbidden)
            {
                throw new UnauthorizedAccessException();
            }

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<List<CustomerDto>>()
                ?? new List<CustomerDto>();
        }

        public async Task<CustomerDto?> GetByIdAsync(int id)
        {
            using var request =
                new HttpRequestMessage(
                    HttpMethod.Get,
                    $"api/Customers/{id}");

            await AddAuthorizationAsync(request);

            var response =
                await _httpClient.SendAsync(request);

            if (response.StatusCode ==
                HttpStatusCode.NotFound)
            {
                return null;
            }

            if (response.StatusCode ==
                HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException();
            }

            if (response.StatusCode ==
                HttpStatusCode.Forbidden)
            {
                throw new UnauthorizedAccessException();
            }

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<CustomerDto>();
        }

        public async Task<CustomerDto?> CreateAsync(
            CreateCustomerRequest customer)
        {
            using var request =
                new HttpRequestMessage(
                    HttpMethod.Post,
                    "api/Customers");

            await AddAuthorizationAsync(request);

            request.Content =
                JsonContent.Create(customer);

            var response =
                await _httpClient.SendAsync(request);

            if (response.StatusCode ==
                HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException();
            }

            if (response.StatusCode ==
                HttpStatusCode.Forbidden)
            {
                throw new UnauthorizedAccessException();
            }

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content
                .ReadFromJsonAsync<CustomerDto>();
        }

        public async Task<CustomerDto?> UpdateAsync(
            int id,
            UpdateCustomerRequest customer)
        {
            using var request =
                new HttpRequestMessage(
                    HttpMethod.Put,
                    $"api/Customers/{id}");

            await AddAuthorizationAsync(request);

            request.Content =
                JsonContent.Create(customer);

            var response =
                await _httpClient.SendAsync(request);

            if (response.StatusCode ==
                HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException();
            }

            if (response.StatusCode ==
                HttpStatusCode.Forbidden)
            {
                throw new UnauthorizedAccessException();
            }

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content
                .ReadFromJsonAsync<CustomerDto>();
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            using var request =
                new HttpRequestMessage(
                    HttpMethod.Delete,
                    $"api/Customers/{id}");

            await AddAuthorizationAsync(request);

            var response =
                await _httpClient.SendAsync(request);

            if (response.StatusCode ==
                HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException();
            }

            if (response.StatusCode ==
                HttpStatusCode.Forbidden)
            {
                throw new UnauthorizedAccessException();
            }

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
    }
}