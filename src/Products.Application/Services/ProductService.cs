using FluentValidation;
using Products.Application.Common.DTOs;
using Products.Application.Common.Mappings;
using Products.Domain.Entities;
using Products.Domain.Interfaces.Persistence;
using Microsoft.Extensions.Logging;

namespace Products.Application.Services;

public sealed class ProductService : IProductService
{
    private readonly IUnitOfWork _uow;
    private readonly IValidator<CreateProductDto> _createValidator;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        IUnitOfWork uow,
        IValidator<CreateProductDto> createValidator,
        ILogger<ProductService> logger)
    {
        _uow = uow;
        _createValidator = createValidator;
        _logger = logger;
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        _logger.LogDebug("Fetching all products");
        var products = await _uow.Products.GetAllAsync();
        return products.Select(p => p.ToDto());
    }

    public async Task<IEnumerable<ProductDto>> GetByColourAsync(string colour)
    {
        _logger.LogDebug("Fetching products by colour: {Colour}", colour);
        var products = await _uow.Products.GetByColourAsync(colour);
        return products.Select(p => p.ToDto());
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto request)
    {
        _logger.LogInformation("Creating product {Name}", request.Name);

        await _createValidator.ValidateAndThrowAsync(request);

        var product = Product.Create(request.Name, request.Description, request.Price, request.Colour);
        _uow.Products.Add(product);
        await _uow.CommitAsync();

        _logger.LogInformation("Product created: {ProductId}", product.Id);
        return product.ToDto();
    }
}
