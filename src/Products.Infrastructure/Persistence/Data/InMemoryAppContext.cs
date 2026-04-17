using System.Collections.Concurrent;
using Products.Domain.Entities;

namespace Products.Infrastructure.Persistence.Context
{

    public class InMemoryAppContext
    {
        public ConcurrentDictionary<Guid, Product> Products { get; } = new();
    }
}