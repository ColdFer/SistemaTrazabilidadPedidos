using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrazabilidadPedidos.Server.Services;

namespace TrazabilidadPedidos.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SystemController : ControllerBase
    {
        private readonly IApplicationInfoService _applicationInfoService;

        public SystemController(
            IApplicationInfoService applicationInfoService)
        {
            _applicationInfoService = applicationInfoService;
        }

        [HttpGet("info")]
        public IActionResult GetInfo()
        {
            return Ok(new
            {
                application = _applicationInfoService.ApplicationName,
                version = _applicationInfoService.Version,
                architecture = "Blazor WebAssembly + ASP.NET Core Web API + Shared Library"
            });
        }

        [Authorize]
        [HttpGet("protected")]
        public IActionResult GetProtected()
        {
            return Ok(new
            {
                message = "Authenticated access granted",
                userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                name = User.FindFirst(ClaimTypes.Name)?.Value,
                email = User.FindFirst(ClaimTypes.Email)?.Value,
                role = User.FindFirst(ClaimTypes.Role)?.Value
            });
        }
        [Authorize(Roles = "Customer")]
        [HttpGet("customer")]
        public IActionResult GetCustomerArea()
        {
            return Ok(new
            {
                message = "Customer access granted",
                role = User.FindFirst(ClaimTypes.Role)?.Value
            });
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet("administrator")]
        public IActionResult GetAdministratorArea()
        {
            return Ok(new
            {
                message = "Administrator access granted",
                role = User.FindFirst(ClaimTypes.Role)?.Value
            });
        }
    }
}