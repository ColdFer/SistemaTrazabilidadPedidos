using System.ComponentModel.DataAnnotations;

namespace TrazabilidadPedidos.Shared.DTOs.Users
{
    public class UpdateManagedUserRequest
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        [MaxLength(30)]
        public string? Phone { get; set; }

        public bool? IsAvailable { get; set; }

        [MinLength(6)]
        public string? NewPassword { get; set; }

        [Compare(nameof(NewPassword))]
        public string? ConfirmNewPassword { get; set; }
    }
}