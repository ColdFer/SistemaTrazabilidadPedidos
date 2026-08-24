namespace TrazabilidadPedidos.Shared.Entities
{
    public class Customer
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Ci { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public User? User { get; set; }
    }
}