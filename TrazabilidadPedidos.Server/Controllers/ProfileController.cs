using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrazabilidadPedidos.Server.Data;
using TrazabilidadPedidos.Shared.DTOs.Profile;

namespace TrazabilidadPedidos.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "ManageAddresses")]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProfileController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ProfileDto>> GetProfile()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var customer = await _context.Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId.Value);

            if (customer == null)
                return NotFound(new { message = "Cliente no encontrado." });

            var address = await _context.Addresses
                .Where(a => a.CustomerId == customer.Id && a.IsActive)
                .OrderByDescending(a => a.CreatedAt)
                .FirstOrDefaultAsync();

            return Ok(new ProfileDto
            {
                CustomerId = customer.Id,
                UserId = customer.UserId,
                FirstName = customer.User?.FirstName ?? string.Empty,
                LastName = customer.User?.LastName ?? string.Empty,
                Email = customer.User?.Email ?? string.Empty,
                Ci = customer.Ci,
                Phone = customer.Phone,
                Address = address?.AddressLine
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var customer = await _context.Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId.Value);

            if (customer == null)
                return NotFound(new { message = "Cliente no encontrado." });

            customer.User!.FirstName = request.FirstName.Trim();
            customer.User.LastName = request.LastName.Trim();
            customer.User.UpdatedAt = DateTime.Now;
            customer.Phone = request.Phone.Trim();
            customer.UpdatedAt = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(request.Address))
            {
                var existingAddress = await _context.Addresses
                    .Where(a => a.CustomerId == customer.Id && a.IsActive)
                    .OrderByDescending(a => a.CreatedAt)
                    .FirstOrDefaultAsync();

                if (existingAddress != null)
                {
                    existingAddress.AddressLine = request.Address.Trim();
                    existingAddress.UpdatedAt = DateTime.Now;
                }
                else
                {
                    _context.Addresses.Add(new Shared.Entities.Address
                    {
                        CustomerId = customer.Id,
                        AddressLine = request.Address.Trim(),
                        Label = "Principal",
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    });
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Perfil actualizado correctamente." });
        }

        private int? GetUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(claim, out var userId))
                return userId;
            return null;
        }
    }
}
