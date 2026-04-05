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
        throw new InvalidOperationException("Missing database connection string. Set ConnectionStrings:DefaultConnection or DATABASE_URL.");
    }
});

builder.Services.AddHttpClient();

// 3. Robust CORS Policy
// We are explicitly allowing your Localhost and your Vercel URL
builder.Services.AddCors(options =>
{
    options.AddPolicy("VercelPolicy", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173", 
                "https://my-ml-app.vercel.app"
              )
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// 4. Configure the HTTP request pipeline
// Apply the policy globally for both Dev and Prod to avoid any mismatch
app.UseCors("VercelPolicy");

app.UseAuthorization();
app.MapControllers();

app.Run();