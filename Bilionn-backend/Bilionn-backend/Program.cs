using System.Text.Json.Serialization;
using Bilionn_backend.Auth;
using Bilionn_backend.Data;
using Bilionn_backend.Exceptions;
using Bilionn_backend.Repositories;
using Bilionn_backend.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<ICashierService, CashierService>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Cookie auth (plain cookie, handled by our own handler)
builder.Services.AddAuthentication(PlainCookieAuthHandler.Scheme)
    .AddScheme<AuthenticationSchemeOptions, PlainCookieAuthHandler>(PlainCookieAuthHandler.Scheme, null);
builder.Services.AddAuthorization();

// CORS so Angular can send the cookie
builder.Services.AddCors(o => o.AddPolicy("ng", p => p
    .WithOrigins("http://localhost:4200")      // your Angular URL
    .AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("ng");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();