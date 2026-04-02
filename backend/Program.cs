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
// It will look for a Connection String in your environment variables/appsettings.json
// If it's not found, it defaults to the local shop.db (useful for quick local tests)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                      ?? "Host=ppmkjfmxztmgpqutrajn.supabase.co;Database=postgres;Username=postgres;Password=SultaiPlayer4Life";

builder.Services.AddDbContext<ShopContext>(options =>
    options.UseNpgsql(connectionString));

// 3. Robust CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("VercelPolicy", policy =>
    {
        // Replace this with your actual Vercel URL once the site is live
        policy.WithOrigins("https://my-ml-app-kcougar26.vercel.app") 
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
    
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// 4. Configure the HTTP request pipeline
if (app.Environment.IsProduction())
{
    app.UseCors("VercelPolicy");
}
else
{
    app.UseCors(); 
}

app.UseAuthorization();
app.MapControllers();

app.Run();