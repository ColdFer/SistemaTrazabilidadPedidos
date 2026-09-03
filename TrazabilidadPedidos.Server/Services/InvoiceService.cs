using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TrazabilidadPedidos.Server.Data;
using TrazabilidadPedidos.Server.Repositories;
using TrazabilidadPedidos.Shared.Entities;

namespace TrazabilidadPedidos.Server.Services
{
    public interface IInvoiceService
    {
        Task<Invoice?> GetByIdAsync(int id);
        Task<Invoice?> GetByOrderIdAsync(int orderId);
        Task<List<Invoice>> GetAllAsync();
        Task<List<Invoice>> GetByCustomerIdAsync(int customerId);
        Task<Invoice?> GenerateForOrderAsync(int orderId);
        byte[] GeneratePdf(Invoice invoice);
    }

    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _repository;
        private readonly AppDbContext _context;

        public InvoiceService(IInvoiceRepository repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<Invoice?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Invoice?> GetByOrderIdAsync(int orderId)
        {
            return await _repository.GetByOrderIdAsync(orderId);
        }

        public async Task<List<Invoice>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<List<Invoice>> GetByCustomerIdAsync(int customerId)
        {
            return await _repository.GetByCustomerIdAsync(customerId);
        }

        public async Task<Invoice?> GenerateForOrderAsync(int orderId)
        {
            var existing = await _repository.GetByOrderIdAsync(orderId);
            if (existing != null) return existing;

            var order = await _context.Orders
                .Include(o => o.Customer)
                    .ThenInclude(c => c!.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null || order.Customer == null) return null;

            var code = $"FAC-{DateTime.Now:yyyy}-{await GetNextInvoiceNumber():D6}";

            var subtotal = order.OrderDetails.Sum(d => d.UnitPrice * d.Quantity);
            var ivaAmount = Math.Round(subtotal * 0.13m, 2);
            var total = subtotal + ivaAmount;

            var invoice = new Invoice
            {
                OrderId = orderId,
                Code = code,
                Nit = "00000000",
                RazonSocial = order.Customer.User != null
                    ? $"{order.Customer.User.FirstName} {order.Customer.User.LastName}"
                    : "Cliente",
                CustomerId = order.Customer.Id,
                Subtotal = subtotal,
                IvaRate = 0.13m,
                IvaAmount = ivaAmount,
                Total = total,
                InvoiceDate = DateTime.Now,
                CreatedAt = DateTime.Now,
                Details = order.OrderDetails.Select(d => new InvoiceDetail
                {
                    ProductId = d.ProductId,
                    ProductName = d.Product?.Name ?? string.Empty,
                    ProductCode = d.Product?.Code ?? string.Empty,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    Subtotal = d.UnitPrice * d.Quantity
                }).ToList()
            };

            await _repository.AddAsync(invoice);
            return invoice;
        }

        private async Task<int> GetNextInvoiceNumber()
        {
            var count = await _context.Invoices.CountAsync();
            return count + 1;
        }

        public byte[] GeneratePdf(Invoice invoice)
        {
            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    page.MarginVertical(30);
                    page.MarginHorizontal(40);

                    page.Header().Row(row =>
                    {
                        row.RelativeItem(2).Column(col =>
                        {
                            col.Item().Text("NOVATEC S.R.L.").Bold().FontSize(18).FontColor("#1a237e");
                            col.Item().Text("Venta de Tecnologia y Accesorios").FontSize(9).FontColor("#555555");
                            col.Item().Text("Santa Cruz de la Sierra, Bolivia").FontSize(9).FontColor("#555555");
                            col.Item().Text("NIT: 12345678").FontSize(9).FontColor("#555555");
                        });

                        row.RelativeItem().AlignRight().Column(col =>
                        {
                            col.Item().Text("FACTURA").Bold().FontSize(16).FontColor("#d32f2f");
                            col.Item().Text($"Nro: {invoice.Code}").FontSize(10).FontColor("#333333");
                            col.Item().Text($"Fecha: {invoice.InvoiceDate:dd/MM/yyyy}").FontSize(10).FontColor("#333333");
                        });
                    });

                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        col.Item().LineHorizontal(1).LineColor("#1a237e");

                        col.Item().PaddingTop(10).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("DATOS DEL CLIENTE").Bold().FontSize(11).FontColor("#1a237e");
                                c.Item().Text($"Razon Social: {invoice.RazonSocial}").FontSize(9);
                                c.Item().Text($"NIT/CI: {invoice.Nit}").FontSize(9);
                                if (!string.IsNullOrEmpty(invoice.Direccion))
                                    c.Item().Text($"Direccion: {invoice.Direccion}").FontSize(9);
                            });
                        });

                        col.Item().PaddingTop(15).LineHorizontal(0.5f).LineColor("#cccccc");

                        col.Item().PaddingTop(10).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(30);
                                columns.RelativeColumn(3);
                                columns.ConstantColumn(60);
                                columns.ConstantColumn(70);
                                columns.ConstantColumn(80);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background("#1a237e").Padding(5).Text("Nro").FontColor(Colors.White).Bold().FontSize(8);
                                header.Cell().Background("#1a237e").Padding(5).Text("Producto").FontColor(Colors.White).Bold().FontSize(8);
                                header.Cell().Background("#1a237e").Padding(5).Text("Cant.").FontColor(Colors.White).Bold().FontSize(8).AlignCenter();
                                header.Cell().Background("#1a237e").Padding(5).Text("P. Unit.").FontColor(Colors.White).Bold().FontSize(8).AlignRight();
                                header.Cell().Background("#1a237e").Padding(5).Text("Subtotal").FontColor(Colors.White).Bold().FontSize(8).AlignRight();
                            });

                            int num = 1;
                            foreach (var detail in invoice.Details)
                            {
                                var bgColor = num % 2 == 0 ? "#f5f5f5" : "#ffffff";
                                table.Cell().Background(bgColor).Padding(4).Text(num.ToString()).FontSize(8).AlignCenter();
                                table.Cell().Background(bgColor).Padding(4).Text(detail.ProductName).FontSize(8);
                                table.Cell().Background(bgColor).Padding(4).Text(detail.Quantity.ToString()).FontSize(8).AlignCenter();
                                table.Cell().Background(bgColor).Padding(4).Text($"Bs {detail.UnitPrice:N2}").FontSize(8).AlignRight();
                                table.Cell().Background(bgColor).Padding(4).Text($"Bs {detail.Subtotal:N2}").FontSize(8).AlignRight();
                                num++;
                            }
                        });

                        col.Item().PaddingTop(15).LineHorizontal(0.5f).LineColor("#cccccc");

                        col.Item().PaddingTop(10).AlignRight().Width(200).Column(total =>
                        {
                            total.Item().Row(r =>
                            {
                                r.RelativeItem().Text("Subtotal:").FontSize(10);
                                r.ConstantItem(100).AlignRight().Text($"Bs {invoice.Subtotal:N2}").FontSize(10);
                            });
                            total.Item().Row(r =>
                            {
                                r.RelativeItem().Text("IVA (13%):").FontSize(10);
                                r.ConstantItem(100).AlignRight().Text($"Bs {invoice.IvaAmount:N2}").FontSize(10);
                            });
                            total.Item().LineHorizontal(1).LineColor("#1a237e");
                            total.Item().PaddingTop(5).Row(r =>
                            {
                                r.RelativeItem().Text("TOTAL:").Bold().FontSize(12).FontColor("#1a237e");
                                r.ConstantItem(100).AlignRight().Text($"Bs {invoice.Total:N2}").Bold().FontSize(12).FontColor("#1a237e");
                            });
                        });

                        col.Item().PaddingTop(30).AlignCenter().Text("Gracias por su compra!").Bold().FontSize(10).FontColor("#1a237e");
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("NovaTec S.R.L. - Santa Cruz de la Sierra - ").FontSize(8).FontColor("#999999");
                        text.CurrentPageNumber().FontSize(8).FontColor("#999999");
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}
