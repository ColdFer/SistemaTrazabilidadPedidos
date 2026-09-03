using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrazabilidadPedidos.Server.Data;
using TrazabilidadPedidos.Server.Repositories;
using TrazabilidadPedidos.Shared.Entities;

namespace TrazabilidadPedidos.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "ManageUsers")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleRepository _roleRepository;
        private readonly AppDbContext _context;

        public RolesController(IRoleRepository roleRepository, AppDbContext context)
        {
            _roleRepository = roleRepository;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<RoleDto>>> GetAll()
        {
            var roles = await _roleRepository.GetAllAsync();
            return Ok(roles.Select(MapToDto).ToList());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<RoleDto>> GetById(int id)
        {
            var role = await _roleRepository.GetWithPermissionsAsync(id);
            if (role == null) return NotFound();
            return Ok(MapToDto(role));
        }

        [HttpPost]
        public async Task<ActionResult<RoleDto>> Create(CreateRoleRequest request)
        {
            if (await _roleRepository.HasDuplicateNameAsync(request.Name))
                return BadRequest(new { message = "Ya existe un rol con ese nombre." });

            var role = new Role
            {
                Name = request.Name.Trim(),
                Description = request.Description?.Trim(),
                IsActive = true
            };

            await _roleRepository.AddAsync(role);
            return Ok(MapToDto(role));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateRoleRequest request)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null) return NotFound();

            if (await _roleRepository.HasDuplicateNameAsync(request.Name, id))
                return BadRequest(new { message = "Ya existe otro rol con ese nombre." });

            role.Name = request.Name.Trim();
            role.Description = request.Description?.Trim();
            role.IsActive = request.IsActive;

            await _roleRepository.UpdateAsync(role);
            return Ok(new { message = "Rol actualizado." });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null) return NotFound();

            if (await _roleRepository.HasUsersAsync(id))
                return BadRequest(new { message = "No se puede eliminar un rol que tiene usuarios asignados." });

            var protectedRoles = new[] { "Administrator", "Customer" };
            if (protectedRoles.Contains(role.Name))
                return BadRequest(new { message = "No se puede eliminar un rol del sistema." });

            await _context.RolePermissions
                .Where(rp => rp.RoleId == id)
                .ExecuteDeleteAsync();

            await _roleRepository.DeleteAsync(role);
            return Ok(new { message = "Rol eliminado." });
        }

        [HttpGet("{id:int}/permissions")]
        public async Task<ActionResult<List<int>>> GetPermissions(int id)
        {
            var role = await _roleRepository.GetWithPermissionsAsync(id);
            if (role == null) return NotFound();

            var permissionIds = role.RolePermissions.Select(rp => rp.PermissionId).ToList();
            return Ok(permissionIds);
        }

        [HttpPut("{id:int}/permissions")]
        public async Task<IActionResult> UpdatePermissions(int id, UpdateRolePermissionsRequest request)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null) return NotFound();

            var existing = await _context.RolePermissions
                .Where(rp => rp.RoleId == id)
                .ToListAsync();

            _context.RolePermissions.RemoveRange(existing);

            foreach (var permissionId in request.PermissionIds.Distinct())
            {
                var permission = await _context.Permissions.FindAsync(permissionId);
                if (permission != null)
                {
                    _context.RolePermissions.Add(new RolePermission
                    {
                        RoleId = id,
                        PermissionId = permissionId
                    });
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Permisos actualizados." });
        }

        private static RoleDto MapToDto(Role role)
        {
            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsActive = role.IsActive,
                UserCount = role.Users?.Count ?? 0,
                Permissions = role.RolePermissions?
                    .Where(rp => rp.Permission != null)
                    .Select(rp => new PermissionDto
                    {
                        Id = rp.Permission!.Id,
                        Name = rp.Permission.Name,
                        Description = rp.Permission.Description
                    }).ToList() ?? new List<PermissionDto>()
            };
        }
    }

    public class RoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int UserCount { get; set; }
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

    public class UpdateRolePermissionsRequest
    {
        public List<int> PermissionIds { get; set; } = new();
    }
}
