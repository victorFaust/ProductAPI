using Products.Domain.Entities;

namespace Products.Domain.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    void Add(RefreshToken refreshToken);
    void Revoke(RefreshToken refreshToken);
}
