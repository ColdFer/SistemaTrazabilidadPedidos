using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TrazabilidadPedidos.Client;
using TrazabilidadPedidos.Client.Services;



var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5216/")
});

builder.Services.AddScoped<AuthClientService>();
builder.Services.AddScoped<CustomerClientService>();
builder.Services.AddScoped<InventoryClientService>();
builder.Services.AddScoped<DeliveryClientService>();
builder.Services.AddScoped<ManagedUserClientService>();
builder.Services.AddScoped<OrderClientService>();
builder.Services.AddScoped<CartClientService>();
builder.Services.AddScoped<ProfileClientService>();
builder.Services.AddScoped<ReportClientService>();
builder.Services.AddScoped<PaymentClientService>();
builder.Services.AddScoped<NotificationClientService>();
builder.Services.AddScoped<InvoiceClientService>();
builder.Services.AddScoped<RoleClientService>();
builder.Services.AddScoped<PermissionClientService>();
builder.Services.AddScoped<AuditClientService>();

await builder.Build().RunAsync();

