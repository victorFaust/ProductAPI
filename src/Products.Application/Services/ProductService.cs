using FluentValidation;
using Products.Application.Common.DTOs;
using Products.Domain.Entities;
using AutoMapper;
using Products.Domain.Interfaces.Persistence;
using Microsoft.Extensions.Logging;

namespace Products.Application.Services;

public sealed class ProductService : IProductService
{
    private readonly IUnitOfWork _uow;
    private readonly IValidator<CreateProductDto> _createValidator;
    private readonly ILogger<ProductService> _logger;
    private readonly IMapper? _mapper;

    public ProductService(
        IUnitOfWork uow,
        IValidator<CreateProductDto> createValidator,
        ILogger<ProductService> logger,
        IMapper? mapper = null)
    {
        _uow = uow;
        _createValidator = createValidator;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        _logger.LogDebug("Fetching all products");
        var products = await _uow.Products.GetAllAsync();
        if (_mapper is not null)
            return _mapper.Map<IEnumerable<ProductDto>>(products);

        return products.Select(p => new ProductDto(
            p.Id,
            p.Name,
            p.Description,
            p.Price,
            p.Colour,
            p.CreatedAt,
            p.UpdatedAt));
    }

    public async Task<IEnumerable<ProductDto>> GetByColourAsync(string colour)
    {
        _logger.LogDebug("Fetching products by colour: {Colour}", colour);
        var products = await _uow.Products.GetByColourAsync(colour);
        if (_mapper is not null)
            return _mapper.Map<IEnumerable<ProductDto>>(products);

        return products.Select(p => new ProductDto(
            p.Id,
            p.Name,
            p.Description,
            p.Price,
            p.Colour,
            p.CreatedAt,
            p.UpdatedAt));
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto request)
    {
        _logger.LogInformation("Creating product {Name}", request.Name);

        await _createValidator.ValidateAndThrowAsync(request);

        var product = Product.Create(request.Name, request.Description, request.Price, request.Colour);
        _uow.Products.Add(product);
        await _uow.CommitAsync();

        _logger.LogInformation("Product created: {ProductId}", product.Id);
        if (_mapper is not null)
            return _mapper.Map<ProductDto>(product);

        return new ProductDto(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.Colour,
            product.CreatedAt,
            product.UpdatedAt);
    }

    public async Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto request)
    {
        _logger.LogInformation("Updating product {ProductId}", id);
        var product = await _uow.Products.GetByIdAsync(id);

        product.Update(request.Name, request.Description, request.Price, request.Colour);
        _uow.Products.Update(product);
        await _uow.CommitAsync();

        _logger.LogInformation("Product updated: {ProductId}", product.Id);
        if (_mapper is not null)
            return _mapper.Map<ProductDto>(product);

        return new ProductDto(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.Colour,
            product.CreatedAt,
            product.UpdatedAt);
    }

    public async Task DeleteAsync(Guid id)
    {
        _logger.LogInformation("Deleting product {ProductId}", id);
        var product = await _uow.Products.GetByIdAsync(id);

        _uow.Products.Remove(product);
        await _uow.CommitAsync();
        _logger.LogInformation("Product deleted: {ProductId}", id);
    }
}

