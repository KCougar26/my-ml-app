using Backend.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// 1. REGISTER SERVICES
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// Use the double underscore naming convention for Render
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                      ?? builder.Configuration["ConnectionStrings__DefaultConnection"];

if (string.IsNullOrEmpty(connectionString))
{
    // If the connection string is missing, we'll use a hardcoded fallback 
    // to prevent the 500 startup crash.
    connectionString = "Data Source=shop.db";
}

builder.Services.AddDbContext<ShopContext>(options =>
    options.UseSqlite(connectionString));

// Register HttpClient (Required for your StoreController's ML scoring)
builder.Services.AddHttpClient();

// CORS Policy
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

// 2. THE PIPELINE
// This shows the actual error message in the browser if the app crashes
app.UseDeveloperExceptionPage(); 

app.UseForwardedHeaders(); 
app.UseRouting();

// MUST be between UseRouting and MapControllers
app.UseCors("PermissivePolicy"); 

app.UseAuthorization();

app.MapControllers();

// Ensure the app listens on the port Render provides
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");