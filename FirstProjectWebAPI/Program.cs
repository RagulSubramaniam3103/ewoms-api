using FirstProjectWebAPI.Commands.CustomerDetails;
using FirstProjectWebAPI.MainData.DBContextFiles;
using FirstProjectWebAPI.MainData.ModelsMigration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MainDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<CustomerDetailsCommandsHandler>();

// Add services to the container
builder.Services.AddControllers();

// ✅ Add Swagger (old familiar way)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddIdentity<CustomerDetails, IdentityRole>().AddEntityFrameworkStores<MainDBContext>().AddDefaultTokenProviders();

var app = builder.Build();

// ✅ Enable Swagger UI in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FirstProjectWebAPI v1");
        c.RoutePrefix = string.Empty; // 👈 this makes Swagger open at root URL
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
