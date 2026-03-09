using InventoryManagement.API.Middleware;
using InventoryManagement.Application.Features.Products.Handlers;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Infrastructure.Logging;
using InventoryManagement.Infrastructure.Persistence;
using InventoryManagement.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

SerilogConfig.Configure();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();


builder.Services.AddControllers();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => { options.CustomSchemaIds(type => type.ToString()); });


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Default")));


builder.Services.AddMemoryCache();


builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<ProductHandlers>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Inventory API v1");
    });
}

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();
app.Run();
