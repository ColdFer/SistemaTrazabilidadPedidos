using TrazabilidadPedidos.Server.Data;
using TrazabilidadPedidos.Server.Repositories;
using TrazabilidadPedidos.Shared.DTOs.Orders;
using TrazabilidadPedidos.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace TrazabilidadPedidos.Server.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;

        public OrderService(
            IOrderRepository orderRepository,
            AppDbContext context,
            INotificationService notificationService)
        {
            _orderRepository = orderRepository;
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<List<OrderDto>> GetAllAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            return orders.Select(MapToDto).ToList();
        }

        public async Task<List<OrderDto>> GetByCustomerIdAsync(int customerId)
        {
            var orders = await _orderRepository.GetByCustomerIdAsync(customerId);
            return orders.Select(MapToDto).ToList();
        }

        public async Task<OrderDto?> GetByIdAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return null;
            return MapToDto(order);
        }

        public async Task<OrderDto?> CreateAsync(CreateOrderRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var pendingStatus = await _orderRepository.GetStatusByNameAsync("Pendiente");
                if (pendingStatus == null)
                {
                    await transaction.RollbackAsync();
                    return null;
                }

                var order = new Order
                {
                    CustomerId = request.CustomerId,
                    Code = GenerateOrderCode(),
                    OrderDate = DateTime.Now,
                    CurrentStatusId = pendingStatus.Id,
                    Observation = request.Observation,
                    DeliveryLatitude = request.DeliveryLatitude,
                    DeliveryLongitude = request.DeliveryLongitude,
                    DeliveryAddress = request.DeliveryAddress,
                    DeliveryReference = request.DeliveryReference,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                decimal total = 0;

                foreach (var item in request.Items)
                {
                    var product = await _context.Products
                        .FirstOrDefaultAsync(p => p.Id == item.ProductId);

                    if (product == null || product.CurrentStock < item.Quantity)
                    {
                        await transaction.RollbackAsync();
                        return null;
                    }

                    var detail = new OrderDetail
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price
                    };

                    order.OrderDetails.Add(detail);
                    total += product.Price * item.Quantity;
                }

                order.Total = total;

                await _orderRepository.AddAsync(order);
                await _context.SaveChangesAsync();

                var history = new OrderStatusHistory
                {
                    OrderId = order.Id,
                    OrderStatusId = pendingStatus.Id,
                    UserId = await GetUserIdFromCustomerAsync(request.CustomerId),
                    StatusDate = DateTime.Now,
                    Observation = "Pedido creado"
                };

                await _orderRepository.AddStatusHistoryAsync(history);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var operatorUserIds = await _context.Users
                    .Where(u => u.Role != null && (u.Role.Name == "Administrator" || u.Role.Name == "Operator"))
                    .Select(u => u.Id)
                    .ToListAsync();

                foreach (var uid in operatorUserIds)
                {
                    await _notificationService.CreateAsync(uid,
                        "Nuevo pedido",
                        $"Se registro un nuevo pedido {order.Code} por Bs {total:N2}.",
                        "NuevoPedido", order.Id);
                }

                return await GetByIdAsync(order.Id);
            }
            catch
            {
                await transaction.RollbackAsync();
                return null;
            }
        }

        public async Task<bool> AcceptOrderAsync(int orderId, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null) return false;

                var acceptedStatus = await _orderRepository.GetStatusByNameAsync("Aceptado");
                if (acceptedStatus == null) return false;

                foreach (var detail in order.OrderDetails)
                {
                    var product = await _context.Products
                        .FirstOrDefaultAsync(p => p.Id == detail.ProductId);

                    if (product == null || product.CurrentStock < detail.Quantity)
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }

                    product.CurrentStock -= detail.Quantity;
                    product.UpdatedAt = DateTime.Now;
                }

                order.CurrentStatusId = acceptedStatus.Id;
                order.UpdatedAt = DateTime.Now;

                var history = new OrderStatusHistory
                {
                    OrderId = orderId,
                    OrderStatusId = acceptedStatus.Id,
                    UserId = userId,
                    StatusDate = DateTime.Now,
                    Observation = "Pedido aceptado. Inventario descontado."
                };

                await _orderRepository.AddStatusHistoryAsync(history);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var customerUserId = await GetUserIdFromCustomerAsync(order.CustomerId);
                if (customerUserId > 0)
                {
                    await _notificationService.CreateAsync(customerUserId,
                        "Pedido aceptado",
                        $"Tu pedido {order.Code} fue aceptado y esta en preparacion.",
                        "PedidoAceptado", orderId);
                }

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> UpdateStatusAsync(int orderId, int statusId, string? observation, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null) return false;

                var status = await _context.OrderStatuses
                    .FirstOrDefaultAsync(s => s.Id == statusId);

                if (status == null) return false;

                var previousStatusId = order.CurrentStatusId;

                order.CurrentStatusId = statusId;
                order.UpdatedAt = DateTime.Now;

                if (status.Name == "Cancelado")
                {
                    var acceptedStatus = await _orderRepository.GetStatusByNameAsync("Aceptado");
                    if (acceptedStatus != null && previousStatusId >= acceptedStatus.Id)
                    {
                        foreach (var detail in order.OrderDetails)
                        {
                            var product = await _context.Products
                                .FirstOrDefaultAsync(p => p.Id == detail.ProductId);
                            if (product != null)
                            {
                                product.CurrentStock += detail.Quantity;
                                product.UpdatedAt = DateTime.Now;
                            }
                        }
                    }
                }

                var history = new OrderStatusHistory
                {
                    OrderId = orderId,
                    OrderStatusId = statusId,
                    UserId = userId,
                    StatusDate = DateTime.Now,
                    Observation = observation
                };

                await _orderRepository.AddStatusHistoryAsync(history);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                var customerUserId = await GetUserIdFromCustomerAsync(order.CustomerId);
                if (customerUserId > 0)
                {
                    var (notifTitle, notifMsg, notifType) = status.Name switch
                    {
                        "Preparando" => ("Pedido en preparacion", $"Tu pedido {order.Code} esta siendo preparado.", "PedidoPreparando"),
                        "ListoParaEntrega" => ("Pedido listo", $"Tu pedido {order.Code} esta listo para entrega.", "PedidoListo"),
                        "Entregado" => ("Pedido entregado", $"Tu pedido {order.Code} fue entregado. Factura disponible.", "PedidoEntregado"),
                        "Cancelado" => ("Pedido cancelado", $"Tu pedido {order.Code} fue cancelado.", "PedidoCancelado"),
                        _ => ("", "", "")
                    };

                    if (!string.IsNullOrEmpty(notifTitle))
                    {
                        await _notificationService.CreateAsync(customerUserId,
                            notifTitle, notifMsg, notifType, orderId);
                    }
                }

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<List<OrderStatusDto>> GetStatusesAsync()
        {
            var statuses = await _orderRepository.GetStatusesAsync();
            return statuses.Select(s => new OrderStatusDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                SortOrder = s.SortOrder
            }).ToList();
        }

        private static OrderDto MapToDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer?.User != null
                    ? $"{order.Customer.User.FirstName} {order.Customer.User.LastName}"
                    : string.Empty,
                Code = order.Code,
                OrderDate = order.OrderDate,
                Total = order.Total,
                StatusName = order.CurrentStatus?.Name ?? string.Empty,
                Observation = order.Observation,
                DeliveryLatitude = order.DeliveryLatitude,
                DeliveryLongitude = order.DeliveryLongitude,
                DeliveryAddress = order.DeliveryAddress,
                DeliveryReference = order.DeliveryReference,
                Details = order.OrderDetails.Select(d => new OrderDetailDto
                {
                    Id = d.Id,
                    ProductId = d.ProductId,
                    ProductName = d.Product?.Name ?? string.Empty,
                    ProductCode = d.Product?.Code ?? string.Empty,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice
                }).ToList()
            };
        }

        private static string GenerateOrderCode()
        {
            return $"PED-{DateTime.Now:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}";
        }

        private async Task<int> GetUserIdFromCustomerAsync(int customerId)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == customerId);
            return customer?.UserId ?? 0;
        }
    }
}
