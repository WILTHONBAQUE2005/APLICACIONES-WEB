namespace PaginaWeb.Core.Dtos;

public sealed record LoginRequest(string Email, string Password);
public sealed record RegisterRequest(string Email, string FullName, string Password);
public sealed record MeResponse(Guid Id, string Email, string FullName, string Role);
