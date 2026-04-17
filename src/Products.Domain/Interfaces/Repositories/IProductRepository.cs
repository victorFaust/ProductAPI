using Products.Domain.Entities;

namespace Products.Domain.Interfaces.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<IEnumerable<Product>> GetByColourAsync(string colour);
    Task<Product?> GetByIdAsync(Guid id);

    /// <remarks>Synchronous — the Unit of Work is responsible for committing.</remarks>
    void Add(Product product);

    /// <remarks>Synchronous — the Unit of Work is responsible for committing.</remarks>
    void Update(Product product);

    /// <remarks>Synchronous — the Unit of Work is responsible for committing.</remarks>
    void Remove(Product product);
}
