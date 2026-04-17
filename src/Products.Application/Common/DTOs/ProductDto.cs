namespace Products.Application.Common.DTOs;

public sealed record ProductDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Colour,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
