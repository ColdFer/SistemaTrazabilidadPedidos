using Microsoft.EntityFrameworkCore;
using TrazabilidadPedidos.Server.Data;
using TrazabilidadPedidos.Server.Repositories;
using TrazabilidadPedidos.Shared.DTOs.Dispatches;
using TrazabilidadPedidos.Shared.Entities;
using TrazabilidadPedidos.Shared.Enums;

namespace TrazabilidadPedidos.Server.Services
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IDeliveryRepository _deliveryRepository;
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;

        public DeliveryService(
            IDeliveryRepository deliveryRepository,
            AppDbContext context,
            INotificationService notificationService)
        {
            _deliveryRepository = deliveryRepository;
            _context = context;
            _notificationService = notificationService;
        }


        // =====================================================
        // DESPACHOS
        // =====================================================

        public async Task<List<DeliveryDto>> GetAllAsync()
        {
            var deliveries =
                await _deliveryRepository.GetAllAsync();

            return deliveries
                .Select(MapDeliveryToDto)
                .ToList();
        }


        public async Task<DeliveryDto?> GetByIdAsync(int id)
        {
            var delivery =
                await _deliveryRepository.GetByIdAsync(id);

            if (delivery == null)
                return null;

            return MapDeliveryToDto(delivery);
        }


        // =====================================================
        // DATOS PARA SELECTORES
        // =====================================================

        public async Task<List<DeliveryOrderDto>> GetOrdersAsync()
        {
            var orders =
                await _deliveryRepository.GetOrdersAsync();

            return orders
                .Select(order => new DeliveryOrderDto
                {
                    Id = order.Id,

                    CustomerId = order.CustomerId,

                    Code = order.Code,

                    CustomerName = order.Customer?.User == null
                        ? string.Empty
                        : $"{order.Customer.User.FirstName} {order.Customer.User.LastName}",

                    Total = order.Total,

                    DeliveryAddress = order.DeliveryAddress,

                    DeliveryReference = order.DeliveryReference,

                    CustomerPhone = order.Customer?.Phone ?? "",

                    CustomerFullName = order.Customer?.User == null
                        ? string.Empty
                        : $"{order.Customer.User.FirstName} {order.Customer.User.LastName}",

                    DeliveryLatitude = order.DeliveryLatitude,

                    DeliveryLongitude = order.DeliveryLongitude
                })
                .ToList();
        }


        public async Task<List<DeliveryAddressDto>>
            GetAddressesAsync()
        {
            var addresses =
                await _deliveryRepository.GetAddressesAsync();

            return addresses
                .Select(address => new DeliveryAddressDto
                {
                    Id = address.Id,

                    CustomerId = address.CustomerId,

                    AddressLine = address.AddressLine,

                    Reference = address.Reference,

                    Label = address.Label
                })
                .ToList();
        }


        public async Task<List<DeliveryDriverDto>>
            GetDriversAsync()
        {
            var drivers =
                await _deliveryRepository.GetDriversAsync();

            return drivers
                .Select(driver => new DeliveryDriverDto
                {
                    Id = driver.Id,

                    UserId = driver.UserId,

                    FullName = driver.User == null
                        ? string.Empty
                        : $"{driver.User.FirstName} {driver.User.LastName}",

                    Phone = driver.Phone,

                    IsAvailable = driver.IsAvailable
                })
                .ToList();
        }


        // =====================================================
        // CREAR DESPACHO
        // =====================================================

        public async Task<DeliveryDto?> CreateAsync(
            CreateDeliveryRequest request)
        {
            var order =
                await _deliveryRepository
                    .GetOrderByIdAsync(request.OrderId);

            if (order == null)
                return null;


            // Un pedido no debe tener dos despachos.
            if (await _deliveryRepository
                .DeliveryExistsForOrderAsync(order.Id))
            {
                return null;
            }


            Shared.Entities.Address? address = null;

            if (request.AddressId.HasValue && request.AddressId.Value > 0)
            {
                address =
                    await _deliveryRepository
                        .GetAddressByIdAsync(request.AddressId.Value);

                if (address == null || !address.IsActive)
                    return null;

                if (address.CustomerId != order.CustomerId)
                    return null;
            }


            DeliveryDriver? driver = null;

            if (request.DeliveryDriverId.HasValue)
            {
                driver =
                    await _deliveryRepository.GetDriverByIdAsync(
                        request.DeliveryDriverId.Value);

                if (driver == null)
                    return null;

                if (!driver.IsAvailable)
                    return null;
            }


            var now = DateTime.Now;

            var delivery = new Delivery
            {
                OrderId = order.Id,
                Order = order,

                AddressId = address?.Id,
                Address = address,

                DeliveryDriverId = driver?.Id,
                DeliveryDriver = driver,

                ScheduledDate = request.ScheduledDate,

                StartTime = request.StartTime,
                EndTime = request.EndTime,

                ContactPhone = request.ContactPhone.Trim(),

                RecipientName =
                    request.RecipientName.Trim(),

                Status = DeliveryStatus.Pending,

                Observation =
                    request.Observation?.Trim(),

                DepartureDate = null,
                DeliveredDate = null,

                CreatedAt = now,
                UpdatedAt = now
            };


            await _deliveryRepository.AddAsync(delivery);

            await _deliveryRepository.SaveChangesAsync();


            // Volvemos a consultarlo para recuperar
            // todas las relaciones cargadas.
            var created =
                await _deliveryRepository.GetByIdAsync(
                    delivery.Id);

            return created == null
                ? null
                : MapDeliveryToDto(created);
        }


        // =====================================================
        // EDITAR PROGRAMACIÓN
        // =====================================================

        public async Task<DeliveryDto?> UpdateAsync(
            int id,
            UpdateDeliveryRequest request)
        {
            var delivery =
                await _deliveryRepository.GetByIdAsync(id);

            if (delivery == null)
                return null;


            var address =
                await _deliveryRepository
                    .GetAddressByIdAsync(request.AddressId);

            if (address == null || !address.IsActive)
                return null;


            // La nueva dirección debe pertenecer
            // al mismo cliente del pedido.
            if (delivery.Order == null ||
                address.CustomerId != delivery.Order.CustomerId)
            {
                return null;
            }


            DeliveryDriver? driver = null;

            if (request.DeliveryDriverId.HasValue)
            {
                driver =
                    await _deliveryRepository.GetDriverByIdAsync(
                        request.DeliveryDriverId.Value);

                if (driver == null)
                    return null;

                /*
                 * Si ya es el repartidor de este despacho,
                 * permitimos mantenerlo aunque actualmente
                 * aparezca como no disponible.
                 */
                if (!driver.IsAvailable &&
                    delivery.DeliveryDriverId != driver.Id)
                {
                    return null;
                }
            }


            delivery.AddressId = address.Id;
            delivery.Address = address;

            delivery.DeliveryDriverId = driver?.Id;
            delivery.DeliveryDriver = driver;

            delivery.ScheduledDate =
                request.ScheduledDate;

            delivery.StartTime =
                request.StartTime;

            delivery.EndTime =
                request.EndTime;

            delivery.ContactPhone =
                request.ContactPhone.Trim();

            delivery.RecipientName =
                request.RecipientName.Trim();

            delivery.Observation =
                request.Observation?.Trim();

            delivery.UpdatedAt =
                DateTime.Now;


            await _deliveryRepository.SaveChangesAsync();


            var updated =
                await _deliveryRepository.GetByIdAsync(id);

            return updated == null
                ? null
                : MapDeliveryToDto(updated);
        }


        // =====================================================
        // CAMBIAR ESTADO
        // =====================================================

        public async Task<DeliveryDto?> ChangeStatusAsync(
            int id,
            ChangeDeliveryStatusRequest request)
        {
            var delivery =
                await _deliveryRepository.GetByIdAsync(id);

            if (delivery == null)
                return null;


            if (!Enum.IsDefined(
                typeof(DeliveryStatus),
                request.Status))
            {
                return null;
            }


            var newStatus =
                (DeliveryStatus)request.Status;


            /*
             * Para Assigned e InRoute debe existir
             * un repartidor asignado.
             */
            if ((newStatus == DeliveryStatus.Assigned ||
                 newStatus == DeliveryStatus.InRoute ||
                 newStatus == DeliveryStatus.Delivered)
                &&
                !delivery.DeliveryDriverId.HasValue)
            {
                throw new InvalidOperationException(
                    "Debe asignar un repartidor antes de utilizar este estado.");
            }


            delivery.Status = newStatus;

            delivery.Observation =
                request.Observation?.Trim()
                ?? delivery.Observation;

            delivery.UpdatedAt = DateTime.Now;


            // Cuando sale a reparto registramos fecha de salida
            // y actualizamos el estado del pedido a EnCamino.
            if (newStatus == DeliveryStatus.InRoute)
            {
                delivery.DepartureDate ??= DateTime.Now;

                if (delivery.DeliveryDriver != null)
                {
                    delivery.DeliveryDriver.IsAvailable = false;
                    delivery.DeliveryDriver.UpdatedAt = DateTime.Now;
                }

                var orderForStatus = await _context.Orders
                    .FirstOrDefaultAsync(o => o.Id == delivery.OrderId);
                if (orderForStatus != null)
                {
                    var enCaminoStatus = await _context.OrderStatuses
                        .FirstOrDefaultAsync(s => s.Name == "EnCamino");
                    if (enCaminoStatus != null)
                    {
                        orderForStatus.CurrentStatusId = enCaminoStatus.Id;
                        orderForStatus.UpdatedAt = DateTime.Now;

                        var historyEntry = new OrderStatusHistory
                        {
                            OrderId = delivery.OrderId,
                            OrderStatusId = enCaminoStatus.Id,
                            UserId = delivery.DeliveryDriver?.UserId ?? 0,
                            StatusDate = DateTime.Now,
                            Observation = "Pedido en camino"
                        };
                        _context.OrderStatusHistories.Add(historyEntry);
                    }
                }
            }


            // Cuando se entrega registramos la fecha.
            if (newStatus == DeliveryStatus.Delivered)
            {
                delivery.DeliveredDate = DateTime.Now;

                if (delivery.DeliveryDriver != null)
                {
                    delivery.DeliveryDriver.IsAvailable = true;
                    delivery.DeliveryDriver.UpdatedAt = DateTime.Now;
                }

                var order = await _context.Orders
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Product)
                    .Include(o => o.Customer)
                        .ThenInclude(c => c!.User)
                    .FirstOrDefaultAsync(o => o.Id == delivery.OrderId);

                if (order != null)
                {
                    var deliveredStatus = await _context.OrderStatuses
                        .FirstOrDefaultAsync(s => s.Name == "Entregado");
                    if (deliveredStatus != null)
                    {
                        order.CurrentStatusId = deliveredStatus.Id;
                        order.UpdatedAt = DateTime.Now;

                        var deliveredHistory = new OrderStatusHistory
                        {
                            OrderId = order.Id,
                            OrderStatusId = deliveredStatus.Id,
                            UserId = delivery.DeliveryDriver?.UserId ?? 0,
                            StatusDate = DateTime.Now,
                            Observation = "Pedido entregado"
                        };
                        _context.OrderStatusHistories.Add(deliveredHistory);
                    }

                    var existingInvoice = await _context.Invoices
                        .FirstOrDefaultAsync(i => i.OrderId == order.Id);
                    if (existingInvoice == null)
                    {
                        var nit = "0";
                        var razonSocial = "Cliente General";
                        var direccion = delivery.Address?.AddressLine ?? "";

                        if (order.Customer?.User != null)
                        {
                            razonSocial = $"{order.Customer.User.FirstName} {order.Customer.User.LastName}";
                        }

                        var subtotal = order.OrderDetails.Sum(d => d.UnitPrice * d.Quantity);
                        var ivaAmount = Math.Round(subtotal * 0.13m, 2);
                        var total = subtotal + ivaAmount;

                        var invoice = new Invoice
                        {
                            OrderId = order.Id,
                            Code = $"FAC-{DateTime.Now:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}",
                            Nit = nit,
                            RazonSocial = razonSocial,
                            Direccion = direccion,
                            CustomerId = order.CustomerId,
                            Subtotal = subtotal,
                            IvaRate = 0.13m,
                            IvaAmount = ivaAmount,
                            Total = total,
                            InvoiceDate = DateTime.Now,
                            CreatedAt = DateTime.Now
                        };

                        _context.Invoices.Add(invoice);

                        foreach (var detail in order.OrderDetails)
                        {
                            var invoiceDetail = new InvoiceDetail
                            {
                                ProductId = detail.ProductId,
                                ProductName = detail.Product?.Name ?? "",
                                ProductCode = detail.Product?.Code ?? "",
                                Quantity = detail.Quantity,
                                UnitPrice = detail.UnitPrice,
                                Subtotal = detail.UnitPrice * detail.Quantity
                            };
                            invoice.Details.Add(invoiceDetail);
                        }
                    }
                }
            }


            // Si falla o se cancela, liberamos al repartidor.
            if (newStatus == DeliveryStatus.Failed ||
                newStatus == DeliveryStatus.Cancelled)
            {
                if (delivery.DeliveryDriver != null)
                {
                    delivery.DeliveryDriver.IsAvailable = true;
                    delivery.DeliveryDriver.UpdatedAt = DateTime.Now;
                }
            }


            await _deliveryRepository.SaveChangesAsync();

            var deliveryOrder = await _context.Orders.FindAsync(delivery.OrderId);
            if (deliveryOrder != null)
            {
                var customerUserId = await _context.Customers
                    .Where(c => c.Id == deliveryOrder.CustomerId)
                    .Select(c => c.UserId)
                    .FirstOrDefaultAsync();

                if (customerUserId > 0)
                {
                    var (title, msg, type) = newStatus switch
                    {
                        DeliveryStatus.InRoute => ("Despacho en ruta", $"Tu pedido {deliveryOrder.Code} esta en camino.", "DespachoEnRuta"),
                        DeliveryStatus.Delivered => ("Pedido entregado", $"Tu pedido {deliveryOrder.Code} fue entregado.", "PedidoEntregado"),
                        DeliveryStatus.Failed => ("Entrega fallida", $"La entrega del pedido {deliveryOrder.Code} fallo.", "EntregaFallida"),
                        DeliveryStatus.Cancelled => ("Despacho cancelado", $"El despacho del pedido {deliveryOrder.Code} fue cancelado.", "DespachoCancelado"),
                        _ => ("", "", "")
                    };
                    if (!string.IsNullOrEmpty(title))
                        await _notificationService.CreateAsync(customerUserId, title, msg, type, deliveryOrder.Id);
                }
            }


            var updated =
                await _deliveryRepository.GetByIdAsync(id);

            return updated == null
                ? null
                : MapDeliveryToDto(updated);
        }


        // =====================================================
        // MAPEO
        // =====================================================

        private static DeliveryDto MapDeliveryToDto(
            Delivery delivery)
        {
            var driverName =
                delivery.DeliveryDriver?.User == null
                    ? string.Empty
                    : $"{delivery.DeliveryDriver.User.FirstName} {delivery.DeliveryDriver.User.LastName}";

            var addressText =
                delivery.Address?.AddressLine
                ?? delivery.Order?.DeliveryAddress
                ?? string.Empty;

            return new DeliveryDto
            {
                Id = delivery.Id,

                OrderId = delivery.OrderId,

                OrderCode =
                    delivery.Order?.Code
                    ?? string.Empty,
                CustomerName =
                    delivery.Order?.Customer?.User == null
                        ? string.Empty
                        : $"{delivery.Order.Customer.User.FirstName} {delivery.Order.Customer.User.LastName}",

                AddressId = delivery.AddressId,

                Address = addressText,

                DeliveryDriverId =
                    delivery.DeliveryDriverId,

                DeliveryDriverName =
                    driverName,

                ScheduledDate =
                    delivery.ScheduledDate,

                StartTime =
                    delivery.StartTime,

                EndTime =
                    delivery.EndTime,

                ContactPhone =
                    delivery.ContactPhone,

                RecipientName =
                    delivery.RecipientName,

                Status =
                    (int)delivery.Status,

                StatusName =
                    GetStatusName(delivery.Status),

                DepartureDate =
                    delivery.DepartureDate,

                DeliveredDate =
                    delivery.DeliveredDate,

                Observation =
                    delivery.Observation
            };
        }


        private static string GetStatusName(
            DeliveryStatus status)
        {
            return status switch
            {
                DeliveryStatus.Pending =>
                    "Pendiente",

                DeliveryStatus.Scheduled =>
                    "Programado",

                DeliveryStatus.Assigned =>
                    "Asignado",

                DeliveryStatus.InRoute =>
                    "En ruta",

                DeliveryStatus.Delivered =>
                    "Entregado",

                DeliveryStatus.Failed =>
                    "Fallido",

                DeliveryStatus.Cancelled =>
                    "Cancelado",

                _ => "Desconocido"
            };
        }
    }
}