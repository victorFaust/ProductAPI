namespace Products.Application.Common.DTOs
{

    public record LoginDto(string Username, string Password);

    public record RefreshTokenDto(string RefreshToken);

    public record RevokeTokenDto(string RefreshToken);

    public record AuthTokenResult(string AccessToken,string RefreshToken,DateTime AccessTokenExpiresAt);

    public record RevokeResult(string Message);
    public sealed record AuthErrorResponse(string Message);
}
