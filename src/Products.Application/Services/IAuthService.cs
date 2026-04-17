using Products.Application.Common.DTOs;

namespace Products.Application.Services;

public interface IAuthService
{
    Task<AuthTokenResult> LoginAsync(LoginDto dto);
    Task<AuthTokenResult> RefreshAsync(RefreshTokenDto dto);
    Task<RevokeResult> RevokeAsync(RevokeTokenDto dto);
}
