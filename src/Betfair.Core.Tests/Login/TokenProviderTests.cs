using Betfair.Core.Client;
using Betfair.Core.Login;
using Betfair.Core.Tests.Client.TestDoubles;

namespace Betfair.Core.Tests.Login;

public class TokenProviderTests : IDisposable
{
    private readonly HttpMessageHandlerSpy _handler = new ();
    private readonly BetfairHttpClient _client;
    private readonly Credentials _cred = new ("username", "password", "appKey");
    private bool _disposedValue;

    public TokenProviderTests()
    {
        _client = new BetfairHttpClient(_handler, _cred);
        _handler.RespondsWithBody = new { Token = "Token", Status = "SUCCESS", };
    }

    [Fact]
    public void CredentialsMustNotBeNull()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new BetfairHttpClient(null!));

        ex.ParamName.Should().Be("credentials");
    }

    [Fact]
    public async Task GetTokenPostsToApiLoginEndpoint()
    {
        await _client.GetToken(default);

        _handler.MethodUsed.Should().Be(HttpMethod.Post);
        _handler.UriCalled.Should().Be(new Uri("https://identitysso.betfair.com/api/login"));
    }

    [Fact]
    public async Task LoginContentTypeIsFormUrlEncoded()
    {
        await _client.GetToken(default);

        _handler.ContentHeadersSent?.ContentType.Should().Be(
            new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
    }

    [Theory]
    [InlineData("AppKey")]
    [InlineData("Other")]
    public async Task HeadersContainAppKey(string appKey)
    {
        var cred = new Credentials("username", "password", appKey);
        using var provider = new BetfairHttpClient(_handler, cred);

        await provider.GetToken(default);

        _handler.HeadersSent.Should().ContainKey("X-Application")
            .WhoseValue.Should().BeEquivalentTo(appKey);
    }

    [Theory]
    [InlineData("Username")]
    [InlineData("Other")]
    public async Task UsernameIsInFormContent(string username)
    {
        var cred = new Credentials(username, "password", "appKey");
        using var provider = new BetfairHttpClient(_handler, cred);

        await provider.GetToken(default);

        using var content = new FormUrlEncodedContent(
            new Dictionary<string, string> { { "username", username }, { "password", "password" } });
        var expected = await content.ReadAsStringAsync();

        _handler.ContentSent.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("Password")]
    [InlineData("Other")]
    public async Task PasswordIsInFormContent(string password)
    {
        var cred = new Credentials("username", password, "appKey");
        using var provider = new BetfairHttpClient(_handler, cred);

        await provider.GetToken(default);

        using var content = new FormUrlEncodedContent(
            new Dictionary<string, string> { { "username", "username" }, { "password", password } });
        var expected = await content.ReadAsStringAsync();

        _handler.ContentSent.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("Token")]
    [InlineData("Other")]
    public async Task LoginRespondsWithSessionToken(string token)
    {
        _handler.RespondsWithBody = new { Token = token, Status = "SUCCESS", };

        var result = await _client.GetToken(default);

        result.Should().Be(token);
    }

    [Theory]
    [InlineData("ERROR_MESSAGE")]
    [InlineData("INVALID_USERNAME_OR_PASSWORD")]
    public async Task LoginRespondsWithErrorIfNotSuccessful(string error)
    {
        _handler.RespondsWithBody = new { Status = "FAIL", Error = error };

        var ex = await Assert.ThrowsAsync<BetfairRequestException>(() =>
            _client.GetToken(default));

        ex.Message.Should().Be(error);
    }

    [Fact]
    public async Task LoginCallsCertLoginUrlIfCertificateIsPresent()
    {
        using var cert = new X509Certificate2Stub();
        var cred = new Credentials("username", "password", "appKey", cert);
        using var provider = new BetfairHttpClient(_handler, cred);

        await provider.GetToken(default);

        _handler.UriCalled.Should().Be(
            new Uri("https://identitysso-cert.betfair.com/api/certlogin"));
    }

    [Theory]
    [InlineData("Token")]
    [InlineData("Other")]
    public async Task CertLoginRespondsWithSessionToken(string token)
    {
        _handler.RespondsWithBody = new { SessionToken = token, LoginStatus = "SUCCESS", };

        var result = await _client.GetToken(default);

        result.Should().Be(token);
    }

    [Theory]
    [InlineData("ERROR_MESSAGE")]
    [InlineData("INVALID_USERNAME_OR_PASSWORD")]
    public async Task CertLoginRespondsWithErrorIfNotSuccessful(string error)
    {
        _handler.RespondsWithBody = new { LoginStatus = error };

        var ex = await Assert.ThrowsAsync<BetfairRequestException>(() =>
            _client.GetToken(default));

        ex.Message.Should().Be(error);
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
