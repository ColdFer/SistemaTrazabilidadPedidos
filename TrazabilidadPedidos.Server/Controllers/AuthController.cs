using Microsoft.AspNetCore.Mvc;
using TrazabilidadPedidos.Server.Services;
using TrazabilidadPedidos.Shared.DTOs;

namespace TrazabilidadPedidos.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
        {
            var response = await _authService.RegisterAsync(request);
            if (response == null)
            {
                return BadRequest("Usuario existente");
            }
            return Ok(response);
        }
        [HttpPost("Login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);
            if (response == null)
            {
                return Unauthorized("Credenciales invalidas");
            }
            return Ok(response);
        }
    }
}

