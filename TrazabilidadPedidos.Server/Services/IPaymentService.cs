using TrazabilidadPedidos.Shared.DTOs.Payments;

namespace TrazabilidadPedidos.Server.Services
{
    public interface IPaymentService
    {
        Task<PaymentDto?> GetByIdAsync(int id);
        Task<List<PaymentDto>> GetAllAsync();
        Task<List<PaymentDto>> GetPendingAsync();
        Task<List<PaymentDto>> GetByCustomerIdAsync(int customerId);
        Task<PaymentDto?> CreateAsync(CreatePaymentRequest request, int userId);
        Task<PaymentDto?> VerifyAsync(int paymentId, VerifyPaymentRequest request, int userId);
    }
}
