using Betfair.Errors;

using Betfair.Login;
using Betfair.Stream;
using Betfair.Stream.Messages;
using Betfair.Stream.Responses;
using Betfair.Tests.Errors;
using Betfair.Tests.Stream.TestDoubles;

namespace Betfair.Tests.Stream;

public class AuthenticateTests : IDisposable
{
    private readonly TcpClientSpy _tcpClient = new ("test", 999);
    private readonly StreamClientStub _client;
    private readonly Credentials _credentials = Credentials.Create("username", "password", "appKey").Value;
    private readonly Subscription _subscription;
    private bool _disposedValue;

    public AuthenticateTests()
    {
        _client = new StreamClientStub(_tcpClient);
        _subscription = new Subscription(_client, _credentials);
        _client.Response = new StatusMessage
        {
            Op = "connection",
            StatusCode = "SUCCESS",
        };
    }

    [Fact]
    public async Task TokenMustNotBeNull()
    {
        var result = await _subscription.Authenticate(null);

        result.ShouldBeFailure(ErrorResult.Empty("token"));
    }

    [Fact]
    public async Task TokenMustNotBeEmpty()
    {
        var result = await _subscription.Authenticate(string.Empty);

        result.ShouldBeFailure(ErrorResult.Empty("token"));
    }

    [Fact]
    public async Task TokenMustNotBeWhiteSpace()
    {
        var result = await _subscription.Authenticate(" ");

        result.ShouldBeFailure(ErrorResult.Empty("token"));
    }

    [Fact]
    public async Task AuthenticationMessageIsSent()
    {
        var expected = new Authentication
        {
            Id = 1,
            AppKey = "appKey",
            Session = "token",
        };

        await _subscription.Authenticate("token");

        _client.SentMessages.First().Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("token")]
    [InlineData("newToken")]
    [InlineData("session")]
    public async Task AuthenticationMessageContainsSessionToken(string token)
    {
        var expected = new Authentication
        {
            Id = 1,
            AppKey = "appKey",
            Session = token,
        };

        await _subscription.Authenticate(token);

        _client.SentMessages.First().Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("appKey")]
    [InlineData("newKey")]
    [InlineData("other")]
    public async Task AuthenticationMessageContainsAppKey(string appKey)
    {
        var expected = new Authentication
        {
            Id = 1,
            AppKey = appKey,
            Session = "token",
        };
        var cred = Credentials.Create("username", "password", appKey).Value;
        var sub = new Subscription(_client, cred);

        await sub.Authenticate("token");

        _client.SentMessages.First().Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task SuccessResultReturnedIfSuccessful()
    {
        _client.Response = new StatusMessage
        {
            Op = "connection",
            StatusCode = "SUCCESS",
        };

        var result = await _subscription.Authenticate("token");

        result.ShouldBeSuccess();
    }

    [Fact]
    public async Task ErrorResultReturnedIfError()
    {
        _client.Response = new StatusMessage
        {
            Op = "connection",
            StatusCode = "FAILURE",
            ErrorCode = "NO_APP_KEY",
        };

        var result = await _subscription.Authenticate("token");

        result.ShouldBeFailure(ErrorResult.Create("NO_APP_KEY"));
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
        {
            _client.Dispose();
            _tcpClient.Dispose();
        }

        _disposedValue = true;
    }
}
