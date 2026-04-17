using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Products.Application.Common;
using Products.IntegrationTests.Common;

namespace Products.IntegrationTests.Controllers;

public class HealthControllerTests  : IntegrationTestBase, IClassFixture<CustomWebApplicationFactory>
{
    public HealthControllerTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetHealth_ReturnsOk_WithoutAuthentication()
    {
        var response = await Client.GetAsync("/health");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetHealth_ReturnsSuccessApiResponse()
    {
        var response = await Client.GetAsync("/health");

        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        body.Should().NotBeNull();
        body!.IsSuccess.Should().BeTrue();
        body.ResponseCode.Should().Be(200);
    }
}

