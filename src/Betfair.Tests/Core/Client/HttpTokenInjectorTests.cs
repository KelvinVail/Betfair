using Betfair.Core.Client;
using Betfair.Tests.TestDoubles;
using HttpClientStub = Betfair.Tests.Core.Client.TestDoubles.HttpClientStub;

namespace Betfair.Tests.Core.Client;

public class HttpTokenInjectorTests : IDisposable
{
    private readonly HttpClientStub _httpClient = new ();
    private readonly HttpContent _content = new StringContent("content");
    private readonly TokenProviderStub _tokenProvider = new ();
    private readonly Uri _uri = new Uri("http://test.com");
    private readonly HttpTokenInjector _tokenInjector;
    private bool _disposedValue;

    public HttpTokenInjectorTests() =>
        _tokenInjector = new HttpTokenInjector(_httpClient, _tokenProvider, "appKey");

    [Theory]
    [InlineData("AppKey")]
    [InlineData("OtherKey")]
    public async Task PostPutsAppKeyIsInContentHeader(string appKey)
    {
        var client = new HttpTokenInjector(_httpClient, _tokenProvider, appKey);

        await client.PostAsync<dynamic>(_uri, _content);

        _httpClient.HttpContentSent.Headers.Should().ContainKey("X-Application")
            .WhoseValue.Should().Contain(appKey);
    }

    [Theory]
    [InlineData("SessionToken")]
    [InlineData("OtherToken")]
    public async Task PostPutsSessionTokenIsInContentHeader(string token)
    {
        _tokenProvider.RespondsWithToken = token;

        await _tokenInjector.PostAsync<dynamic>(_uri, _content);

        _httpClient.HttpContentSent.Headers.Should().ContainKey("X-Authentication")
            .WhoseValue.Should().Contain(token);
    }

    [Fact]
    public async Task TokenIsReusedIfValid()
    {
        await _tokenInjector.PostAsync<dynamic>(_uri, _content);
        await _tokenInjector.PostAsync<dynamic>(_uri, _content);

        _tokenProvider.TokensUsed.Should().Be(1);
    }

    //[Fact]
    //public async Task TokenIsRefreshedIfSessionIsInvalid()
    //{
    //    _handler.RespondsWitHttpStatusCode = HttpStatusCode.BadRequest;
    //    _handler.RespondsWithBody = new BadRequestResponse("INVALID_SESSION_INFORMATION");

    //    await _client.PostAsync<dynamic>(_uri);

    //    _provider.TokensUsed.Should().Be(2);
    //}

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        if (disposing) _content.Dispose();

        _disposedValue = true;
    }

    private sealed class BadRequestResponse
    {
        public BadRequestResponse(string errorCode) =>
            Detail.ApiNgException.ErrorCode = errorCode;

        public Detail Detail { get; } = new ();
    }

    private sealed class Detail
    {
        public ApiNgException ApiNgException { get; } = new ();
    }

    private sealed class ApiNgException
    {
        public string? ErrorCode { get; set; }
    }
}
