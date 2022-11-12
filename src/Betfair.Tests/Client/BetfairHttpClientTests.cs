using System.Net.Http.Headers;
using Betfair.Client;
using Betfair.Errors;
using Betfair.Login;
using Betfair.Tests.TestDoubles;
using CSharpFunctionalExtensions;
using Utf8Json;
using Utf8Json.Resolvers;

namespace Betfair.Tests.Client;

public sealed class BetfairHttpClientTests : IDisposable
{
    private readonly HttpMessageHandlerSpy _handler = new ();
    private readonly BetfairHttpClient _client;
    private readonly Credentials _cred = Credentials.Create("username", "password", "appKey").Value;

    public BetfairHttpClientTests()
    {
        var response = new Response { Token = "Token", Status = "SUCCESS", };
        _handler.SetResponseBody(JsonSerializer.ToJsonString(response, StandardResolver.CamelCase));

        _client = new BetfairHttpClient(_handler);
    }

    [Fact]
    public void TimeoutIsSetToThirtySeconds()
    {
        using var client = new BetfairHttpClient();

        Assert.Equal(TimeSpan.FromSeconds(30), client.Timeout);
    }

    [Fact]
    public void AcceptsApplicationJson()
    {
        using var client = new BetfairHttpClient();

        Assert.Contains(
            new MediaTypeWithQualityHeaderValue("application/json"),
            client.DefaultRequestHeaders.Accept);
    }

    [Fact]
    public void ConnectionIsKeptAlive()
    {
        using var client = new BetfairHttpClient();

        Assert.Contains(
            "keep-alive",
            client.DefaultRequestHeaders.Connection);
    }

    [Fact]
    public void AcceptGzipEncoding()
    {
        using var client = new BetfairHttpClient();

        Assert.Contains(
            new StringWithQualityHeaderValue("gzip"),
            client.DefaultRequestHeaders.AcceptEncoding);
    }

    [Fact]
    public async Task LoginReturnErrorIfCredentialsAreNull()
    {
        var result = await _client.Login(null, default);

        AssertError(ErrorResult.Empty("credentials"), result);
    }

    [Fact]
    public async Task LoginPostsToApiLoginEndpoint()
    {
        await _client.Login(_cred, default);

        _handler.AssertRequestMethod(HttpMethod.Post);
        _handler.AssertRequestUri(new Uri("https://identitysso.betfair.com/api/login"));
    }

    [Fact]
    public async Task LoginAcceptsApplicationJson()
    {
        await _client.Login(_cred, default);

        _handler.AssertRequestHeader("Accept", "application/json");
    }

    [Theory]
    [InlineData("AppKey")]
    [InlineData("Other")]
    public async Task HeadersContainAppKey(string appKey)
    {
        var cred = Credentials.Create("username", "password", appKey).Value;

        await _client.Login(cred, default);

        _handler.AssertRequestHeader("X-Application", appKey);
    }

    [Fact]
    public async Task LoginContentTypeIsFormUrlEncoded()
    {
        await _client.Login(_cred, default);

        _handler.AssertContentType("application/x-www-form-urlencoded");
    }

    [Theory]
    [InlineData("Username")]
    [InlineData("Other")]
    public async Task UsernameIsInFormContent(string username)
    {
        var cred = Credentials.Create(username, "password", "appKey").Value;

        await _client.Login(cred, default);

        using var content = new FormUrlEncodedContent(
            new Dictionary<string, string> { { "username", username }, { "password", "password" } });
        _handler.AssertRequestContent(await content.ReadAsStringAsync());
    }

    [Theory]
    [InlineData("Password")]
    [InlineData("Other")]
    public async Task PasswordIsInFormContent(string password)
    {
        var cred = Credentials.Create("username", password, "appKey").Value;

        await _client.Login(cred, default);

        using var content = new FormUrlEncodedContent(
            new Dictionary<string, string> { { "username", "username" }, { "password", password } });
        _handler.AssertRequestContent(await content.ReadAsStringAsync());
    }

    [Theory]
    [InlineData("Token")]
    [InlineData("Other")]
    public async Task LoginRespondsWithSessionToken(string token)
    {
        var response = new Response { Token = token, Status = "SUCCESS", };
        _handler.SetResponseBody(JsonSerializer.ToJsonString(response, StandardResolver.CamelCase));

        var result = await _client.Login(_cred, default);

        Assert.Equal(token, result.Value);
    }

    [Theory]
    [InlineData("ERROR_MESSAGE")]
    [InlineData("INVALID_USERNAME_OR_PASSWORD")]
    public async Task LoginRespondsWithErrorIfNotSuccessful(string error)
    {
        var response = new Response { Status = "FAIL", Error = error };
        _handler.SetResponseBody(JsonSerializer.ToJsonString(response, StandardResolver.CamelCase));

        var result = await _client.Login(_cred, default);

        AssertError(ErrorResult.Create(error), result);
    }

    public void Dispose()
    {
        _handler?.Dispose();
        _client?.Dispose();
    }

    private static void AssertError(ErrorResult expected, Result<object, ErrorResult> result)
    {
        Assert.True(result.IsFailure);
        Assert.Equal(expected, result.Error);
        Assert.Equal(expected.Message, result.Error.Message);
    }
}
