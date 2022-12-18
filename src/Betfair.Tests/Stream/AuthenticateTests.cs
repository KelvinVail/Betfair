using Betfair.Errors;
using Betfair.Login;
using Betfair.Stream;
using Betfair.Stream.Messages;
using Betfair.Stream.Responses;
using Betfair.Tests.Errors;
using Betfair.Tests.Stream.TestDoubles;

namespace Betfair.Tests.Stream;

public class AuthenticateTests
{
    private readonly PipelineSpy _pipe;
    private readonly Credentials _credentials = Credentials.Create("username", "password", "appKey").Value;
    private readonly Subscription _subscription;
    private readonly List<ChangeMessage> _messages = new ();

    public AuthenticateTests()
    {
        _pipe = new PipelineSpy();
        _subscription = new Subscription(_pipe, _credentials);
    }

    [Fact]
    public async Task TokenMustNotBeNull()
    {
        await _subscription.Authenticate(null);

        _subscription.Status.ShouldBeFailure(ErrorResult.Empty("token"));
    }

    [Fact]
    public async Task TokenMustNotBeEmpty()
    {
        await _subscription.Authenticate(string.Empty);

        _subscription.Status.ShouldBeFailure(ErrorResult.Empty("token"));
    }

    [Fact]
    public async Task TokenMustNotBeWhiteSpace()
    {
        await _subscription.Authenticate(" ");

        _subscription.Status.ShouldBeFailure(ErrorResult.Empty("token"));
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

        _pipe.WrittenObjects.First().Should().BeEquivalentTo(expected);
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

        _pipe.WrittenObjects.First().Should().BeEquivalentTo(expected);
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
        var sub = new Subscription(_pipe, cred);

        await sub.Authenticate("token");

        _pipe.WrittenObjects.First().Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task SuccessResultReturnedIfSuccessful()
    {
        await _subscription.Authenticate("token");

        _subscription.Status.ShouldBeSuccess();
    }

    [Fact]
    public async Task ErrorResultReturnedIfError()
    {
        _pipe.Responses.Add(new ChangeMessage
        {
            Operation = "connection",
            StatusCode = "FAILURE",
            ErrorCode = "NO_APP_KEY",
        });

        await _subscription.Authenticate("token");
        await foreach (var line in _subscription.GetChanges())
            _messages.Add(line);

        _subscription.Status.ShouldBeFailure(ErrorResult.Create("NO_APP_KEY"));
    }

    [Fact]
    public async Task ErrorResultReturnedIfStatusCodeIsNull()
    {
        _pipe.Responses.Add(new ChangeMessage
        {
            Operation = "connection",
        });

        await _subscription.Authenticate("token");
        await foreach (var line in _subscription.GetChanges())
            _messages.Add(line);

        _subscription.Status.ShouldBeFailure(ErrorResult.Create("CONNECTION_ERROR"));
    }
}
