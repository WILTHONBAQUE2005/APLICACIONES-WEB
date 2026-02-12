using System.Security.Claims;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaginaWeb.Core.Dtos;
using PaginaWeb.Core.Entities;
using PaginaWeb.Core.Interfaces;

namespace PaginaWeb.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly IAntiforgery _antiforgery;
    private readonly IWebHostEnvironment _env;

    public AuthController(IUserRepository users, IPasswordHasher hasher, IAntiforgery antiforgery, IWebHostEnvironment env)
    {
        _users = users;
        _hasher = hasher;
        _antiforgery = antiforgery;
        _env = env;
    }

    // Importante: aquí exponemos el RequestToken en cookie XSRF-TOKEN (NO HttpOnly) para Angular
    [HttpGet("csrf")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public IActionResult Csrf()
    {
        var tokens = _antiforgery.GetAndStoreTokens(HttpContext);

        if (!string.IsNullOrWhiteSpace(tokens.RequestToken))
        {
            Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken, new CookieOptions
            {
                HttpOnly = false, // Angular debe poder leerla
                SameSite = SameSiteMode.Lax,
                Secure = !_env.IsDevelopment(),
                Path = "/"
            });
        }

        return NoContent();
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<MeResponse>> Me(CancellationToken ct)
    {
        var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(idStr, out var id)) return Unauthorized();

        var user = await _users.GetByIdAsync(id, ct);
        if (user is null) return Unauthorized();

        return new MeResponse(user.Id, user.Email, user.FullName, user.Role);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<MeResponse>> Login([FromBody] LoginRequest req, CancellationToken ct)
    {
        var email = (req.Email ?? string.Empty).Trim().ToLowerInvariant();
        var user = await _users.FindByEmailAsync(email, ct);
        if (user is null || !user.IsActive)
            return Unauthorized(new ProblemDetails { Status = 401, Title = "Invalid credentials" });

        if (!_hasher.Verify(req.Password ?? string.Empty, user.PasswordHash))
            return Unauthorized(new ProblemDetails { Status = 401, Title = "Invalid credentials" });

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, user.Role),
            new("sid", Guid.NewGuid().ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                AllowRefresh = true,
                IssuedUtc = DateTimeOffset.UtcNow
            });

        return new MeResponse(user.Id, user.Email, user.FullName, user.Role);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return NoContent();
    }

    [HttpPost("register")]
[AllowAnonymous]
public async Task<ActionResult<MeResponse>> Register([FromBody] RegisterRequest req, CancellationToken ct)
{
    var email = (req.Email ?? string.Empty).Trim().ToLowerInvariant();
    var fullName = (req.FullName ?? string.Empty).Trim();
    var password = (req.Password ?? string.Empty);

    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(password))
        return BadRequest(new ProblemDetails { Status = 400, Title = "Missing fields" });

    var existing = await _users.FindByEmailAsync(email, ct);
    if (existing is not null)
        return Conflict(new ProblemDetails { Status = 409, Title = "Email already exists" });

    var user = new AppUser
    {
        Email = email,
        FullName = fullName,
        Role = "user",
        IsActive = true,
        PasswordHash = _hasher.Hash(password)
    };

    await _users.AddAsync(user, ct);
    await _users.SaveAsync(ct);

    // Auto-login (cookie) igual que en Login()
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Email, user.Email),
        new(ClaimTypes.Name, user.FullName),
        new(ClaimTypes.Role, user.Role),
        new("sid", Guid.NewGuid().ToString())
    };

    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new ClaimsPrincipal(identity);

    await HttpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        principal,
        new AuthenticationProperties
        {
            IsPersistent = true,
            AllowRefresh = true,
            IssuedUtc = DateTimeOffset.UtcNow
        });

    return new MeResponse(user.Id, user.Email, user.FullName, user.Role);
}


}
