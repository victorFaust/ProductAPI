namespace Products.Domain.Exceptions;

public sealed class ProductNotFoundException : Exception
{
    public ProductNotFoundException(Guid id)
        : base($"Product with Id '{id}' was not found.")
    {
    }
}
