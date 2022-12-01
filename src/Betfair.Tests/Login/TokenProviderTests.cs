using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using Betfair.Errors;
using Betfair.Login;
using Betfair.Tests.Errors;
using Betfair.Tests.TestDoubles;
using FluentAssertions;
using Utf8Json;
using Utf8Json.Resolvers;

namespace Betfair.Tests.Login;

public class TokenProviderTests : IDisposable
{
    private readonly BetfairHttpClientFake _client = new ();
    private readonly Credentials _cred = Credentials.Create("username", "password", "appKey").Value;
    private readonly TokenProvider _tokenProvider;
    private bool _disposedValue;

    public TokenProviderTests()
    {
        var response = new LoginResponse { Token = "Token", Status = "SUCCESS", };
        _client.SetResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.ToJsonString(response, StandardResolver.CamelCase)),
        };

        _tokenProvider = new TokenProvider(_client, _cred);
    }

    [Fact]
    public void ClientMustNotBeNull()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new TokenProvider(null, _cred));

        Assert.Equal("client", ex.ParamName);
    }

    [Fact]
    public void CredentialsMustNotBeNull()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new TokenProvider(_client, null));

        Assert.Equal("credentials", ex.ParamName);
    }

    [Fact]
    public async Task GetTokenPostsToApiLoginEndpoint()
    {
        await _tokenProvider.GetToken(default);

        _client.LastMessageSent.Method.Should().Be(HttpMethod.Post);
        _client.LastMessageSent.RequestUri.Should().Be(new Uri("https://identitysso.betfair.com/api/login"));
    }

    [Fact]
    public async Task LoginContentTypeIsFormUrlEncoded()
    {
        await _tokenProvider.GetToken(default);

        _client.LastMessageSent.Content!.Headers.ContentType.Should().Be(
            new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
    }

    [Theory]
    [InlineData("AppKey")]
    [InlineData("Other")]
    public async Task HeadersContainAppKey(string appKey)
    {
        var cred = Credentials.Create("username", "password", appKey).Value;
        var provider = new TokenProvider(_client, cred);

        await provider.GetToken(default);

        _client.LastMessageSent.Headers.Should().ContainKey("X-Application").WhoseValue.Should().BeEquivalentTo(appKey);
    }

    [Theory]
    [InlineData("Username")]
    [InlineData("Other")]
    public async Task UsernameIsInFormContent(string username)
    {
        var cred = Credentials.Create(username, "password", "appKey").Value;
        var provider = new TokenProvider(_client, cred);

        await provider.GetToken(default);

        using var content = new FormUrlEncodedContent(
            new Dictionary<string, string> { { "username", username }, { "password", "password" } });
        var expected = await content.ReadAsStringAsync();

        _client.LastBodySent.Should().Be(expected);
    }

    [Theory]
    [InlineData("Password")]
    [InlineData("Other")]
    public async Task PasswordIsInFormContent(string password)
    {
        var cred = Credentials.Create("username", password, "appKey").Value;
        var provider = new TokenProvider(_client, cred);

        await provider.GetToken(default);

        using var content = new FormUrlEncodedContent(
            new Dictionary<string, string> { { "username", "username" }, { "password", password } });
        var expected = await content.ReadAsStringAsync();

        _client.LastBodySent.Should().Be(expected);
    }

    [Theory]
    [InlineData("Token")]
    [InlineData("Other")]
    public async Task LoginRespondsWithSessionToken(string token)
    {
        var response = new LoginResponse { Token = token, Status = "SUCCESS", };
        _client.SetResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.ToJsonString(response, StandardResolver.CamelCase)),
        };

        var result = await _tokenProvider.GetToken(default);

        result.ShouldBeSuccess();
        result.Value.Should().Be(token);
    }

    [Theory]
    [InlineData("ERROR_MESSAGE")]
    [InlineData("INVALID_USERNAME_OR_PASSWORD")]
    public async Task LoginRespondsWithErrorIfNotSuccessful(string error)
    {
        var response = new LoginResponse { Status = "FAIL", Error = error };
        _client.SetResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.ToJsonString(response, StandardResolver.CamelCase)),
        };

        var result = await _tokenProvider.GetToken(default);

        result.ShouldBeFailure(ErrorResult.Create(error));
    }

    [Fact]
    public async Task LoginCallCertLoginUrlIfCertificateIsPresent()
    {
        using var cert = new X509Certificate2Stub();
        var cred = Credentials.Create("username", "password", "appKey", cert).Value;
        var provider = new TokenProvider(_client, cred);

        await provider.GetToken(default);

        _client.LastMessageSent.RequestUri.Should().Be(new Uri("https://identitysso-cert.betfair.com/api/certlogin"));
    }

    [Theory]
    [InlineData("Token")]
    [InlineData("Other")]
    public async Task CertLoginRespondsWithSessionToken(string token)
    {
        var response = new LoginResponse { SessionToken = token, LoginStatus = "SUCCESS", };
        _client.SetResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.ToJsonString(response, StandardResolver.CamelCase)),
        };

        var result = await _tokenProvider.GetToken(default);

        result.ShouldBeSuccess();
        result.Value.Should().Be(token);
    }

    [Theory]
    [InlineData("ERROR_MESSAGE")]
    [InlineData("INVALID_USERNAME_OR_PASSWORD")]
    public async Task CertLoginRespondsWithErrorIfNotSuccessful(string error)
    {
        var response = new LoginResponse { LoginStatus = error };
        _client.SetResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.ToJsonString(response, StandardResolver.CamelCase)),
        };

        var result = await _tokenProvider.GetToken(default);

        result.ShouldBeFailure(ErrorResult.Create(error));
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue)
            return;

        if (disposing)
            _client.Dispose();

        _disposedValue = true;
    }
}
