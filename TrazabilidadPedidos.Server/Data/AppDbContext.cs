using Microsoft.EntityFrameworkCore;
using TrazabilidadPedidos.Shared.Entities;

namespace TrazabilidadPedidos.Server.Data
{
    public class AppDbContext(DbContextOptions options) : 
        DbContext(options)
    {
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();
        public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<DeliveryDriver> DeliveryDrivers => Set<DeliveryDriver>();
        public DbSet<Address> Addresses => Set<Address>();
        public DbSet<OrderStatus> OrderStatuses => Set<OrderStatus>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Delivery> Deliveries => Set<Delivery>();
        public DbSet<Incident> Incidents => Set<Incident>();
        public DbSet<OrderStatusHistory> OrderStatusHistories =>
            Set<OrderStatusHistory>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Description)
                    .HasMaxLength(255);

                entity.HasIndex(e => e.Name)
                    .IsUnique();
            });
            modelBuilder.Entity<Permission>().HasData(
                new
                {
                    Id = 1,
                    Name = "ManageUsers",
                    Description = "Create, update and manage system users"
                },
                new
                {
                    Id = 2,
                    Name = "ManageCatalog",
                    Description = "Manage categories and products"
                },
                new
                {
                    Id = 3,
                    Name = "ManageInventory",
                    Description = "Manage stock and inventory movements"
                },
                new
                {
                    Id = 4,
                    Name = "ManageOrders",
                    Description = "Manage customer orders"
                },
                new
                {
                    Id = 5,
                    Name = "VerifyPayments",
                    Description = "Review and verify customer payments"
                },
                new
                {
                    Id = 6,
                    Name = "ScheduleDeliveries",
                    Description = "Schedule and reschedule deliveries"
                },
                new
                {
                    Id = 7,
                    Name = "AssignDeliveryDrivers",
                    Description = "Assign delivery drivers to deliveries"
                },
                new
                {
                    Id = 8,
                    Name = "ViewAssignedDeliveries",
                    Description = "View assigned deliveries"
                },
                new
                {
                    Id = 9,
                    Name = "UpdateDeliveryStatus",
                    Description = "Update the status of assigned deliveries"
                },
                new
                {
                    Id = 10,
                    Name = "RegisterIncidents",
                    Description = "Register incidents during deliveries"
                },
                new
                {
                    Id = 11,
                    Name = "CreateOrders",
                    Description = "Create customer orders"
                },
                new
                {
                    Id = 12,
                    Name = "ViewOwnOrders",
                    Description = "View the authenticated customer's orders"
                },
                new
                {
                    Id = 13,
                    Name = "ViewOrderTraceability",
                    Description = "View order status and traceability history"
                },
                new
                {
                    Id = 14,
                    Name = "ManageAddresses",
                    Description = "Manage customer delivery addresses"
                },
                new
                {
                    Id = 15,
                    Name = "GenerateReports",
                    Description = "Generate system reports"
                }
            );
            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(e => new
                {
                    e.RoleId,
                    e.PermissionId
                });

                entity.HasOne(e => e.Role)
                    .WithMany(r => r.RolePermissions)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Permission)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(e => e.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<RolePermission>().HasData(

                // Administrator
                new RolePermission { RoleId = 1, PermissionId = 1 },
                new RolePermission { RoleId = 1, PermissionId = 2 },
                new RolePermission { RoleId = 1, PermissionId = 3 },
                new RolePermission { RoleId = 1, PermissionId = 4 },
                new RolePermission { RoleId = 1, PermissionId = 5 },
                new RolePermission { RoleId = 1, PermissionId = 6 },
                new RolePermission { RoleId = 1, PermissionId = 7 },
                new RolePermission { RoleId = 1, PermissionId = 8 },
                new RolePermission { RoleId = 1, PermissionId = 9 },
                new RolePermission { RoleId = 1, PermissionId = 10 },
                new RolePermission { RoleId = 1, PermissionId = 11 },
                new RolePermission { RoleId = 1, PermissionId = 12 },
                new RolePermission { RoleId = 1, PermissionId = 13 },
                new RolePermission { RoleId = 1, PermissionId = 14 },
                new RolePermission { RoleId = 1, PermissionId = 15 },

                // Operator
                new RolePermission { RoleId = 2, PermissionId = 2 },
                new RolePermission { RoleId = 2, PermissionId = 3 },
                new RolePermission { RoleId = 2, PermissionId = 4 },
                new RolePermission { RoleId = 2, PermissionId = 5 },
                new RolePermission { RoleId = 2, PermissionId = 6 },
                new RolePermission { RoleId = 2, PermissionId = 7 },
                new RolePermission { RoleId = 2, PermissionId = 13 },

                // DeliveryDriver
                new RolePermission { RoleId = 3, PermissionId = 8 },
                new RolePermission { RoleId = 3, PermissionId = 9 },
                new RolePermission { RoleId = 3, PermissionId = 10 },

                // Customer
                new RolePermission { RoleId = 4, PermissionId = 11 },
                new RolePermission { RoleId = 4, PermissionId = 12 },
                new RolePermission { RoleId = 4, PermissionId = 13 },
                new RolePermission { RoleId = 4, PermissionId = 14 }
            );
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .HasMaxLength(255);

                entity.Property(e => e.IsActive)
                    .IsRequired();

                entity.HasIndex(e => e.Name)
                    .IsUnique();
            });
            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    Id = 1,
                    Name = "Administrator",
                    Description = "Full system administration",
                    IsActive = true
                },
                new Role
                {
                    Id = 2,
                    Name = "Operator",
                    Description = "Manages orders, payments and deliveries",
                    IsActive = true
                },
                new Role
                {
                    Id = 3,
                    Name = "DeliveryDriver",
                    Description = "Manages assigned deliveries",
                    IsActive = true
                },
                new Role
                {
                    Id = 4,
                    Name = "Customer",
                    Description = "Places orders and checks their traceability",
                    IsActive = true
                }
            );
            modelBuilder.Entity<OrderStatusHistory>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.StatusDate)
                    .IsRequired();

                entity.Property(e => e.Observation)
                    .HasMaxLength(255);

                entity.HasOne(e => e.Order)
                    .WithMany()
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.OrderStatus)
                    .WithMany()
                    .HasForeignKey(e => e.OrderStatusId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Incident>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(70);

                entity.Property(e => e.Description)
                    .IsRequired();

                entity.Property(e => e.IncidentDate)
                    .IsRequired();

                entity.HasOne(e => e.Delivery)
                    .WithMany(d => d.Incidents)
                    .HasForeignKey(e => e.DeliveryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Delivery>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.ScheduledDate)
                    .IsRequired();

                entity.Property(e => e.ContactPhone)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.RecipientName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.Status)
                    .HasConversion<string>()
                    .HasMaxLength(30)
                    .IsRequired();

                entity.Property(e => e.Observation);

                entity.HasOne(e => e.Order)
                    .WithMany()
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Address)
                    .WithMany()
                    .HasForeignKey(e => e.AddressId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.DeliveryDriver)
                    .WithMany()
                    .HasForeignKey(e => e.DeliveryDriverId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Method)
                    .HasConversion<string>()
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.Receipt)
                    .HasMaxLength(255);

                entity.Property(e => e.PaymentDate)
                    .IsRequired();

                entity.Property(e => e.Status)
                    .HasConversion<string>()
                    .HasMaxLength(30)
                    .IsRequired();

                entity.Property(e => e.Observation)
                    .HasMaxLength(255);

                entity.HasOne(e => e.Order)
                    .WithMany()
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.VerifiedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.VerifiedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<OrderStatus>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(70);

                entity.Property(e => e.Description)
                    .HasMaxLength(255);

                entity.Property(e => e.SortOrder)
                    .IsRequired();

                entity.Property(e => e.IsActive)
                    .IsRequired();

                entity.HasIndex(e => e.Name)
                    .IsUnique();
            });
            modelBuilder.Entity<InventoryMovement>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Type)
                    .IsRequired();

                entity.Property(e => e.Quantity)
                    .IsRequired();

                entity.Property(e => e.Reason)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.MovementDate)
                    .IsRequired();

                entity.HasOne(e => e.Product)
                    .WithMany()
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Description)
                    .HasMaxLength(255);

                entity.Property(e => e.IsActive)
                    .IsRequired();

                entity.HasIndex(e => e.Name)
                    .IsUnique();
            });
            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.AddressLine)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Reference)
                    .HasMaxLength(255);

                entity.Property(e => e.Label)
                    .HasMaxLength(50);

                entity.Property(e => e.IsActive)
                    .IsRequired();

                entity.HasOne(e => e.Customer)
                    .WithMany()
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<DeliveryDriver>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.IsAvailable)
                    .IsRequired();

                entity.HasOne(e => e.User)
                    .WithOne()
                    .HasForeignKey<DeliveryDriver>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.UserId)
                    .IsUnique();
            }); 
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Ci)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasOne(e => e.User)
                    .WithOne()
                    .HasForeignKey<Customer>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.UserId)
                    .IsUnique();

                entity.HasIndex(e => e.Ci)
                    .IsUnique();
            });
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasIndex(e => e.Code)
                    .IsUnique();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.Description);

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.CurrentStock)
                    .IsRequired();

                entity.Property(e => e.Image)
                    .HasMaxLength(255);

                entity.Property(e => e.IsActive)
                    .IsRequired();

                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasIndex(e => e.Code)
                    .IsUnique();

                entity.Property(e => e.OrderDate)
                    .IsRequired();

                entity.Property(e => e.Total)
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.Observation);

                entity.HasOne(e => e.Customer)
                    .WithMany()
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.CurrentStatus)
                    .WithMany()
                    .HasForeignKey(e => e.CurrentStatusId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Quantity)
                    .IsRequired();

                entity.Property(e => e.UnitPrice)
                    .HasColumnType("decimal(10,2)");

                entity.HasOne(e => e.Order)
                    .WithMany(o => o.OrderDetails)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                    .WithMany()
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.PasswordHash)
                    .IsRequired();

                entity.Property(e => e.IsActive)
                    .IsRequired();

                entity.HasIndex(e => e.Email)
                    .IsUnique();

                entity.HasOne(e => e.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}

