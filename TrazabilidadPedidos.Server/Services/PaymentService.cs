using Microsoft.EntityFrameworkCore;
using TrazabilidadPedidos.Server.Data;
using TrazabilidadPedidos.Server.Repositories;
using TrazabilidadPedidos.Shared.DTOs.Payments;
using TrazabilidadPedidos.Shared.Entities;
using TrazabilidadPedidos.Shared.Enums;

namespace TrazabilidadPedidos.Server.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;

        public PaymentService(
            IPaymentRepository paymentRepository,
            AppDbContext context,
            INotificationService notificationService)
        {
            _paymentRepository = paymentRepository;
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<PaymentDto?> GetByIdAsync(int id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            return payment != null ? MapToDto(payment) : null;
        }

        public async Task<List<PaymentDto>> GetAllAsync()
        {
            var payments = await _paymentRepository.GetAllAsync();
            return payments.Select(MapToDto).ToList();
        }

        public async Task<List<PaymentDto>> GetPendingAsync()
        {
            var payments = await _paymentRepository.GetPendingAsync();
            return payments.Select(MapToDto).ToList();
        }

        public async Task<List<PaymentDto>> GetByCustomerIdAsync(int customerId)
        {
            var payments = await _paymentRepository.GetByCustomerIdAsync(customerId);
            return payments.Select(MapToDto).ToList();
        }

        public async Task<PaymentDto?> CreateAsync(CreatePaymentRequest request, int userId)
        {
            var order = await _context.Orders.FindAsync(request.OrderId);
            if (order == null) return null;

            var existingPayment = await _paymentRepository.GetByOrderIdAsync(request.OrderId);
            if (existingPayment != null) return null;

            var method = (PaymentMethod)request.Method;

            var payment = new Payment
            {
                OrderId = request.OrderId,
                Method = method,
                Amount = request.Amount,
                ProofImage = request.ProofImage,
                PaymentDate = DateTime.Now,
                Status = method == PaymentMethod.BankTransfer || method == PaymentMethod.Cash || method == PaymentMethod.Card
                    ? PaymentStatus.Confirmed
                    : PaymentStatus.Pending,
                TransactionCode = $"TXN-{DateTime.Now:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _paymentRepository.AddAsync(payment);
            return MapToDto(payment);
        }

        public async Task<PaymentDto?> VerifyAsync(
            int paymentId, VerifyPaymentRequest request, int userId)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null) return null;

            payment.Status = (PaymentStatus)request.Status;
            payment.VerifiedByUserId = userId;
            payment.Observation = request.Observation;
            payment.UpdatedAt = DateTime.Now;

            await _paymentRepository.SaveChangesAsync();

            var order = await _context.Orders.FindAsync(payment.OrderId);
            if (order != null)
            {
                var customerUserId = await _context.Customers
                    .Where(c => c.Id == order.CustomerId)
                    .Select(c => c.UserId)
                    .FirstOrDefaultAsync();

                if (customerUserId > 0)
                {
                    var status = (PaymentStatus)request.Status;
                    var (title, msg, type) = status switch
                    {
                        PaymentStatus.Confirmed => ("Pago confirmado", $"Tu pago del pedido {order.Code} fue confirmado.", "PagoConfirmado"),
                        PaymentStatus.Rejected => ("Pago rechazado", $"Tu pago del pedido {order.Code} fue rechazado. Contacta soporte.", "PagoRechazado"),
                        _ => ("", "", "")
                    };
                    if (!string.IsNullOrEmpty(title))
                        await _notificationService.CreateAsync(customerUserId, title, msg, type, order.Id);
                }
            }

            return MapToDto(payment);
        }

        private static PaymentDto MapToDto(Payment payment)
        {
            return new PaymentDto
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                OrderCode = payment.Order?.Code ?? string.Empty,
                TransactionCode = payment.TransactionCode,
                Method = payment.Method.ToString(),
                Amount = payment.Amount,
                Receipt = payment.Receipt,
                ProofImage = payment.ProofImage,
                PaymentDate = payment.PaymentDate,
                Status = payment.Status.ToString(),
                Observation = payment.Observation,
                CreatedAt = payment.CreatedAt
            };
        }
    }
}
