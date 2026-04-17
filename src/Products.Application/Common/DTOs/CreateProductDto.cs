namespace Products.Application.Common.DTOs
{

    public record CreateProductDto(
        string Name,
        string Description,
        decimal Price,
        string Colour);
}