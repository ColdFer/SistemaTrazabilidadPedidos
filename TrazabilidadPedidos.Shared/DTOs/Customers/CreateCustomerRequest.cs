using System.ComponentModel.DataAnnotations;

namespace TrazabilidadPedidos.Shared.DTOs.Customers
{
    public class CreateCustomerRequest
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Ci { get; set; } = string.Empty;

        [Required]
        [Phone]
        [MaxLength(30)]
        public string Phone { get; set; } = string.Empty;
    }
}