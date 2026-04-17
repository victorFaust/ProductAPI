namespace Products.Domain.Exceptions;

public sealed class InvalidProductException : Exception
{
    public InvalidProductException(string message)
        : base(message)
    {
    }
}
