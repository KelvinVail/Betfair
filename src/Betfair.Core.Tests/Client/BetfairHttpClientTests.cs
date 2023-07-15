using Betfair.Core.Client;
using Betfair.Core.Tests.Client.TestDoubles;

namespace Betfair.Core.Tests.Client;

public sealed class BetfairHttpClientTests : IDisposable
{
    private readonly HttpMessageHandlerSpy _handler = new ();
    private readonly BetfairHttpClient _client;
    private readonly Uri _uri = new ("http://test.com/");

    public BetfairHttpClientTests()
    {
        var response = new { Token = "Token", Status = "SUCCESS", };
        _handler.RespondsWithBody = response;

        _client = new BetfairHttpClient(_handler, "appKey");
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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "This is a test method.")]
    public void AppKeyMustNotBeNull()
    {
        Action ctor = () => new BetfairHttpClient(null!);
        ctor.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("appKey");

        Action ctor2 = () => new BetfairHttpClient(_handler, null!);
        ctor2.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("appKey");
    }

    [Fact]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "This is a test method.")]
    public void AppKeyMustNotBeEmpty()
    {
        Action ctor = () => new BetfairHttpClient(string.Empty);
        ctor.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("appKey");

        Action ctor2 = () => new BetfairHttpClient(_handler, string.Empty);
        ctor2.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("appKey");
    }

    [Fact]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "This is a test method.")]
    public void AppKeyMustNotBeWhiteSpace()
    {
        Action ctor = () => new BetfairHttpClient(" ");
        ctor.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("appKey");

        Action ctor2 = () => new BetfairHttpClient(_handler, " ");
        ctor2.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("appKey");
    }

    [Fact]
    public async Task UriMustNotBeNull()
    {
        var ex = await _client.Invoking(x => x.Post<object>(null!, "token"))
            .Should().ThrowAsync<ArgumentNullException>();

        ex.And.ParamName.Should().Be("uri");
    }

    [Fact]
    public async Task SessionTokenMustNotBeNull()
    {
        var ex = await _client.Invoking(x => x.Post<object>(_uri, null!))
            .Should().ThrowAsync<ArgumentNullException>();

        ex.And.ParamName.Should().Be("sessionToken");
    }

    [Fact]
    public async Task SessionTokenMustNotBeEmpty()
    {
        var ex = await _client.Invoking(x => x.Post<object>(_uri, string.Empty))
            .Should().ThrowAsync<ArgumentNullException>();

        ex.And.ParamName.Should().Be("sessionToken");
    }

    [Fact]
    public async Task SessionTokenMustNotBeWhiteSpace()
    {
        var ex = await _client.Invoking(x => x.Post<object>(_uri, " "))
            .Should().ThrowAsync<ArgumentNullException>();

        ex.And.ParamName.Should().Be("sessionToken");
    }

    [Theory]
    [InlineData("http://test.com")]
    [InlineData("http://other.com")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1054:URI-like parameters should not be strings", Justification = "This is a test method.")]
    public async Task PostUsesThePassedInUri(string uri)
    {
        await _client.Post<object>(new Uri(uri), "token");

        _handler.MethodUsed.Should().Be(HttpMethod.Post);
        _handler.UriCalled.Should().Be(new Uri(uri));
    }

    [Fact]
    public async Task PostPutsContentTypeInContentHeader()
    {
        await _client.Post<dynamic>(_uri, "token");

        _handler.ContentHeadersSent.Should().ContainKey("Content-Type")
            .WhoseValue.Should().Contain("application/json");
    }

    [Theory]
    [InlineData("AppKey")]
    [InlineData("OtherKey")]
    public async Task PostPutsAppKeyIsInContentHeader(string appKey)
    {
        using var client = new BetfairHttpClient(_handler, appKey);

        await client.Post<dynamic>(_uri, "token");

        _handler.ContentHeadersSent.Should().ContainKey("X-Application")
            .WhoseValue.Should().Contain(appKey);
    }

    [Theory]
    [InlineData("SessionToken")]
    [InlineData("OtherToken")]
    public async Task PostPutsSessionTokenIsInContentHeader(string token)
    {
        await _client.Post<dynamic>(_uri, token);

        _handler.ContentHeadersSent.Should().ContainKey("X-Authentication")
            .WhoseValue.Should().Contain(token);
    }

    [Fact]
    public async Task ReturnErrorIfPostIsNotOk()
    {
        _handler.RespondsWitHttpStatusCode = HttpStatusCode.BadRequest;

        var ex = await _client.Invoking(x => x.Post<object>(_uri, "token"))
            .Should().ThrowAsync<BetfairRequestException>();

        ex.And.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("bodyContent")]
    [InlineData("other")]
    public async Task PostPutsBodyInTheRequest(object body)
    {
        await _client.Post<dynamic>(_uri, "token", body);

        _handler.ContentSent.Should().Be(body);
    }

    [Fact]
    public void DefaultHeadersAreConfigure()
    {
        using var client = new BetfairHttpClient("appKey");

        client.Timeout.Should().Be(TimeSpan.FromSeconds(30));
    }

    public void Dispose()
    {
        _handler.Dispose();
        _client.Dispose();
    }
}
