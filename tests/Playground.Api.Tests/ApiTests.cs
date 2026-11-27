using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Playground.Api.Tests;

public class ApiTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Theory]
    [InlineData("/health/live")]
    [InlineData("/health/ready")]
    [InlineData("/api/info")]
    public async Task Endpoint_returns_ok(string path)
    {
        var response = await _client.GetAsync(path);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Items_are_listed()
    {
        var items = await _client.GetFromJsonAsync<Item[]>("/api/items");

        Assert.NotNull(items);
        Assert.Equal(3, items.Length);
    }

    [Fact]
    public async Task Unknown_item_returns_not_found()
    {
        var response = await _client.GetAsync("/api/items/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
