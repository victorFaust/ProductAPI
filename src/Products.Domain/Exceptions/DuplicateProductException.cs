namespace Products.Domain.Exceptions;

public sealed class DuplicateProductException : Exception
{
    public DuplicateProductException(string name)
        : base($"A product with the name '{name}' already exists.")
    {
    }
}
