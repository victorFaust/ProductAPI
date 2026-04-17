using System.Collections.Concurrent;
using Products.Domain.Entities;

namespace Products.Infrastructure.Persistence.Context;

public sealed class InMemoryAppContext
{
    public ConcurrentDictionary<Guid, Product> Products { get; } = new();
}
