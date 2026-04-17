namespace Products.Application.Common
{

    public interface IJwtTokenService
    {

        (string Token, DateTime ExpiresAt) GenerateAccessToken(string username, IEnumerable<string> roles);


        string GenerateRefreshToken();

        bool ValidateToken(string token);
    }

}