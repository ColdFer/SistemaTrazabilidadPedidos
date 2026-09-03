namespace TrazabilidadPedidos.Shared.Entities
{
    public class Invoice
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public string Code { get; set; } = string.Empty;

        public string Nit { get; set; } = string.Empty;

        public string RazonSocial { get; set; } = string.Empty;

        public string? Direccion { get; set; }

        public int CustomerId { get; set; }

        public decimal Subtotal { get; set; }

        public decimal IvaRate { get; set; } = 0.13m;

        public decimal IvaAmount { get; set; }

        public decimal Total { get; set; }

        public DateTime InvoiceDate { get; set; } = DateTime.Now;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Order? Order { get; set; }

        public Customer? Customer { get; set; }

        public ICollection<InvoiceDetail> Details { get; set; } = new List<InvoiceDetail>();
    }

    public class InvoiceDetail
    {
        public int Id { get; set; }

        public int InvoiceId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string ProductCode { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Subtotal { get; set; }

        public Invoice? Invoice { get; set; }

        public Product? Product { get; set; }
    }
}
