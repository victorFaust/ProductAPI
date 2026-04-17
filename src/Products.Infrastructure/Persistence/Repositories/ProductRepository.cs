using Microsoft.EntityFrameworkCore;
using Products.Domain.Entities;
using Products.Domain.Interfaces.Repositories;
using Products.Infrastructure.Persistence.Context;

namespace Products.Infrastructure.Persistence.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
        => await _context.Products.AsNoTracking().ToListAsync();

    public async Task<IEnumerable<Product>> GetByColourAsync(string colour)
        => await _context.Products
            .AsNoTracking()
            .Where(p => EF.Functions.Like(p.Colour, colour))
            .ToListAsync();

    public async Task<Product?> GetByIdAsync(Guid id)
        => await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

    public void Add(Product product)
        => _context.Products.Add(product);

    public void Update(Product product)
        => _context.Products.Update(product);

    public void Remove(Product product)
        => _context.Products.Remove(product);
}
