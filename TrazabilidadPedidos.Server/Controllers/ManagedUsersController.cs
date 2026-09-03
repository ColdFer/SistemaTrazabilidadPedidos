using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrazabilidadPedidos.Server.Services;
using TrazabilidadPedidos.Shared.DTOs.Users;

namespace TrazabilidadPedidos.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "ManageUsers")]
    public class ManagedUsersController : ControllerBase
    {
        private readonly IManagedUserService _userService;

        public ManagedUsersController(
            IManagedUserService userService)
        {
            _userService = userService;
        }


        // GET: api/ManagedUsers
        [HttpGet]
        public async Task<ActionResult<List<ManagedUserDto>>>
            GetAll()
        {
            var users =
                await _userService.GetAllAsync();

            return Ok(users);
        }


        // GET: api/ManagedUsers/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ManagedUserDto>>
            GetById(int id)
        {
            var user =
                await _userService.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "Usuario no encontrado."
                });
            }

            return Ok(user);
        }


        // POST: api/ManagedUsers
        [HttpPost]
        public async Task<ActionResult<ManagedUserDto>>
            Create(CreateManagedUserRequest request)
        {
            var user =
                await _userService.CreateAsync(request);

            if (user == null)
            {
                return BadRequest(new
                {
                    message =
                        "No se pudo crear el usuario. Verifique el correo, rol y datos ingresados."
                });
            }

            return Ok(user);
        }


        // PUT: api/ManagedUsers/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ManagedUserDto>>
            Update(
                int id,
                UpdateManagedUserRequest request)
        {
            var existing =
                await _userService.GetByIdAsync(id);

            if (existing == null)
            {
                return NotFound(new
                {
                    message = "Usuario no encontrado."
                });
            }

            var user =
                await _userService.UpdateAsync(
                    id,
                    request);

            if (user == null)
            {
                return BadRequest(new
                {
                    message =
                        "No se pudo actualizar el usuario. Verifique los datos ingresados."
                });
            }

            return Ok(user);
        }


        // DELETE: api/ManagedUsers/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult>
            Deactivate(int id)
        {
            var result =
                await _userService.DeactivateAsync(id);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Usuario no encontrado."
                });
            }

            return NoContent();
        }
    }
}