using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Products.Application.Common.DTOs;
using Products.Application.Services;
using Products.Application.Validators;
using Products.Domain.Entities;
using Products.Domain.Interfaces.Persistence;
using Products.Domain.Interfaces.Repositories;

namespace Products.UnitTests.Application.Services;

public class ProductServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUoW;
    private readonly Mock<IProductRepository> _mockRepo;
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        _mockRepo = new Mock<IProductRepository>();
        _mockUoW  = new Mock<IUnitOfWork>();
        _mockUoW.Setup(u => u.Products).Returns(_mockRepo.Object);
        _mockUoW.Setup(u => u.CommitAsync()).ReturnsAsync(1);

        _sut = new ProductService(
            _mockUoW.Object,
            new CreateProductDtoValidator(),
            NullLogger<ProductService>.Instance);
    }

    // Get All

    [Fact]
    public async Task GetAllAsync_WhenProductsExist_ReturnsAllMapped()
    {
        var products = new List<Product>
        {
            Product.Create("Widget A", "Desc", 1m, "Red"),
            Product.Create("Widget B", "Desc", 2m, "Blue"),
        };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(products);

        var result = await _sut.GetAllAsync();

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllAsync_WhenEmpty_ReturnsEmptyEnumerable()
    {
        _mockRepo.Setup(r => r.GetAllAsync())
                 .ReturnsAsync(Enumerable.Empty<Product>());

        var result = await _sut.GetAllAsync();

        result.Should().BeEmpty();
    }

    // Get By Colour

    [Fact]
    public async Task GetByColourAsync_ReturnsOnlyMatchingProducts()
    {
        var red = new List<Product> { Product.Create("Red Widget", "Desc", 1m, "Red") };
        _mockRepo.Setup(r => r.GetByColourAsync("Red")).ReturnsAsync(red);

        var result = await _sut.GetByColourAsync("Red");

        result.Should().HaveCount(1);
        result.Single().Colour.Should().Be("Red");
    }

    [Fact]
    public async Task GetByColourAsync_NoMatch_ReturnsEmpty()
    {
        _mockRepo.Setup(r => r.GetByColourAsync("Purple"))
                 .ReturnsAsync(Enumerable.Empty<Product>());

        var result = await _sut.GetByColourAsync("Purple");

        result.Should().BeEmpty();
    }

    // Create 

    [Fact]
    public async Task CreateAsync_ValidRequest_CallsRepositoryAddAndCommit()
    {
        var req = new CreateProductDto("Widget", "Desc", 9.99m, "Red");

        await _sut.CreateAsync(req);

        _mockRepo.Verify(r => r.Add(It.IsAny<Product>()), Times.Once);
        _mockUoW.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_ReturnsMappedDto()
    {
        var req = new CreateProductDto("Widget", "Desc", 9.99m, "Red");

        var result = await _sut.CreateAsync(req);

        result.Should().NotBeNull();
        result.Name.Should().Be("Widget");
        result.Price.Should().Be(9.99m);
        result.Colour.Should().Be("Red");
        result.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task CreateAsync_EmptyName_ThrowsValidationException()
    {
        var req = new CreateProductDto("", "Desc", 9.99m, "Red");

        var act = () => _sut.CreateAsync(req);

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*Name*");
    }

    [Fact]
    public async Task CreateAsync_NegativePrice_ThrowsValidationException()
    {
        var req = new CreateProductDto("Widget", "Desc", -1m, "Red");

        var act = () => _sut.CreateAsync(req);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateAsync_EmptyColour_ThrowsValidationException()
    {
        var req = new CreateProductDto("Widget", "Desc", 9.99m, "");

        var act = () => _sut.CreateAsync(req);

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*Colour*");
    }
}
