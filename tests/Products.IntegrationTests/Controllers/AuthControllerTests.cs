using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Products.Application.Common;
using Products.Application.Common.DTOs;
using Products.IntegrationTests.Common;

namespace Products.IntegrationTests.Controllers;

public class AuthControllerTests
    : IntegrationTestBase, IClassFixture<CustomWebApplicationFactory>
{
    public AuthControllerTests(CustomWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task Login_WithValidCredentials_Returns200WithToken()
    {
        var response = await Client.PostAsJsonAsync("/api/auth/login",
            new { username = AdminUsername, password = AdminPassword });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<ApiResponse<AuthTokenResult>>();
        body.Should().NotBeNull();
        body!.Data!.AccessToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_WithInvalidPassword_Returns401()
    {
        var response = await Client.PostAsJsonAsync("/api/auth/login",
            new { username = AdminUsername, password = "wrong-password" });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithMissingUsername_Returns400()
    {
        // Send only the password field — Username is [Required] so model binding returns 400
        var response = await Client.PostAsJsonAsync("/api/auth/login",
            new { password = AdminPassword });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
