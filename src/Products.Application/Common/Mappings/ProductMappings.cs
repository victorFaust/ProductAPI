using Products.Application.Common.DTOs;
using Products.Domain.Entities;

namespace Products.Application.Common.Mappings;

public static class ProductMappings
{
    public static ProductDto ToDto(this Product product) =>
        new(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.Colour,
            product.CreatedAt,
            product.UpdatedAt);
}
