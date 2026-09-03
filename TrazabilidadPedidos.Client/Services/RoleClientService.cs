using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.JSInterop;

namespace TrazabilidadPedidos.Client.Services
{
    public class RoleClientService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _js;

        public RoleClientService(HttpClient httpClient, IJSRuntime js)
        {
            _httpClient = httpClient;
            _js = js;
        }

        public async Task<List<RoleDto>> GetAllAsync()
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "api/Roles");
            await AddAuthorizationAsync(request);
            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<RoleDto>>() ?? new();
        }

        public async Task<RoleDto?> GetByIdAsync(int id)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"api/Roles/{id}");
            await AddAuthorizationAsync(request);
            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);
            if (response.StatusCode == HttpStatusCode.NotFound) return null;
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<RoleDto>();
        }

        public async Task<RoleDto?> CreateAsync(CreateRoleRequest request)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/Roles");
            await AddAuthorizationAsync(httpRequest);
            httpRequest.Content = JsonContent.Create(request);
            var response = await _httpClient.SendAsync(httpRequest);
            CheckUnauthorized(response);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<RoleDto>();
        }

        public async Task<bool> UpdateAsync(int id, UpdateRoleRequest request)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"api/Roles/{id}");
            await AddAuthorizationAsync(httpRequest);
            httpRequest.Content = JsonContent.Create(request);
            var response = await _httpClient.SendAsync(httpRequest);
            CheckUnauthorized(response);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"api/Roles/{id}");
            await AddAuthorizationAsync(httpRequest);
            var response = await _httpClient.SendAsync(httpRequest);
            CheckUnauthorized(response);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<int>> GetPermissionsAsync(int roleId)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"api/Roles/{roleId}/permissions");
            await AddAuthorizationAsync(request);
            var response = await _httpClient.SendAsync(request);
            CheckUnauthorized(response);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<int>>() ?? new();
        }

        public async Task<bool> UpdatePermissionsAsync(int roleId, List<int> permissionIds)
        {
            using var request = new HttpRequestMessage(HttpMethod.Put, $"api/Roles/{roleId}/permissions");
            await AddAuthorizationAsync(request);
            request.Content = JsonContent.Create(new { PermissionIds = permissionIds });
            var response = await _httpClient.SendAsync(request);
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

    public class RoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int UserCount { get; set; }
        public int PermCount => Permissions.Count;
        public List<PermissionDto> Permissions { get; set; } = new();
    }

    public class PermissionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class CreateRoleRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class UpdateRoleRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
