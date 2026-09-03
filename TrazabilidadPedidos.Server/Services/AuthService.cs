using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TrazabilidadPedidos.Server.Data;
using TrazabilidadPedidos.Server.Repositories;
using TrazabilidadPedidos.Shared.DTOs;
using TrazabilidadPedidos.Shared.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TrazabilidadPedidos.Server.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public AuthService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IConfiguration configuration,
            AppDbContext context)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _configuration = configuration;
            _context = context;
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);

            if (user == null)
            {
                return null;
            }

            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException("Su cuenta ha sido desactivada. Contacte al administrador.");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return null;
            }

            return GenerateAuthResponse(user);
        }

        public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
        {
            if (await _userRepository.UserExistsAsync(request.Email))
            {
                return null;
            }

            string passwordHash =
                BCrypt.Net.BCrypt.HashPassword(request.Password);

            var role = await _roleRepository
                .GetByNameAsync("Customer");

            if (role == null)
            {
                throw new Exception("The selected role does not exist.");
            }

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = passwordHash,
                RoleId = role.Id,
                Role = role,
                IsActive = true
            };

            var createdUser =
                await _userRepository.CreateUserAsync(user);

            var customer = new Customer
            {
                UserId = createdUser.Id,
                Ci = $"CLI-{createdUser.Id:D8}",
                Phone = "000000000",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return GenerateAuthResponse(createdUser);
        }

        private AuthResponse GenerateAuthResponse(User user)
        {
            var tokenString = GenerateToken(user);

            return new AuthResponse
            {
                Token = tokenString,
                UserId = user.Id,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                Role = user.Role?.Name ?? string.Empty
            };
        }

        private string GenerateToken(User user)
        {
            var jwtSettings =
                _configuration.GetSection("JwtSettings");

            var key = Encoding.UTF8.GetBytes(
                jwtSettings["SecretKey"]!
            );

            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()
                ),

                new Claim(
                    ClaimTypes.Email,
                    user.Email
                ),

                new Claim(
                    ClaimTypes.Name,
                    $"{user.FirstName} {user.LastName}"
                ),

                new Claim(
                    ClaimTypes.Role,
                    user.Role?.Name ?? string.Empty
                )
            };

            var tokenDescriptor =
                new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),

                    Expires = DateTime.UtcNow.AddMinutes(
                        double.Parse(
                            jwtSettings["ExpirationMinutes"]!
                        )
                    ),

                    Issuer = jwtSettings["Issuer"],

                    Audience = jwtSettings["Audience"],

                    SigningCredentials =
                        new SigningCredentials(
                            new SymmetricSecurityKey(key),
                            SecurityAlgorithms.HmacSha256Signature
                        )
                };

            var tokenHandler =
                new JwtSecurityTokenHandler();

            var token =
                tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}