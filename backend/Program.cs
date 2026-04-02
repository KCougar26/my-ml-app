using Backend.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Controllers with JSON naming consistency
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // This ensures C# 'OrderDatetime' becomes 'orderDatetime' in React
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// 2. Configure SQLite with a robust path
// This ensures the DB is found regardless of where the app is hosted
var dbPath = Path.Combine(AppContext.BaseDirectory, "shop.db");
builder.Services.AddDbContext<ShopContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// 3. Robust CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("VercelPolicy", policy =>
    {
        policy.WithOrigins("https://your-app-name.vercel.app") // Replace with your actual Vercel URL
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
    
    // Keep a default policy for local development
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// 4. Configure the HTTP request pipeline
// Use the specific policy for production
if (app.Environment.IsProduction())
{
    app.UseCors("VercelPolicy");
}
else
{
    app.UseCors(); // Uses the default "AllowAll" for local dev
}

app.UseAuthorization();
app.MapControllers();

app.Run();