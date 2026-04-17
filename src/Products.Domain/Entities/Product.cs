using Products.Domain.Common;
using Products.Domain.Exceptions;

namespace Products.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Colour { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

  
    private Product() { }

    public static Product Create(string name, string description, decimal price, string colour)
    {
        Validate(name, description, price, colour);

        return new Product
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            Price = price,
            Colour = colour,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string description, decimal price, string colour)
    {
        Validate(name, description, price, colour);

        Name = name;
        Description = description;
        Price = price;
        Colour = colour;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateColour(string colour)
    {
        if (string.IsNullOrWhiteSpace(colour))
            throw new InvalidProductException("Colour cannot be empty.");

        Colour = colour;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void Validate(string name, string description, decimal price, string colour)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidProductException("Name cannot be empty.");

        if (price < 0)
            throw new InvalidProductException("Price cannot be negative.");

        if (string.IsNullOrWhiteSpace(colour))
            throw new InvalidProductException("Colour cannot be empty.");
    }
}
