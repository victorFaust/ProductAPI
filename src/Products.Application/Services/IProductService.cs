using Products.Application.Common.DTOs;

namespace Products.Application.Services;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<IEnumerable<ProductDto>> GetByColourAsync(string colour);
    Task<ProductDto> CreateAsync(CreateProductDto request);
    Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto request);
    Task DeleteAsync(Guid id);
}
