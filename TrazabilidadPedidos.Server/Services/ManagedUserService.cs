using TrazabilidadPedidos.Server.Repositories;
using TrazabilidadPedidos.Shared.DTOs.Users;
using TrazabilidadPedidos.Shared.Entities;

namespace TrazabilidadPedidos.Server.Services
{
    public class ManagedUserService : IManagedUserService
    {
        private readonly IManagedUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        private static readonly string[] AllowedRoles =
        {
            "Administrator",
            "Operator",
            "DeliveryDriver"
        };

        public ManagedUserService(
            IManagedUserRepository userRepository,
            IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }


        // =====================================================
        // LISTAR USUARIOS INTERNOS
        // =====================================================

        public async Task<List<ManagedUserDto>> GetAllAsync()
        {
            var users =
                await _userRepository.GetAllAsync();

            var internalUsers =
                users.Where(u =>
                    u.Role != null &&
                    AllowedRoles.Contains(u.Role.Name));

            var result =
                new List<ManagedUserDto>();

            foreach (var user in internalUsers)
            {
                result.Add(
                    await MapToDtoAsync(user));
            }

            return result;
        }


        // =====================================================
        // OBTENER POR ID
        // =====================================================

        public async Task<ManagedUserDto?> GetByIdAsync(int id)
        {
            var user =
                await _userRepository.GetByIdAsync(id);

            if (user == null ||
                user.Role == null ||
                !AllowedRoles.Contains(user.Role.Name))
            {
                return null;
            }

            return await MapToDtoAsync(user);
        }


        // =====================================================
        // CREAR
        // =====================================================

        public async Task<ManagedUserDto?> CreateAsync(
            CreateManagedUserRequest request)
        {
            var roleName =
                request.RoleName.Trim();

            // Solo se pueden crear estos roles desde Administración.
            if (!AllowedRoles.Contains(roleName))
            {
                return null;
            }


            var email =
                request.Email.Trim().ToLower();

            if (await _userRepository
                .EmailExistsAsync(email))
            {
                return null;
            }


            var role =
                await _roleRepository
                    .GetByNameAsync(roleName);

            if (role == null || !role.IsActive)
            {
                return null;
            }


            // Un repartidor debe tener teléfono.
            if (roleName == "DeliveryDriver" &&
                string.IsNullOrWhiteSpace(request.Phone))
            {
                return null;
            }


            var now = DateTime.Now;


            var user = new User
            {
                RoleId = role.Id,
                Role = role,

                FirstName =
                    request.FirstName.Trim(),

                LastName =
                    request.LastName.Trim(),

                Email = email,

                PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword(
                        request.Password),

                IsActive = true,

                CreatedAt = now,
                UpdatedAt = now
            };


            await _userRepository
                .AddUserAsync(user);


            /*
             * Si el rol es DeliveryDriver,
             * también creamos automáticamente
             * su perfil de repartidor.
             */
            if (roleName == "DeliveryDriver")
            {
                var driver =
                    new DeliveryDriver
                    {
                        User = user,

                        Phone =
                            request.Phone!.Trim(),

                        IsAvailable = true,

                        CreatedAt = now,
                        UpdatedAt = now
                    };

                await _userRepository
                    .AddDriverAsync(driver);
            }


            /*
             * User + DeliveryDriver, cuando corresponda,
             * se guardan juntos mediante SaveChanges.
             */
            await _userRepository.SaveChangesAsync();


            return await MapToDtoAsync(user);
        }


        // =====================================================
        // EDITAR
        // =====================================================

        public async Task<ManagedUserDto?> UpdateAsync(
            int id,
            UpdateManagedUserRequest request)
        {
            var user =
                await _userRepository.GetByIdAsync(id);

            if (user == null ||
                user.Role == null ||
                !AllowedRoles.Contains(user.Role.Name))
            {
                return null;
            }


            var email =
                request.Email.Trim().ToLower();

            if (await _userRepository.EmailExistsAsync(
                email,
                user.Id))
            {
                return null;
            }


            user.FirstName =
                request.FirstName.Trim();

            user.LastName =
                request.LastName.Trim();

            user.Email = email;

            user.IsActive =
                request.IsActive;


            // Si el administrador escribió una nueva contraseña,
            // reemplazamos el hash anterior.
            if (!string.IsNullOrWhiteSpace(request.NewPassword))
            {
                if (request.NewPassword != request.ConfirmNewPassword)
                {
                    return null;
                }

                user.PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword(
                        request.NewPassword);
            }


            user.UpdatedAt =
                DateTime.Now;


            // Datos específicos del repartidor.
            if (user.Role.Name == "DeliveryDriver")
            {
                var driver =
                    await _userRepository
                        .GetDriverByUserIdAsync(user.Id);

                if (driver == null)
                {
                    return null;
                }


                if (string.IsNullOrWhiteSpace(
                    request.Phone))
                {
                    return null;
                }


                driver.Phone =
                    request.Phone.Trim();


                if (request.IsAvailable.HasValue)
                {
                    driver.IsAvailable =
                        request.IsAvailable.Value;
                }


                // Usuario inactivo no debe quedar disponible.
                if (!request.IsActive)
                {
                    driver.IsAvailable = false;
                }


                driver.UpdatedAt =
                    DateTime.Now;
            }


            await _userRepository.SaveChangesAsync();


            return await MapToDtoAsync(user);
        }


        // =====================================================
        // DESACTIVAR
        // =====================================================

        public async Task<bool> DeactivateAsync(int id)
        {
            var user =
                await _userRepository.GetByIdAsync(id);

            if (user == null ||
                user.Role == null ||
                !AllowedRoles.Contains(user.Role.Name))
            {
                return false;
            }


            user.IsActive = false;
            user.UpdatedAt = DateTime.Now;


            if (user.Role.Name == "DeliveryDriver")
            {
                var driver =
                    await _userRepository
                        .GetDriverByUserIdAsync(user.Id);

                if (driver != null)
                {
                    driver.IsAvailable = false;
                    driver.UpdatedAt = DateTime.Now;
                }
            }


            await _userRepository.SaveChangesAsync();

            return true;
        }


        // =====================================================
        // MAPEO
        // =====================================================

        private async Task<ManagedUserDto>
            MapToDtoAsync(User user)
        {
            string? phone = null;
            bool? isAvailable = null;


            if (user.Role?.Name == "DeliveryDriver")
            {
                var driver =
                    await _userRepository
                        .GetDriverByUserIdAsync(user.Id);

                if (driver != null)
                {
                    phone = driver.Phone;
                    isAvailable = driver.IsAvailable;
                }
            }


            return new ManagedUserDto
            {
                Id = user.Id,

                FirstName = user.FirstName,
                LastName = user.LastName,

                Email = user.Email,

                Role =
                    user.Role?.Name
                    ?? string.Empty,

                IsActive = user.IsActive,

                Phone = phone,

                IsAvailable = isAvailable
            };
        }
    }
}