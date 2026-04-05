using Backend.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// 1. Services
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                      ?? builder.Configuration["DATABASE_URL"];

builder.Services.AddDbContext<ShopContext>(options =>
{
    if (!string.IsNullOrWhiteSpace(connectionString))
    {
        options.UseNpgsql(connectionString);
    }
});

builder.Services.AddHttpClient();

// 2. The "Everything Allowed" Policy (For Troubleshooting)
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermissivePolicy", policy =>
    {
        policy.AllowAnyOrigin() 
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// 3. THE PIPELINE - THIS EXACT ORDER IS VITAL
app.UseForwardedHeaders(); // Helps Render pass the correct original headers
app.UseRouting();

// MUST be between UseRouting and UseEndpoints/MapControllers
app.UseCors("PermissivePolicy"); 

app.UseAuthorization();

app.MapControllers();

app.Run();