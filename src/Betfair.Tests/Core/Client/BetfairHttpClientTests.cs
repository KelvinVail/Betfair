using Betfair.Core.Client;
using Betfair.Core.Login;
using Betfair.Tests.Core.Client.TestDoubles;

namespace Betfair.Tests.Core.Client;

public sealed class BetfairHttpClientTests : IDisposable
{
    private readonly HttpMessageHandlerSpy _handler = new ();
    private readonly Credentials _credentials = new Credentials("username", "password", "appKey");
    private readonly BetfairHttpClient _client;
    private readonly Uri _uri = new ("http://test.com/");

    public BetfairHttpClientTests()
    {
        var response = new { Token = "Token", Status = "SUCCESS", };
        _handler.RespondsWithBody = response;

        _client = new BetfairHttpClient(_handler, _credentials);
    }

    [Fact]
    public void TimeoutIsSetToThirtySeconds() =>
        _client.Timeout.Should().Be(TimeSpan.FromSeconds(30));

    [Fact]
    public void AcceptsApplicationJson() =>
        _client.DefaultRequestHeaders.Accept.Should().Contain(
            new MediaTypeWithQualityHeaderValue("application/json"));

    [Fact]
    public void ConnectionIsKeptAlive() =>
        _client.DefaultRequestHeaders.Connection.Should().Contain("keep-alive");

    [Fact]
    public void AcceptGzipEncoding() =>
        _client.DefaultRequestHeaders.AcceptEncoding.Should().Contain(
            new StringWithQualityHeaderValue("gzip"));

    [Fact]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance",
        "CA1806:Do not ignore method results",
        Justification = "This is a test method.")]
    public void CredentialsMustNotBeNull()
    {
        Action ctor = () => new BetfairHttpClient(null!);
        ctor.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("credentials");

        Action ctor2 = () => new BetfairHttpClient(_handler, null!);
        ctor2.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("credentials");
    }

    [Fact]
    public async Task UriMustNotBeNull()
    {
        var ex = await _client.Invoking(x => x.Post<object>(null!))
            .Should().ThrowAsync<ArgumentNullException>();

        ex.And.ParamName.Should().Be("uri");
    }

    [Theory]
    [InlineData("http://test.com")]
    [InlineData("http://other.com")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design",
        "CA1054:URI-like parameters should not be strings",
        Justification = "This is a test method.")]
    public async Task PostUsesThePassedInUri(string uri)
    {
        await _client.Post<object>(new Uri(uri));

        _handler.MethodUsed.Should().Be(HttpMethod.Post);
        _handler.UriCalled.Should().Be(new Uri(uri));
    }

    [Fact]
    public async Task PostPutsContentTypeInContentHeader()
    {
        await _client.Post<dynamic>(_uri);

        _handler.ContentHeadersSent.Should().ContainKey("Content-Type")
            .WhoseValue.Should().Contain("application/json");
    }

    [Theory]
    [InlineData("AppKey")]
    [InlineData("OtherKey")]
    public async Task PostPutsAppKeyIsInContentHeader(string appKey)
    {
        var credentials = new Credentials("username", "password", appKey);
        using var client = new BetfairHttpClient(_handler, credentials);

        await client.Post<dynamic>(_uri);

        _handler.ContentHeadersSent.Should().ContainKey("X-Application")
            .WhoseValue.Should().Contain(appKey);
    }

    [Theory]
    [InlineData("SessionToken")]
    [InlineData("OtherToken")]
    public async Task PostPutsSessionTokenIsInContentHeader(string token)
    {
        _handler.RespondsWithBody = new { Token = token, Status = "SUCCESS", };

        await _client.Post<dynamic>(_uri);

        _handler.ContentHeadersSent.Should().ContainKey("X-Authentication")
            .WhoseValue.Should().Contain(token);
    }

    [Fact]
    public async Task ReturnErrorIfPostIsNotOk()
    {
        _handler.RespondsWitHttpStatusCode = HttpStatusCode.BadRequest;

        var ex = await _client.Invoking(x => x.Post<object>(_uri))
            .Should().ThrowAsync<HttpRequestException>();

        ex.And.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("bodyContent")]
    [InlineData("other")]
    public async Task PostPutsBodyInTheRequest(object body)
    {
        await _client.Post<dynamic>(_uri, body);

        _handler.ContentSent.Should().Be(body);
    }

    [Fact]
    public void DefaultHeadersAreConfigured()
    {
        using var client = new BetfairHttpClient(_credentials);

        client.Timeout.Should().Be(TimeSpan.FromSeconds(30));
        client.DefaultRequestHeaders.Should().ContainKey("Accept")
            .WhoseValue.Should().Contain("application/json");
        client.DefaultRequestHeaders.Should().ContainKey("Connection")
            .WhoseValue.Should().Contain("keep-alive");
        client.DefaultRequestHeaders.Should().ContainKey("Accept-Encoding")
            .WhoseValue.Should().Contain("gzip");
    }

    [Fact]
    public void CertShouldBeAddedToThenHandler()
    {
        using var cert = new X509Certificate2Stub();
        var credentials = new Credentials("username", "password", "appKey", cert);

        using var client = new BetfairHttpClient(_handler, credentials);

        _handler.ClientCertificates.Contains(cert).Should().BeTrue();
    }

    [Theory]
    [InlineData("appKey")]
    [InlineData("new-key")]
    public void AppKeyFromCredentialsIsExposed(string appKey)
    {
        var credentials = new Credentials("username", "password", appKey);

        using var client = new BetfairHttpClient(_handler, credentials);

        client.AppKey.Should().Be(appKey);
    }

    [Fact]
    public async Task GetTokenCallsBetfairLoginOnFirstExecution()
    {
        await _client.GetToken();

        _handler.UriCalled.Should().Be(new Uri("https://identitysso.betfair.com/api/login"));
    }

    [Fact]
    public async Task GetTokenOnlyCallsBetfairLoginOnFirstCall()
    {
        await _client.GetToken();
        await _client.GetToken();

        _handler.TimesUriCalled
            .Should().ContainKey(new Uri("https://identitysso.betfair.com/api/login"))
            .WhoseValue.Should().Be(1);
    }

    public void Dispose()
    {
        _handler.Dispose();
        _client.Dispose();
    }
}
