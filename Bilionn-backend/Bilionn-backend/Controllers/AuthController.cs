

using System.Security.Claims;
using System.Text.Json;
using Bilionn_backend.Auth;
using Bilionn_backend.DTOs.Auth;
using Bilionn_backend.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bilionn_backend.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService authService;
    private readonly IValidator<BootstrapAdminRequest> bootstrapAdminRequest;
    private readonly IValidator<LoginRequest> loginValidator;

    public AuthController(IAuthService auth, IValidator<BootstrapAdminRequest> adminRequest, IValidator<LoginRequest> loginVal)
    {
        authService = auth;
        bootstrapAdminRequest = adminRequest;
        loginValidator = loginVal;
    }

    [HttpPost("bootstrap-admin")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> BootstrapAdmin(
        BootstrapAdminRequest request,
        [FromHeader(Name = "X-Setup-Key")] string? setupKey)
    {
        var validation = await bootstrapAdminRequest.ValidateAsync(request);
        if (!validation.IsValid)
            return ValidationProblem(new ValidationProblemDetails(validation.ToDictionary()));

        var admin = await authService.BootstrapAdminAsync(request, setupKey);
        return StatusCode(StatusCodes.Status201Created, admin);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var validation = await loginValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return ValidationProblem(new ValidationProblemDetails(validation.ToDictionary()));

        var user = await authService.LoginAsync(request);

        var json = JsonSerializer.Serialize(new
        {
            userId = user.UserId,
            name = user.Name,
            email = user.Email,
            role = user.Role
        });

        // ASP.NET URL-encodes the value for us; don't escape it yourself
        Response.Cookies.Append(PlainCookieAuthHandler.CookieName, json, new CookieOptions
        {
            HttpOnly = false,                 // readable in the browser
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Path = "/",
            Expires = DateTimeOffset.UtcNow.AddMinutes(60)
        });

        return Ok(user);
    }

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(PlainCookieAuthHandler.CookieName);
        return NoContent();
    }

    // Angular calls this on startup: 200 + user, or 401 = show login page
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Me()
    {
        var email = User.FindFirstValue(ClaimTypes.Email)!;
        return Ok(await authService.GetCurrentUserAsync(email));
    }
}



//using System.Text.Json;
//using Bilionn_backend.DTOs.Auth;
//using Bilionn_backend.Services;
//using FluentValidation;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace Bilionn_backend.Controllers;

//[ApiController]
//[Route("api/auth")]
//public class AuthController: ControllerBase
//{
//    private readonly IAuthService authService;
//    private readonly IValidator<BootstrapAdminRequest> bootstrapAdminRequest;
//    private readonly IValidator<LoginRequest> loginValidator;

//    public AuthController(IAuthService auth, IValidator<BootstrapAdminRequest> adminRequest, IValidator<LoginRequest> loginVal)
//    {
//        authService = auth;
//        bootstrapAdminRequest = adminRequest;
//        loginValidator = loginVal;
//    }

//    [HttpPost("bootstrap-admin")]
//    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
//    public async Task<IActionResult> BootstrapAdmin(
//        BootstrapAdminRequest request,
//        [FromHeader(Name = "X-Setup-Key")] string? setupKey)
//    {
//        var validation = await bootstrapAdminRequest.ValidateAsync(request);
//        if (!validation.IsValid)
//            return ValidationProblem(new ValidationProblemDetails(validation.ToDictionary()));

//        var admin = await authService.BootstrapAdminAsync(request, setupKey);
//        return StatusCode(StatusCodes.Status201Created, admin);
//    }




//    [HttpPost("login")]
//    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
//    public async Task<IActionResult> Login(LoginRequest request)
//    {
//        var validation = await loginValidator.ValidateAsync(request);
//        if (!validation.IsValid)
//            return ValidationProblem(new ValidationProblemDetails(validation.ToDictionary()));

//        var user = await authService.LoginAsync(request);

//        var json = JsonSerializer.Serialize(new
//        {
//            userId = user.UserId,
//            name = user.Name,
//            email = user.Email,
//            role = user.Role.ToString()
//        });

//        Response.Cookies.Append(PlainCookieAuthHandler.CookieName, Uri.EscapeDataString(json), new CookieOptions
//        {
//            HttpOnly = false,              // readable in the browser; set true to hide it from JavaScript
//            Secure = Request.IsHttps,
//            SameSite = SameSiteMode.Lax,
//            Path = "/",
//            Expires = DateTimeOffset.UtcNow.AddMinutes(60)
//        });

//        return Ok(user);
//    }

//    [Authorize]
//    [HttpPost("logout")]
//    public IActionResult Logout()
//    {
//        Response.Cookies.Delete(PlainCookieAuthHandler.CookieName);
//        return NoContent();
//    }

//}
