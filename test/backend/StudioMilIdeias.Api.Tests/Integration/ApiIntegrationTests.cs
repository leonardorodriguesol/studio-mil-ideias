using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using StudioMilIdeias.Api.Tests.Integration.Infrastructure;

namespace StudioMilIdeias.Api.Tests.Integration;

public sealed class ApiIntegrationTests : IClassFixture<IntegrationTestWebApplicationFactory>, IAsyncLifetime
{
    private readonly IntegrationTestWebApplicationFactory _factory;
    private HttpClient _client = default!;

    public ApiIntegrationTests(IntegrationTestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    public Task DisposeAsync()
    {
        _client.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Register_And_Login_ShouldReturnAccessToken()
    {
        var password = "User@Test123";
        var email = $"user-{Guid.NewGuid():N}@tests.local";

        var registerResponse = await _client.PostAsJsonAsync("/auth/register", new
        {
            name = "Integration User",
            email,
            password
        });

        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

        using var registerJson = await ReadJsonAsync(registerResponse);
        var accessToken = registerJson.RootElement.GetProperty("accessToken").GetString();
        var role = registerJson.RootElement.GetProperty("role").GetString();

        Assert.False(string.IsNullOrWhiteSpace(accessToken));
        Assert.Equal("Customer", role);

        var loginResponse = await _client.PostAsJsonAsync("/auth/login", new
        {
            email,
            password
        });

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        using var loginJson = await ReadJsonAsync(loginResponse);
        var loginToken = loginJson.RootElement.GetProperty("accessToken").GetString();
        Assert.False(string.IsNullOrWhiteSpace(loginToken));
    }

    [Fact]
    public async Task Cart_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync("/cart");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Checkout_WithEmptyCart_ShouldReturnBadRequest()
    {
        await RegisterAndAuthenticateAsync();

        var response = await _client.PostAsJsonAsync("/checkout", new
        {
            successUrl = "https://studio.tests.local/success",
            cancelUrl = "https://studio.tests.local/cancel"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        using var json = await ReadJsonAsync(response);
        var error = json.RootElement.GetProperty("error").GetString();
        Assert.Equal("Cart is empty.", error);
    }

    [Fact]
    public async Task StripeWebhook_WithInvalidSignature_ShouldReturnBadRequest()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/webhook/stripe")
        {
            Content = new StringContent("{\"sessionId\":\"test\"}", Encoding.UTF8, "application/json")
        };
        request.Headers.Add("Stripe-Signature", "invalid");

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PurchaseFlow_ShouldEnableDownloadOnlyAfterSuccessfulWebhook()
    {
        await RegisterAndAuthenticateAsync();
        var productId = await GetFirstProductIdAsync();

        var addToCartResponse = await _client.PostAsJsonAsync("/cart/items", new
        {
            productId,
            quantity = 1
        });
        Assert.Equal(HttpStatusCode.OK, addToCartResponse.StatusCode);

        var checkoutResponse = await _client.PostAsJsonAsync("/checkout", new
        {
            successUrl = "https://studio.tests.local/success",
            cancelUrl = "https://studio.tests.local/cancel"
        });
        Assert.Equal(HttpStatusCode.OK, checkoutResponse.StatusCode);

        using var checkoutJson = await ReadJsonAsync(checkoutResponse);
        var orderId = checkoutJson.RootElement.GetProperty("orderId").GetGuid();
        var sessionId = checkoutJson.RootElement.GetProperty("sessionId").GetString();
        Assert.False(string.IsNullOrWhiteSpace(sessionId));

        var prePaymentDownload = await _client.GetAsync($"/orders/{orderId}/items/{productId}/download-link");
        Assert.Equal(HttpStatusCode.NotFound, prePaymentDownload.StatusCode);

        var webhookRequest = new HttpRequestMessage(HttpMethod.Post, "/webhook/stripe")
        {
            Content = new StringContent(
                JsonSerializer.Serialize(new { sessionId }),
                Encoding.UTF8,
                "application/json")
        };
        webhookRequest.Headers.Add("Stripe-Signature", "test-signature");

        var webhookResponse = await _client.SendAsync(webhookRequest);
        Assert.Equal(HttpStatusCode.OK, webhookResponse.StatusCode);

        var ordersResponse = await _client.GetAsync("/orders");
        Assert.Equal(HttpStatusCode.OK, ordersResponse.StatusCode);

        using var ordersJson = await ReadJsonAsync(ordersResponse);
        Assert.Equal(JsonValueKind.Array, ordersJson.RootElement.ValueKind);
        Assert.True(ordersJson.RootElement.GetArrayLength() > 0);

        var orderStatus = ordersJson.RootElement[0].GetProperty("status").GetString();
        Assert.Equal("Paid", orderStatus);

        var downloadLinkResponse = await _client.GetAsync($"/orders/{orderId}/items/{productId}/download-link");
        Assert.Equal(HttpStatusCode.OK, downloadLinkResponse.StatusCode);

        using var downloadLinkJson = await ReadJsonAsync(downloadLinkResponse);
        var downloadUrl = downloadLinkJson.RootElement.GetProperty("downloadUrl").GetString();
        Assert.False(string.IsNullOrWhiteSpace(downloadUrl));

        var downloadToken = downloadUrl!.Split('/', StringSplitOptions.RemoveEmptyEntries).Last();
        var resolveResponse = await _client.GetAsync($"/downloads/{downloadToken}");
        Assert.Equal(HttpStatusCode.OK, resolveResponse.StatusCode);

        using var resolveJson = await ReadJsonAsync(resolveResponse);
        var resolvedProductId = resolveJson.RootElement.GetProperty("productId").GetGuid();
        var resourceUrl = resolveJson.RootElement.GetProperty("resourceUrl").GetString();

        Assert.Equal(productId, resolvedProductId);
        Assert.False(string.IsNullOrWhiteSpace(resourceUrl));
    }

    private async Task<(Guid UserId, string AccessToken)> RegisterAndAuthenticateAsync()
    {
        var email = $"user-{Guid.NewGuid():N}@tests.local";
        var password = "User@Test123";
        var response = await _client.PostAsJsonAsync("/auth/register", new
        {
            name = "Integration User",
            email,
            password
        });

        response.EnsureSuccessStatusCode();

        using var json = await ReadJsonAsync(response);
        var userId = json.RootElement.GetProperty("userId").GetGuid();
        var accessToken = json.RootElement.GetProperty("accessToken").GetString() ?? string.Empty;

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return (userId, accessToken);
    }

    private async Task<Guid> GetFirstProductIdAsync()
    {
        var response = await _client.GetAsync("/products");
        response.EnsureSuccessStatusCode();

        using var json = await ReadJsonAsync(response);
        Assert.Equal(JsonValueKind.Array, json.RootElement.ValueKind);
        Assert.True(json.RootElement.GetArrayLength() > 0);

        return json.RootElement[0].GetProperty("id").GetGuid();
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        await using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonDocument.ParseAsync(stream);
    }
}
