using System.Text.Json.Serialization;
//using Bilionn_backend.Auth;
using Bilionn_backend.Data;
using Bilionn_backend.Exceptions;
using Bilionn_backend.Repositories;
using Bilionn_backend.Services;
using FluentValidation;
//using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
//using Scalar.AspNetCore;

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
//builder.Services.AddAuthentication(PlainCookieAuthHandler.Scheme)
//    .AddScheme<AuthenticationSchemeOptions, PlainCookieAuthHandler>(PlainCookieAuthHandler.Scheme, null);
builder.Services.AddAuthorization();


// new cookie auth system
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "Bilionn.Auth"; // our custom cookie naam
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // 60 minutes expiry

        // Custom handling for API: if user unauthenticated, 401 bhej do (redirect mat karo)
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
    });

// CORS so Angular can send the cookie
builder.Services.AddCors(o => o.AddPolicy("ng", p => p
    .WithOrigins("http://localhost:4200")      // our Angular URL
    .AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

//builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();

app.UseCors("ng");                      
app.UseExceptionHandler();

//app.MapOpenApi();
//app.MapScalarApiReference();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bilionn API v1");
});



if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();