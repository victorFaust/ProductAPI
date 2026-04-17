using Products.Domain.Interfaces.Repositories;

namespace Products.Domain.Interfaces.Persistence;

public interface IUnitOfWork
{
    IProductRepository Products { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    IUserRepository Users { get; }

    Task<int> CommitAsync();
    Task RollbackAsync();
}
