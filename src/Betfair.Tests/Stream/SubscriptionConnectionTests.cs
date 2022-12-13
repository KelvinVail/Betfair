using Betfair.Stream;
using Betfair.Tests.Stream.TestDoubles;

namespace Betfair.Tests.Stream;

public sealed class SubscriptionConnectionTests
{
    private readonly StreamClientStub _client = new ();
    private readonly Subscription _subscription;

    public SubscriptionConnectionTests() =>
        _subscription = new Subscription(_client);

    [Fact]
    public void StreamClientMustNotBeNull()
    {
        Action act = () => new Subscription(null);

        act.Should().Throw<ArgumentNullException>()
            .Where(x => x.ParamName == "client");
    }

    [Fact]
    public async Task SetConnectedToTrueWhenConnectionMessageIsReceived()
    {
        _client.SendLine(
            "{\"op\":\"connection\",\"connectionId\":\"ConnectionId\"}");

        await ProcessMessages();

        _subscription.Connected.Should().BeTrue();
    }

    [Theory]
    [InlineData("ConnectionId")]
    [InlineData("NewConnectionId")]
    [InlineData("RefreshedConnectionId")]
    public async Task ConnectionIdIsRecorded(string connectionId)
    {
        _client.SendLine(
            $"{{\"op\":\"connection\",\"connectionId\":\"{connectionId}\"}}");

        await ProcessMessages();

        _subscription.ConnectionId.Should().Be(connectionId);
    }

    [Fact]
    public async Task ConnectedIsFalseIfAnErrorMessageIsReceived()
    {
        _client.SendLine(
            "{\"op\":\"connection\",\"connectionId\":\"ConnectionId\"}");
        const string message = "{\"op\":\"status\"," +
                               "\"statusCode\":\"FAILURE\"," +
                               "\"errorCode\":\"TIMEOUT\"," +
                               "\"errorMessage\":\"Timed out trying to read message make sure to add \\\\r\\\\n\\nRead data : ﻿\"," +
                               "\"connectionClosed\":true," +
                               "\"connectionId\":\"ConnectionId\"}";
        _client.SendLine(message);

        await ProcessMessages();

        _subscription.Connected.Should().BeFalse();
    }

    //[Fact]
    //public async Task OnReadStatusUpdateConnectionStatusIsUpdated()
    //{
    //    await SendLineAsync("{\"op\":\"connection\",\"connectionId\":\"ConnectionId\"}");
    //    await SendLineAsync("{\"op\":\"status\",\"connectionClosed\":false}");
    //    Assert.True(Subscription.Connected);

    //    await SendLineAsync("{\"op\":\"status\",\"connectionClosed\":true}");
    //    Assert.False(Subscription.Connected);
    //}

    //[Fact]
    //public async Task OnReadHandleUnknownOperationWithoutThrowing()
    //{
    //    await SendLineAsync("{\"op\":\"unknown\"}");
    //}

    //[Fact]
    //public async Task OnAuthenticateGetSessionTokenIsCalled()
    //{
    //    await Subscription.Authenticate();
    //    Assert.Equal(1, Session.TimesGetSessionTokenAsyncCalled);
    //}

    //[Fact]
    //public async Task OnAuthenticate()
    //{
    //    var authMessage = JsonSerializer.ToJsonString(
    //        new AuthenticationMessageStub(
    //            Session.AppKey,
    //            await Session.GetTokenAsync()));

    //    await Subscription.Authenticate();
    //    Assert.Equal(authMessage, Writer.LastLineWritten);
    //}

    private async Task ProcessMessages()
    {
        await foreach (var unused in _subscription.GetChanges())
        {
            var a = 1;
        }
    }
}