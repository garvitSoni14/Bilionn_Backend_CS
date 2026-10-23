//namespace Bilionn_backend.Auth;

//using System.Security.Claims;
//using System.Text.Encodings.Web;
////using System.Text.Json;
//using Bilionn_backend.Models;
//using Bilionn_backend.Repositories;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.Extensions.Options;


//public class PlainCookieAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
//{
//    public const string Scheme = "PlainCookie";
//    public const string CookieName = "Bilionn.Auth";

//    private readonly IUserRepository _users;

//    public PlainCookieAuthHandler(
//        IOptionsMonitor<AuthenticationSchemeOptions> options,
//        ILoggerFactory logger,
//        UrlEncoder encoder,
//        IUserRepository users) : base(options, logger, encoder)
//    {
//        _users = users;
//    }

//    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
//    {
//        // 1. no cookie = anonymous (-> 401 on [Authorize] endpoints)
//        if (!Request.Cookies.TryGetValue(CookieName, out var raw) || string.IsNullOrWhiteSpace(raw))
//            return AuthenticateResult.NoResult();

//        //// 2. read only the email from the cookie JSON
//        //string? email;
//        //try
//        //{
//        //    using var doc = JsonDocument.Parse(raw);
//        //    email = doc.RootElement.GetProperty("email").GetString();
//        //}
//        //catch
//        //{
//        //    return AuthenticateResult.Fail("Malformed cookie.");
//        //}
//        //if (string.IsNullOrWhiteSpace(email))
//        //    return AuthenticateResult.Fail("No email in cookie.");

//        var email = raw.Split('|')
//        .Select(part => part.Split('=', 2))
//        .Where(kv => kv.Length == 2 && kv[0] == "email")
//        .Select(kv => kv[1])
//        .FirstOrDefault();

//        if (string.IsNullOrWhiteSpace(email))
//            return AuthenticateResult.Fail("No email in cookie.");

//        // 3. load the user from the DB    ---IMPORTANT STEP---
//        var user = await _users.GetByEmailAsync(email.Trim().ToLowerInvariant());
//        if (user is null || user.AccountStatus != AccountStatus.ACTIVE)
//            return AuthenticateResult.Fail("User not found or not active.");

//        // 4. claims -> ClaimsIdentity -> ClaimsPrincipal (values come from the DB)
//        var claims = new List<Claim>
//        {
//            new(ClaimTypes.NameIdentifier, user.UserId ?? ""),
//            new(ClaimTypes.Name,           user.Name ?? ""),
//            new(ClaimTypes.Email,          user.Email),
//            new(ClaimTypes.Role,           user.Role.ToString())
//        };
//        var identity = new ClaimsIdentity(claims, Scheme);
//        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme);
//        return AuthenticateResult.Success(ticket);
//    }
//}
