using System.ComponentModel.DataAnnotations;

namespace Products.Application.Common.DTOs
{

    public record LoginRequest(
        [Required] string Username,
        [Required] string Password);

    public record RefreshRequest(
        [Required] string RefreshToken);

    public record RevokeRequest(
        [Required] string RefreshToken);

    public record CreateProductRequest(
        string Name,
        string? Description,
        decimal Price,
        string Colour);

    public record UpdateProductRequest(
        string Name,
        string? Description,
        decimal Price,
        string Colour);
}