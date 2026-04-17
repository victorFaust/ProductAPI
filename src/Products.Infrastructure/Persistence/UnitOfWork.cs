using Products.Domain.Interfaces.Persistence;
using Products.Domain.Interfaces.Repositories;
using Products.Infrastructure.Persistence.Context;
using Products.Infrastructure.Persistence.Repositories;

namespace Products.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly AppDbContext _context;
    private bool _disposed;

    private IProductRepository? _products;
    private IRefreshTokenRepository? _refreshTokens;
    private IUserRepository? _users;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IProductRepository Products
    {
        get
        {
            return _products ??= new ProductRepository(_context);
        }
    }

    public IRefreshTokenRepository RefreshTokens
    {
        get
        {
            return _refreshTokens ??= new RefreshTokenRepository(_context);
        }
    }

    public IUserRepository Users
    {
        get
        {
            return _users ??= new UserRepository(_context);
        }
    }

    public Task<int> CommitAsync()
    {
        return _context.SaveChangesAsync();
    }

    public Task RollbackAsync()
    {
        foreach (var entry in _context.ChangeTracker.Entries())
            entry.State = Microsoft.EntityFrameworkCore.EntityState.Detached;
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
            _context.Dispose();
        _disposed = true;
    }
}
