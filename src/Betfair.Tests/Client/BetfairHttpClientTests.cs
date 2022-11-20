using System.Dynamic;
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
    private Uri _uri = new Uri("http://test.com/");

    public BetfairHttpClientTests()
    {
        var response = new Response { Token = "Token", Status = "SUCCESS", };
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
    public async Task LoginPostsToApiLoginEndpoint()
    {
        await _client.Login(default);

        _handler.AssertRequestMethod(HttpMethod.Post);
        _handler.AssertRequestUri(new Uri("https://identitysso.betfair.com/api/login"));
    }

    [Fact]
    public async Task LoginAcceptsApplicationJson()
    {
        await _client.Login(default);

        _handler.AssertRequestHeader("Accept", "application/json");
    }

    [Theory]
    [InlineData("AppKey")]
    [InlineData("Other")]
    public async Task HeadersContainAppKey(string appKey)
    {
        var cred = Credentials.Create("username", "password", appKey).Value;
        using var client = new BetfairHttpClient(cred, _handler);

        await client.Login(default);

        _handler.AssertRequestHeader("X-Application", appKey);
    }

    [Fact]
    public async Task LoginContentTypeIsFormUrlEncoded()
    {
        await _client.Login(default);

        _handler.AssertContentType("application/x-www-form-urlencoded");
    }

    [Theory]
    [InlineData("Username")]
    [InlineData("Other")]
    public async Task UsernameIsInFormContent(string username)
    {
        var cred = Credentials.Create(username, "password", "appKey").Value;
        using var client = new BetfairHttpClient(cred, _handler);

        await client.Login(default);

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
        using var client = new BetfairHttpClient(cred, _handler);

        await client.Login(default);

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

        var result = await _client.Login(default);

        Assert.Equal(token, result.Value);
    }

    [Theory]
    [InlineData("ERROR_MESSAGE")]
    [InlineData("INVALID_USERNAME_OR_PASSWORD")]
    public async Task LoginRespondsWithErrorIfNotSuccessful(string error)
    {
        var response = new Response { Status = "FAIL", Error = error };
        _handler.SetResponseBody(JsonSerializer.ToJsonString(response, StandardResolver.CamelCase));

        var result = await _client.Login(default);

        AssertError(ErrorResult.Create(error), result);
    }

    [Fact]
    public async Task LoginCallCertLoginUrlIfCertificateIsPresent()
    {
        using var cert = new X509Certificate2Stub();
        var cred = Credentials.CreateWithCert("username", "password", "appKey", cert).Value;
        using var client = new BetfairHttpClient(cred, _handler);

        await client.Login(default);

        _handler.AssertRequestUri(new Uri("https://identitysso-cert.betfair.com/api/certlogin"));
    }

    [Theory]
    [InlineData("Token")]
    [InlineData("Other")]
    public async Task CertLoginRespondsWithSessionToken(string token)
    {
        var response = new Response { SessionToken = token, LoginStatus = "SUCCESS", };
        _handler.SetResponseBody(JsonSerializer.ToJsonString(response, StandardResolver.CamelCase));

        var result = await _client.Login(default);

        Assert.Equal(token, result.Value);
    }

    [Theory]
    [InlineData("ERROR_MESSAGE")]
    [InlineData("INVALID_USERNAME_OR_PASSWORD")]
    public async Task CertLoginRespondsWithErrorIfNotSuccessful(string error)
    {
        var response = new Response { LoginStatus = error };
        _handler.SetResponseBody(JsonSerializer.ToJsonString(response, StandardResolver.CamelCase));

        var result = await _client.Login(default);

        AssertError(ErrorResult.Create(error), result);
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
