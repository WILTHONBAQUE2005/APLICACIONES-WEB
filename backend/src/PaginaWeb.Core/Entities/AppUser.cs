namespace PaginaWeb.Core.Entities;

public sealed class AppUser : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "admin";
    public DateTime? LastLoginAtUtc { get; set; }
}
