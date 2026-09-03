using System.Net;
using System.Net.Http.Json;
using TrazabilidadPedidos.Shared.DTOs;

namespace TrazabilidadPedidos.Client.Services
{
    public class AuthClientService
    {
        private readonly HttpClient _httpClient;

        public AuthClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/Auth/Login",
                request);

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                var body = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                throw new UnauthorizedAccessException(body?.Message ?? "Su cuenta ha sido desactivada.");
            }

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<AuthResponse>();
        }

        private class ErrorResponse
        {
            public string? Message { get; set; }
        }
    }
}
