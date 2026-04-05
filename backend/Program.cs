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
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                      ?? builder.Configuration["DATABASE_URL"];

builder.Services.AddDbContext<ShopContext>(options =>
{
    if (!string.IsNullOrWhiteSpace(connectionString))
    {
        options.UseNpgsql(connectionString);
        return;
    }

    if (builder.Environment.IsDevelopment())
    {
        options.UseSqlite("Data Source=shop.db");
        return;
    }

    throw new InvalidOperationException("Missing database connection string. Set ConnectionStrings:DefaultConnection or DATABASE_URL.");
});

builder.Services.AddHttpClient();

// 3. Robust CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("VercelPolicy", policy =>
    {
        var origin = builder.Configuration["Frontend:Origin"] 
                     ?? builder.Configuration["FRONTEND_ORIGIN"];
        if (!string.IsNullOrWhiteSpace(origin))
        {
            policy.WithOrigins(origin)
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
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
