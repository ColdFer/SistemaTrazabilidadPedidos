using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrazabilidadPedidos.Server.Repositories;
using TrazabilidadPedidos.Shared.Entities;

namespace TrazabilidadPedidos.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "ManageUsers")]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionRepository _permissionRepository;

        public PermissionsController(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<PermissionDto>>> GetAll()
        {
            var permissions = await _permissionRepository.GetAllAsync();
            return Ok(permissions.Select(p => new PermissionDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description
            }).ToList());
        }

        [HttpPost]
        public async Task<ActionResult<PermissionDto>> Create(CreatePermissionRequest request)
        {
            if (await _permissionRepository.HasDuplicateNameAsync(request.Name))
                return BadRequest(new { message = "Ya existe un permiso con ese nombre." });

            var permission = new Permission
            {
                Name = request.Name.Trim(),
                Description = request.Description?.Trim()
            };

            await _permissionRepository.AddAsync(permission);
            return Ok(new PermissionDto { Id = permission.Id, Name = permission.Name, Description = permission.Description });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, CreatePermissionRequest request)
        {
            var permission = await _permissionRepository.GetByIdAsync(id);
            if (permission == null) return NotFound();

            if (await _permissionRepository.HasDuplicateNameAsync(request.Name, id))
                return BadRequest(new { message = "Ya existe otro permiso con ese nombre." });

            permission.Name = request.Name.Trim();
            permission.Description = request.Description?.Trim();

            await _permissionRepository.UpdateAsync(permission);
            return Ok(new { message = "Permiso actualizado." });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var permission = await _permissionRepository.GetByIdAsync(id);
            if (permission == null) return NotFound();

            if (await _permissionRepository.IsAssignedToAnyRoleAsync(id))
                return BadRequest(new { message = "No se puede eliminar un permiso asignado a un rol." });

            await _permissionRepository.DeleteAsync(permission);
            return Ok(new { message = "Permiso eliminado." });
        }
    }

    public class CreatePermissionRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
