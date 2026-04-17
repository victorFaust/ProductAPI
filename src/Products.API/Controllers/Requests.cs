using System.ComponentModel.DataAnnotations;

namespace Products.API.Controllers;

public sealed record LoginRequest(
    [Required] string Username,
    [Required] string Password);

public sealed record RefreshRequest(
    [Required] string RefreshToken);

public sealed record RevokeRequest(
    [Required] string RefreshToken);

public sealed record CreateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    string Colour);
