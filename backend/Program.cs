using Backend.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Controllers with JSON naming consistency
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// 2. Configure PostgreSQL (Supabase)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                      ?? builder.Configuration["DATABASE_URL"];

builder.Services.AddDbContext<ShopContext>(options =>
{
    if (!string.IsNullOrWhiteSpace(connectionString))
    {
        options.UseNpgsql(connectionString);
    }
    else if (builder.Environment.IsDevelopment())
    {
        options.UseSqlite("Data Source=shop.db");
    }
    else
    {
        throw new InvalidOperationException("Missing database connection string.");
    }
});

builder.Services.AddHttpClient();

// 3. UPDATED: Robust CORS Policy
// We'll define one policy that covers all your needs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",          // Local Vite
                "https://my-ml-app.vercel.app"    // Your EXACT Vercel URL
              )
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// 4. UPDATED: Middleware Pipeline
// Use the "AllowAll" policy for everything to keep it simple
app.UseCors("AllowAll");

app.UseAuthorization();
app.MapControllers();

app.Run();