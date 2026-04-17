using FluentAssertions;
using Products.Domain.Entities;
using Products.Domain.Exceptions;

namespace Products.UnitTests.Domain.Entities;

public class ProductTests
{
    // ── Factory: happy path ──────────────────────────────────────────────────

    [Fact]
    public void Create_WithValidInputs_SetsAllPropertiesCorrectly()
    {
        var product = Product.Create("Widget", "A great widget", 9.99m, "Red");

        product.Name.Should().Be("Widget");
        product.Description.Should().Be("A great widget");
        product.Price.Should().Be(9.99m);
        product.Colour.Should().Be("Red");
        product.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void Create_SetsIdToNewGuid()
    {
        var product = Product.Create("Widget", "Desc", 1m, "Blue");

        product.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Create_SetsCreatedAtToUtcNow()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);

        var product = Product.Create("Widget", "Desc", 1m, "Blue");

        product.CreatedAt.Should().BeAfter(before).And.BeOnOrBefore(DateTime.UtcNow);
    }

    // ── Factory: validation guards ───────────────────────────────────────────

    [Fact]
    public void Create_WithEmptyName_ThrowsInvalidProductException()
    {
        var act = () => Product.Create("", "Desc", 1m, "Blue");

        act.Should().Throw<InvalidProductException>();
    }

    [Fact]
    public void Create_WithNegativePrice_ThrowsInvalidProductException()
    {
        var act = () => Product.Create("Widget", "Desc", -0.01m, "Blue");

        act.Should().Throw<InvalidProductException>();
    }

    [Fact]
    public void Create_WithEmptyColour_ThrowsInvalidProductException()
    {
        var act = () => Product.Create("Widget", "Desc", 1m, "");

        act.Should().Throw<InvalidProductException>();
    }

    // ── Update ───────────────────────────────────────────────────────────────

    [Fact]
    public void Update_ChangesNameAndPrice_SetsUpdatedAt()
    {
        var product = Product.Create("Widget", "Desc", 1m, "Blue");

        product.Update("Updated Widget", "New Desc", 19.99m, "Green");

        product.Name.Should().Be("Updated Widget");
        product.Price.Should().Be(19.99m);
        product.Colour.Should().Be("Green");
        product.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Update_SetsUpdatedAtToUtcNow()
    {
        var product = Product.Create("Widget", "Desc", 1m, "Blue");
        var before = DateTime.UtcNow.AddSeconds(-1);

        product.Update("Widget", "Desc", 2m, "Blue");

        product.UpdatedAt.Should().NotBeNull();
        product.UpdatedAt!.Value.Should().BeAfter(before).And.BeOnOrBefore(DateTime.UtcNow);
    }
}
