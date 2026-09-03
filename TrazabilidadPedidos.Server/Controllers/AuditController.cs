using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrazabilidadPedidos.Server.Services;
using TrazabilidadPedidos.Shared.Entities;

namespace TrazabilidadPedidos.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "GenerateReports")]
    public class AuditController : ControllerBase
    {
        private readonly IAuditService _auditService;

        public AuditController(IAuditService auditService)
        {
            _auditService = auditService;
        }

        [HttpGet]
        public async Task<ActionResult<List<AuditLog>>> GetAll(
            [FromQuery] int? userId,
            [FromQuery] string? action,
            [FromQuery] string? entity,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var logs = await _auditService.GetAllAsync(userId, action, entity, page, pageSize);
            var total = await _auditService.GetCountAsync(userId, action, entity);

            return Ok(new
            {
                logs,
                total,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling((double)total / pageSize)
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuditLog>> GetById(int id)
        {
            var log = await _auditService.GetByIdAsync(id);
            if (log == null) return NotFound();
            return Ok(log);
        }
    }
}
