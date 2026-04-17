namespace Products.Application.Common.DTOs;

/// <summary>Input DTO for creating a product — used by the service layer.</summary>
public sealed record CreateProductDto(
    string Name,
    string Description,
    decimal Price,
    string Colour);
