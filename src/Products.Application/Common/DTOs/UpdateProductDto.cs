namespace Products.Application.Common.DTOs;

public record UpdateProductDto(
    string Name,
    string Description,
    decimal Price,
    string Colour);
