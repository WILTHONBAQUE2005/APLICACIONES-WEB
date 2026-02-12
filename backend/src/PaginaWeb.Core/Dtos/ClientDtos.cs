namespace PaginaWeb.Core.Dtos;

public sealed record ClientListItem(
    Guid Id,
    string DocumentType,
    string DocumentNumber,
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string AddressLine1,
    string City,
    string Notes,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc
);

public sealed record ClientUpsert(
    string DocumentType,
    string DocumentNumber,
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string AddressLine1,
    string City,
    string Notes,
    bool IsActive
);
