using Microsoft.EntityFrameworkCore;
using Products.Domain.Entities;
using Products.Domain.Interfaces.Repositories;
using Products.Infrastructure.Persistence.Context;

namespace Products.Infrastructure.Persistence.Repositories;

public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _context;

    public RefreshTokenRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
        => await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token);

    public void Add(RefreshToken refreshToken)
        => _context.RefreshTokens.Add(refreshToken);

    public void Revoke(RefreshToken refreshToken)
        => _context.RefreshTokens.Update(refreshToken);
}
