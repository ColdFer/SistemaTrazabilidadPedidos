namespace TrazabilidadPedidos.Shared.DTOs.Users
{
    public class ManagedUserDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string FullName =>
            $"{FirstName} {LastName}";

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        // Solo se utiliza cuando el usuario es repartidor.
        public string? Phone { get; set; }

        public bool? IsAvailable { get; set; }
    }
}