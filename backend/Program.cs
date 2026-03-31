using Backend.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Add services to the container.
builder.Services.AddControllers();

// 2. Configure SQLite
builder.Services.AddDbContext<ShopContext>(options =>
    options.UseSqlite("Data Source=shop.db"));

// 3. Enable CORS (So your React frontend can talk to this API)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();