using Backend.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Controllers
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
});

builder.Services.AddHttpClient();

// 3. Define the Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("VercelPolicy", policy =>
    {
        policy.WithOrigins("https://my-ml-app.vercel.app") // No trailing slash!
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Added for extra compatibility
    });
});

var app = builder.Build();

// 4. THE PIPELINE - ORDER IS CRITICAL HERE
app.UseRouting();

// CORS MUST BE HERE: After Routing, Before Authorization/Map
app.UseCors("VercelPolicy"); 

app.UseAuthorization();

app.MapControllers();

app.Run();