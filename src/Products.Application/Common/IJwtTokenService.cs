namespace Products.Application.Common;

public interface IJwtTokenService
{
    /// <summary>Generates a short-lived JWT access token.</summary>
    (string Token, DateTime ExpiresAt) GenerateAccessToken(string username, IEnumerable<string> roles);

    /// <summary>Generates a cryptographically random refresh token string.</summary>
    string GenerateRefreshToken();

    bool ValidateToken(string token);
}
