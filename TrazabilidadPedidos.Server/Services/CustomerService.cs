using TrazabilidadPedidos.Server.Repositories;
using TrazabilidadPedidos.Shared.DTOs.Customers;
using TrazabilidadPedidos.Shared.Entities;

namespace TrazabilidadPedidos.Server.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IRoleRepository _roleRepository;

        public CustomerService(
            ICustomerRepository customerRepository,
            IRoleRepository roleRepository)
        {
            _customerRepository = customerRepository;
            _roleRepository = roleRepository;
        }

        public async Task<List<CustomerDto>> GetAllAsync()
        {
            var customers =
                await _customerRepository.GetAllAsync();

            return customers
                .Select(MapToDto)
                .ToList();
        }

        public async Task<CustomerDto?> GetByIdAsync(int id)
        {
            var customer =
                await _customerRepository.GetByIdAsync(id);

            if (customer == null)
                return null;

            return MapToDto(customer);
        }

        public async Task<CustomerDto?> CreateAsync(
            CreateCustomerRequest request)
        {
            if (await _customerRepository.EmailExistsAsync(
                    request.Email))
            {
                return null;
            }

            if (await _customerRepository.CiExistsAsync(
                    request.Ci))
            {
                return null;
            }

            var customerRole =
                await _roleRepository.GetByNameAsync("Customer");

            if (customerRole == null)
                return null;

            var now = DateTime.Now;

            var user = new User
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = request.Email.Trim(),
                PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword(
                        request.Password),

                RoleId = customerRole.Id,
                Role = customerRole,

                IsActive = true,

                CreatedAt = now,
                UpdatedAt = now
            };

            var customer = new Customer
            {
                User = user,

                Ci = request.Ci.Trim(),
                Phone = request.Phone.Trim(),

                CreatedAt = now,
                UpdatedAt = now
            };

            await _customerRepository.AddAsync(customer);
            await _customerRepository.SaveChangesAsync();

            return MapToDto(customer);
        }

        public async Task<CustomerDto?> UpdateAsync(
            int id,
            UpdateCustomerRequest request)
        {
            var customer =
                await _customerRepository.GetByIdAsync(id);

            if (customer == null ||
                customer.User == null)
            {
                return null;
            }

            if (await _customerRepository.EmailExistsAsync(
                    request.Email,
                    customer.UserId))
            {
                return null;
            }

            if (await _customerRepository.CiExistsAsync(
                    request.Ci,
                    customer.Id))
            {
                return null;
            }

            customer.User.FirstName =
                request.FirstName.Trim();

            customer.User.LastName =
                request.LastName.Trim();

            customer.User.Email =
                request.Email.Trim();

            customer.User.IsActive =
                request.IsActive;

            customer.User.UpdatedAt =
                DateTime.Now;

            customer.Ci =
                request.Ci.Trim();

            customer.Phone =
                request.Phone.Trim();

            customer.UpdatedAt =
                DateTime.Now;

            await _customerRepository.SaveChangesAsync();

            return MapToDto(customer);
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            var customer =
                await _customerRepository.GetByIdAsync(id);

            if (customer == null ||
                customer.User == null)
            {
                return false;
            }

            customer.User.IsActive = false;
            customer.User.UpdatedAt = DateTime.Now;

            customer.UpdatedAt = DateTime.Now;

            await _customerRepository.SaveChangesAsync();

            return true;
        }

        private static CustomerDto MapToDto(
            Customer customer)
        {
            return new CustomerDto
            {
                Id = customer.Id,
                UserId = customer.UserId,

                FirstName =
                    customer.User?.FirstName
                    ?? string.Empty,

                LastName =
                    customer.User?.LastName
                    ?? string.Empty,

                Email =
                    customer.User?.Email
                    ?? string.Empty,

                Ci = customer.Ci,
                Phone = customer.Phone,

                IsActive =
                    customer.User?.IsActive
                    ?? false
            };
        }
    }
}