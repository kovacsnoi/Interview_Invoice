using Invoicing.Infrastructure.Persistence;
using Invoicing.Application.Interfaces;
using Invoicing.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Invoicing.Application.Services;
using QuestPDF.Infrastructure;
using Invoicing.Infrastructure.Documents;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core DbContext regisztrálása
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IInvoiceDocumentGenerator, InvoicePdfGenerator>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

var app = builder.Build();

// Automatikus migráció indításkor
using (var scope = app.Services.CreateScope())
{
  var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

  Console.WriteLine($"DB connection string: {db.Database.GetConnectionString()}");
  Console.WriteLine($"Current directory: {Directory.GetCurrentDirectory()}");

  db.Database.Migrate();
  Console.WriteLine("Migration completed.");
}

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();