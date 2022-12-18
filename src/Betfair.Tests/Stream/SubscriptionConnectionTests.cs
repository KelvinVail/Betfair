//using Betfair.Login;
//using Betfair.Stream;
//using Betfair.Stream.Responses;
//using Betfair.Tests.Stream.TestDoubles;

//namespace Betfair.Tests.Stream;

//public class SubscriptionConnectionTests : IDisposable
//{
    //private readonly TcpClientSpy _tcpClient = new ("test", 999);
    //private readonly StreamClientStub _client;
    //private readonly Credentials _credentials = Credentials.Create("username", "password", "appKey").Value;
    //private readonly Subscription _subscription;
    //private bool _disposedValue;

    //public SubscriptionConnectionTests()
    //{
    //    _client = new StreamClientStub(_tcpClient);
    //    _subscription = new Subscription(_client, _credentials);
    //}

    //[Fact]
    //public void StreamClientMustNotBeNull()
    //{
    //    Action act = () => new Subscription(null, _credentials);

    //    act.Should().Throw<ArgumentNullException>()
    //        .Where(x => x.ParamName == "client");
    //}

    //[Fact]
    //public void CredentialsMustNotBeNull()
    //{
    //    Action act = () => new Subscription(_client, null);

    //    act.Should().Throw<ArgumentNullException>()
    //        .Where(x => x.ParamName == "credentials");
    //}

    //[Fact]
    //public async Task SetConnectedToTrueWhenConnectionMessageIsReceived()
    //{
    //    _client.Response = new ChangeMessage
    //    {
    //        Operation = "connection",
    //        ConnectionId = "ConnectionId",
    //    };

    //    await ProcessMessages();

    //    _subscription.Connected.Should().BeTrue();
    //}

    //[Theory]
    //[InlineData("ConnectionId")]
    //[InlineData("NewConnectionId")]
    //[InlineData("RefreshedConnectionId")]
    //public async Task ConnectionIdIsRecorded(string connectionId)
    //{
    //    _client.Response = new ChangeMessage
    //    {
    //        Operation = "connection",
    //        ConnectionId = connectionId,
    //    };

    //    await ProcessMessages();

    //    _subscription.ConnectionId.Should().Be(connectionId);
    //}

    //[Fact]
    //public async Task ConnectedIsFalseIfAnErrorMessageIsReceived()
    //{
    //    _client.SendLine(
    //        "{\"op\":\"connection\",\"connectionId\":\"ConnectionId\"}");
    //    const string message = "{\"op\":\"status\"," +
    //                           "\"statusCode\":\"FAILURE\"," +
    //                           "\"errorCode\":\"TIMEOUT\"," +
    //                           "\"errorMessage\":\"Timed out trying to read message make sure to add \\\\r\\\\n\\nRead data : ﻿\"," +
    //                           "\"connectionClosed\":true," +
    //                           "\"connectionId\":\"ConnectionId\"}";
    //    _client.SendLine(message);

    //    await ProcessMessages();

    //    _subscription.Connected.Should().BeFalse();
    //}

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

    //public void Dispose()
    //{
    //    Dispose(disposing: true);
    //    GC.SuppressFinalize(this);
    //}

    //protected virtual void Dispose(bool disposing)
    //{
    //    if (_disposedValue)
    //        return;

    //    if (disposing)
    //    {
    //        _client.Dispose();
    //        _tcpClient.Dispose();
    //    }

    //    _disposedValue = true;
    //}

    //private async Task ProcessMessages()
    //{
    //    await foreach (var unused in _subscription.GetChanges())
    //    {
    //        var a = 1;
    //    }
    //}
//}