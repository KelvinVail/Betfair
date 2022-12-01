using System.Net;
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
    private readonly Uri _uri = new ("http://test.com/");

    public BetfairHttpClientTests()
    {
        var response = new LoginResponse { Token = "Token", Status = "SUCCESS", };
        _handler.SetResponseBody(JsonSerializer.ToJsonString(response, StandardResolver.CamelCase));

        _client = new BetfairHttpClient(_cred, _handler);
    }

    [Fact]
    public void TimeoutIsSetToThirtySeconds()
    {
        using var client = new BetfairHttpClient(_cred, _handler);

        Assert.Equal(TimeSpan.FromSeconds(30), client.Timeout);
    }

    [Fact]
    public void AcceptsApplicationJson()
    {
        using var client = new BetfairHttpClient(_cred, _handler);

        Assert.Contains(
            new MediaTypeWithQualityHeaderValue("application/json"),
            client.DefaultRequestHeaders.Accept);
    }

    [Fact]
    public void ConnectionIsKeptAlive()
    {
        using var client = new BetfairHttpClient(_cred, _handler);

        Assert.Contains(
            "keep-alive",
            client.DefaultRequestHeaders.Connection);
    }

    [Fact]
    public void AcceptGzipEncoding()
    {
        using var client = new BetfairHttpClient(_cred, _handler);

        Assert.Contains(
            new StringWithQualityHeaderValue("gzip"),
            client.DefaultRequestHeaders.AcceptEncoding);
    }

    [Fact]
    public void CredentialsMustNotBeNull()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new BetfairHttpClient(null));

        Assert.Equal("credentials", ex.ParamName);
    }

    [Fact]
    public async Task PostReturnsErrorIfUriIsNull()
    {
        var result = await _client.Post<dynamic>(
            null,
            "token",
            Maybe<object>.None, default);

        AssertError(ErrorResult.Empty("uri"), result);
    }

    [Fact]
    public async Task PostReturnsErrorIfSessionTokenIsNull()
    {
        var result = await _client.Post<dynamic>(
            _uri,
            null);

        AssertError(ErrorResult.Empty("sessionToken"), result);
    }

    [Fact]
    public async Task PostReturnsErrorIfSessionTokenIsEmpty()
    {
        var result = await _client.Post<dynamic>(
            _uri,
            string.Empty);

        AssertError(ErrorResult.Empty("sessionToken"), result);
    }

    [Fact]
    public async Task PostReturnsErrorIfSessionTokenIsWhitespace()
    {
        var result = await _client.Post<dynamic>(
            _uri,
            " ");

        AssertError(ErrorResult.Empty("sessionToken"), result);
    }

    [Theory]
    [InlineData("http://test.com")]
    [InlineData("http://other.com")]
    public async Task PostUsesThePassedInUri(string uri)
    {
        await _client.Post<dynamic>(
            new Uri(uri),
            "token");

        _handler.AssertRequestMethod(HttpMethod.Post);
        _handler.AssertRequestUri(new Uri(uri));
    }

    [Theory]
    [InlineData("AppKey")]
    [InlineData("OtherKey")]
    public async Task PostPutsAppKeyIsInRequestHeader(string appKey)
    {
        var cred = Credentials.Create("username", "password", appKey).Value;
        using var client = new BetfairHttpClient(cred, _handler);

        await client.Post<dynamic>(
            _uri,
            "token");

        _handler.AssertContentHeader("X-Application", appKey);
    }

    [Theory]
    [InlineData("SessionToken")]
    [InlineData("OtherToken")]
    public async Task PostPutsSessionTokenIsInRequestHeader(string token)
    {
        await _client.Post<dynamic>(
            _uri,
            token);

        _handler.AssertContentHeader("X-Authentication", token);
    }

    [Fact]
    public async Task ReturnErrorIfPostIsNotOk()
    {
        _handler.SetResponseCode(HttpStatusCode.BadRequest);

        var result = await _client.Post<dynamic>(_uri, "token");

        AssertError(ErrorResult.Create(HttpStatusCode.BadRequest.ToString()), result);
    }

    [Theory]
    [InlineData("bodyContent")]
    public async Task PostPutsBodyInTheRequest(string body)
    {
        await _client.Post<dynamic>(_uri, "token", body);

        _handler.AssertRequestContent(body);
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
