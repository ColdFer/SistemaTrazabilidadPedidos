namespace TrazabilidadPedidos.Shared.DTOs.Customers
{
    public class CustomerDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}";

        public string Email { get; set; } = string.Empty;

        public string Ci { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}