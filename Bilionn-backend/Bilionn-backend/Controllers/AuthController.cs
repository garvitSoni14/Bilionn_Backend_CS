using System.Security.Claims;
//using System.Text.Json;
//using Bilionn_backend.Auth;
using Bilionn_backend.DTOs.Auth;
using Bilionn_backend.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bilionn_backend.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService authService;
    private readonly IValidator<BootstrapAdminRequest> bootstrapAdminRequest;
    private readonly IValidator<LoginRequest> loginValidator;

    public AuthController(IAuthService auth, IValidator<BootstrapAdminRequest> adminRequest, IValidator<LoginRequest> loginVal    )
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

        // 1. claims

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId),
            new(ClaimTypes.Name,           user.Name),
            new(ClaimTypes.Email,          user.Email),
            new(ClaimTypes.Role,           user.Role)
        };

        //// 2. claims -> raw text:  userId=...|name=...|email=...|role=...
        //var value = string.Join("|",
        //    $"userId={Clean(user.UserId)}",
        //    $"name={Clean(user.Name)}",
        //    $"email={Clean(user.Email)}",
        //    $"role={Clean(user.Role)}");

        //// 3. write the Set-Cookie header ourselves (Response.Cookies.Append would URL-encode it)
        //var cookie = $"{PlainCookieAuthHandler.CookieName}={value}; Path=/; SameSite=Lax" +
        //             $"; Expires={DateTimeOffset.UtcNow.AddMinutes(60):R}" +
        //             (Request.IsHttps ? "; Secure" : "");
        //Response.Headers.Append("Set-Cookie", cookie);

        // 2. ClaimsIdentity banayein
        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

        // 3. Cookie properties (e.g-  expiry) set 
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true, // Cookie will sustain even after browser get closed
            //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)  commented out because it will prevent the slidingexpiration to work...
        };

        // 4. .NET's built-in SignInAsync called (....Yeh automatic encrypted cookie browser ko bhej dega)
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);
        
        
        //var json = JsonSerializer.Serialize(new
        //{
        //    userId = user.UserId,
        //    name = user.Name,
        //    email = user.Email,
        //    role = user.Role
        //});


        //Response.Cookies.Append(PlainCookieAuthHandler.CookieName, json, new CookieOptions
        //{
        //    HttpOnly = false,                 // readable in the browser
        //    Secure = Request.IsHttps,
        //    SameSite = SameSiteMode.Lax,
        //    Path = "/",
        //    Expires = DateTimeOffset.UtcNow.AddMinutes(60)
        //});

        return Ok(user);
    }

    //// keeps characters that would break a cookie (or our | separator) out of the value
    //private static string Clean(string? s) =>
    //    (s ?? "").Replace("|", " ").Replace(";", " ").Replace(",", " ").Trim();

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        //Response.Cookies.Delete(PlainCookieAuthHandler.CookieName);
        //return NoContent();

        // .NET's own cookie sign-out helper
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
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
