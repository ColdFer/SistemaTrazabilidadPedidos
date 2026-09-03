using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrazabilidadPedidos.Server.Services;
using TrazabilidadPedidos.Shared.DTOs.Customers;

namespace TrazabilidadPedidos.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "ManageUsers")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(
            ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<List<CustomerDto>>> GetAll()
        {
            var customers =
                await _customerService.GetAllAsync();

            return Ok(customers);
        }

        // GET: api/Customers/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CustomerDto>> GetById(int id)
        {
            var customer =
                await _customerService.GetByIdAsync(id);

            if (customer == null)
            {
                return NotFound(new
                {
                    message = "Cliente no encontrado."
                });
            }

            return Ok(customer);
        }

        // POST: api/Customers
        [HttpPost]
        public async Task<ActionResult<CustomerDto>> Create(
            CreateCustomerRequest request)
        {
            var customer =
                await _customerService.CreateAsync(request);

            if (customer == null)
            {
                return BadRequest(new
                {
                    message =
                        "No se pudo registrar el cliente. Verifique que el correo y CI no estén registrados."
                });
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = customer.Id },
                customer);
        }

        // PUT: api/Customers/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<CustomerDto>> Update(
            int id,
            UpdateCustomerRequest request)
        {
            var existing =
                await _customerService.GetByIdAsync(id);

            if (existing == null)
            {
                return NotFound(new
                {
                    message = "Cliente no encontrado."
                });
            }

            var customer =
                await _customerService.UpdateAsync(
                    id,
                    request);

            if (customer == null)
            {
                return BadRequest(new
                {
                    message =
                        "No se pudo actualizar el cliente. Verifique que el correo y CI no estén registrados por otro cliente."
                });
            }

            return Ok(customer);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result =
                await _customerService.DeactivateAsync(id);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Cliente no encontrado."
                });
            }

            return NoContent();
        }
    }
}