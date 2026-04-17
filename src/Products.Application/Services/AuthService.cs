using Products.Application.Common;
using Products.Application.Common.DTOs;
using Products.Domain.Entities;
using Products.Domain.Exceptions;
using Products.Domain.Interfaces.Persistence;

namespace Products.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;
    private readonly IJwtTokenService _jwtService;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IUnitOfWork uow, IJwtTokenService jwtService, IPasswordHasher passwordHasher)
    {
        _uow = uow;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthTokenResult> LoginAsync(LoginDto dto)
    {
        var user = await _uow.Users.GetByUsernameAsync(dto.Username);

        if (user is null || !_passwordHasher.Verify(dto.Password, user.PasswordHash))
            throw new AuthException("Invalid username or password.");

        var (accessToken, expiresAt) = _jwtService.GenerateAccessToken(dto.Username, user.Roles);
        var rawRefresh = _jwtService.GenerateRefreshToken();

        _uow.RefreshTokens.Add(RefreshToken.Create(rawRefresh, dto.Username, user.Roles));
        await _uow.CommitAsync();

        return new AuthTokenResult(accessToken, rawRefresh, expiresAt);
    }

    public async Task<AuthTokenResult> RefreshAsync(RefreshTokenDto dto)
    {
        var existing = await _uow.RefreshTokens.GetByTokenAsync(dto.RefreshToken);

        if (existing is null || !existing.IsActive)
            throw new AuthException("Refresh token is invalid or has expired.");

        existing.Revoke();
        _uow.RefreshTokens.Revoke(existing);

        var (accessToken, expiresAt) = _jwtService.GenerateAccessToken(existing.Username, existing.Roles);
        var newRaw = _jwtService.GenerateRefreshToken();

        _uow.RefreshTokens.Add(RefreshToken.Create(newRaw, existing.Username, existing.Roles));
        await _uow.CommitAsync();

        return new AuthTokenResult(accessToken, newRaw, expiresAt);
    }

    public async Task<RevokeResult> RevokeAsync(RevokeTokenDto dto)
    {
        var token = await _uow.RefreshTokens.GetByTokenAsync(dto.RefreshToken);

        if (token is null)
            throw new AuthException("Refresh token not found.");

        if (token.IsActive)
        {
            token.Revoke();
            _uow.RefreshTokens.Revoke(token);
            await _uow.CommitAsync();
        }

        return new RevokeResult("Token revoked");
    }
}
