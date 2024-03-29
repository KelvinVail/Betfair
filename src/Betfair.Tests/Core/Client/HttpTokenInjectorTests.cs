﻿using System.Security.AccessControl;
using Betfair.Core.Client;
using Betfair.Tests.Core.Client.TestDoubles;
using Betfair.Tests.TestDoubles;

namespace Betfair.Tests.Core.Client;

public class HttpTokenInjectorTests : IDisposable
{
    private readonly HttpClientStub _httpClient = new ();
    private readonly HttpContent _content = new StringContent("content");
    private readonly TokenProviderStub _tokenProvider = new ();
    private readonly Uri _uri = new ("http://test.com");
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

        _httpClient.HttpContentSent?.Headers.Should().ContainKey("X-Application")
            .WhoseValue.Should().Contain(appKey);
    }

    [Theory]
    [InlineData("SessionToken")]
    [InlineData("OtherToken")]
    public async Task PostPutsSessionTokenIsInContentHeader(string token)
    {
        _tokenProvider.RespondsWithToken.Add(token);

        await _tokenInjector.PostAsync<dynamic>(_uri, _content);

        _httpClient.HttpContentSent?.Headers.Should().ContainKey("X-Authentication")
            .WhoseValue.Should().Contain(token);
    }

    [Fact]
    public async Task TokenIsReusedIfValid()
    {
        await _tokenInjector.PostAsync<dynamic>(_uri, _content);
        await _tokenInjector.PostAsync<dynamic>(_uri, _content);

        _tokenProvider.TokensUsed.Should().Be(1);
    }

    [Fact]
    public async Task TokenIsRefreshedIfSessionIsInvalid()
    {
        _httpClient.ThrowsError = "INVALID_SESSION_INFORMATION";

        await _tokenInjector.PostAsync<dynamic>(_uri, _content);

        _tokenProvider.TokensUsed.Should().Be(2);
    }

    [Theory]
    [InlineData("FirstToken", "SecondToken")]
    [InlineData("OtherToken", "NewToken")]
    public async Task IfSessionIsInvalidANewTokenIsAddedToTheRequest(string first, string second)
    {
        _tokenProvider.RespondsWithToken.Add(first);
        _tokenProvider.RespondsWithToken.Add(second);
        _httpClient.ThrowsError = "INVALID_SESSION_INFORMATION";

        await _tokenInjector.PostAsync<dynamic>(_uri, _content);

        _httpClient.HttpContentSent?.Headers.Should().ContainKey("X-Authentication")
            .WhoseValue.Should().NotContain(first);
        _httpClient.HttpContentSent?.Headers.Should().ContainKey("X-Authentication")
            .WhoseValue.Should().Contain(second);
    }

    [Fact]
    public async Task DoNotRefreshTheTokenAndRethrowTheErrorIfInvalidSessionIsNotThrown()
    {
        _httpClient.ThrowsError = "OTHER_ERROR";

        var act = async () => { await _tokenInjector.PostAsync<dynamic>(_uri, _content); };

        (await act.Should().ThrowAsync<HttpRequestException>())
            .WithMessage("OTHER_ERROR");
        _tokenProvider.TokensUsed.Should().Be(1);
    }

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
}
