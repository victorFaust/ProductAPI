using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Products.Application.Common;
using Products.Application.Common.DTOs;
using Products.IntegrationTests.Common;

namespace Products.IntegrationTests.Controllers;

/// <summary>
/// Each test gets a fresh factory/context because xUnit creates a new class instance
/// per test and the factory is owned by the instance (not shared via IClassFixture).
/// </summary>
public class ProductsControllerTests : IntegrationTestBase, IDisposable
{
    private readonly CustomWebApplicationFactory _factory;

    public ProductsControllerTests() : this(new CustomWebApplicationFactory()) { }

    private ProductsControllerTests(CustomWebApplicationFactory factory)
        : base(factory)
    {
        _factory = factory;
    }

    public void Dispose() => _factory.Dispose();

    // -- Authorization --------------------------------------------------------

    [Fact]
    public async Task GetProducts_WithoutToken_Returns401()
    {
        var response = await Client.GetAsync("/api/products");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateProduct_WithoutToken_Returns401()
    {
        var response = await Client.PostAsJsonAsync("/api/products",
            new { name = "Widget", price = 9.99, colour = "Red" });
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }


    [Fact]
    public async Task GetProducts_WithValidToken_Returns200WithEmptyArray()
    {
        await AuthorizeClientAsync();

        var response = await Client.GetAsync("/api/products");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<ProductDto>>>();
        body!.IsSuccess.Should().BeTrue();
        body.Data.Should().NotBeNull();
        body.Data!.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateProduct_WithValidToken_Returns201()
    {
        await AuthorizeClientAsync();

        var response = await Client.PostAsJsonAsync("/api/products",
            new { name = "Widget", description = "A fine widget", price = 9.99m, colour = "Blue" });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ProductDto>>();
        body!.IsSuccess.Should().BeTrue();
        body.Data!.Id.Should().NotBe(Guid.Empty);
        body.Data.Name.Should().Be("Widget");
        body.Data.Price.Should().Be(9.99m);
        body.Data.Colour.Should().Be("Blue");
    }

    // -- Validation 

    [Fact]
    public async Task CreateProduct_WithMissingName_Returns400()
    {
        await AuthorizeClientAsync();

        var response = await Client.PostAsJsonAsync("/api/products",
            new { description = "No name", price = 5.00m, colour = "Red" });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        body!.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task CreateProduct_WithNegativePrice_Returns400()
    {
        await AuthorizeClientAsync();

        var response = await Client.PostAsJsonAsync("/api/products",
            new { name = "Widget", price = -5.00m, colour = "Red" });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

   

    [Fact]
    public async Task GetProducts_AfterCreating3Products_ReturnsAll3()
    {
        await AuthorizeClientAsync();

        await Client.PostAsJsonAsync("/api/products", new { name = "A", price = 1m, colour = "Red" });
        await Client.PostAsJsonAsync("/api/products", new { name = "B", price = 2m, colour = "Blue" });
        await Client.PostAsJsonAsync("/api/products", new { name = "C", price = 3m, colour = "Green" });

        var response = await Client.GetAsync("/api/products");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<ProductDto>>>();
        body!.Data!.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetProducts_WithColourFilter_ReturnsOnlyMatchingProducts()
    {
        await AuthorizeClientAsync();

        await Client.PostAsJsonAsync("/api/products", new { name = "Red Widget",   price = 1m, colour = "Red" });
        await Client.PostAsJsonAsync("/api/products", new { name = "Blue Widget",  price = 2m, colour = "Blue" });
        await Client.PostAsJsonAsync("/api/products", new { name = "Green Widget", price = 3m, colour = "Green" });

        var response = await Client.GetAsync("/api/products?colour=Red");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<ProductDto>>>();
        body!.Data!.Should().HaveCount(1);
        body.Data.Single().Colour.Should().Be("Red");
    }

    [Fact]
    public async Task GetProducts_WithNonMatchingColour_Returns200EmptyArray()
    {
        await AuthorizeClientAsync();

        await Client.PostAsJsonAsync("/api/products", new { name = "Blue Widget", price = 5m, colour = "Blue" });

        var response = await Client.GetAsync("/api/products?colour=purple");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<ProductDto>>>();
        body!.Data!.Should().BeEmpty();
    }
}
