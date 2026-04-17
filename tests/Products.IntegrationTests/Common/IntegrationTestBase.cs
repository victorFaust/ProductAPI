using System.Net.Http.Headers;
using System.Net.Http.Json;
using Products.Application.Common;
using Products.Application.Common.DTOs;

namespace Products.IntegrationTests.Common;

public abstract class IntegrationTestBase
{
    protected readonly HttpClient Client;

    protected const string AdminUsername = "admin";
    protected const string AdminPassword = "password123";

    protected IntegrationTestBase(CustomWebApplicationFactory factory)
    {
        Client = factory.CreateClient();
    }

    protected async Task<string> GetAuthTokenAsync()
    {
        var response = await Client.PostAsJsonAsync("/api/auth/login",
            new { username = AdminUsername, password = AdminPassword });

        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AuthTokenResult>>();
        return content!.Data!.AccessToken;
    }

    protected void AuthorizeClient(string token)
    {
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    protected async Task AuthorizeClientAsync()
    {
        var token = await GetAuthTokenAsync();
        AuthorizeClient(token);
    }
}
