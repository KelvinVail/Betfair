using System.Threading.Tasks;
using Betfair.Stream.Tests.TestDoubles;
using Utf8Json;
using Xunit;

namespace Betfair.Stream.Tests
{
    public sealed class SubscriptionConnectionTests : SubscriptionTests
    {
        [Fact]
        public async Task OnReadConnectionOperationConnectedIsTrue()
        {
            Assert.False(Subscription.Connected);
            await SendLineAsync("{\"op\":\"connection\",\"connectionId\":\"ConnectionId\"}");
            Assert.True(Subscription.Connected);
        }

        [Theory]
        [InlineData("ConnectionId")]
        [InlineData("NewConnectionId")]
        [InlineData("RefreshedConnectionId")]
        public async Task OnReadConnectionOperationConnectionIdIsRecorded(string connectionId)
        {
            await SendLineAsync($"{{\"op\":\"connection\",\"connectionId\":\"{connectionId}\"}}");
            Assert.Equal(connectionId, Subscription.ConnectionId);
        }

        [Fact]
        public async Task OnReadStatusTimeoutOperationConnectedIsFalse()
        {
            await SendLineAsync("{\"op\":\"connection\",\"connectionId\":\"ConnectionId\"}");

            var message =
                "{\"op\":\"status\"," +
                "\"statusCode\":\"FAILURE\"," +
                "\"errorCode\":\"TIMEOUT\"," +
                "\"errorMessage\":\"Timed out trying to read message make sure to add \\\\r\\\\n\\nRead data : ﻿\"," +
                "\"connectionClosed\":true," +
                "\"connectionId\":\"ConnectionId\"}";

            await SendLineAsync(message);
            Assert.False(Subscription.Connected);
        }

        [Fact]
        public async Task OnReadStatusUpdateConnectionStatusIsUpdated()
        {
            await SendLineAsync("{\"op\":\"connection\",\"connectionId\":\"ConnectionId\"}");
            await SendLineAsync("{\"op\":\"status\",\"connectionClosed\":false}");
            Assert.True(Subscription.Connected);

            await SendLineAsync("{\"op\":\"status\",\"connectionClosed\":true}");
            Assert.False(Subscription.Connected);
        }

        [Fact]
        public async Task OnReadHandleUnknownOperationWithoutThrowing()
        {
            await SendLineAsync("{\"op\":\"unknown\"}");
        }

        [Fact]
        public async Task OnAuthenticateGetSessionTokenIsCalled()
        {
            await Subscription.Authenticate();
            Assert.Equal(1, Session.TimesGetSessionTokenAsyncCalled);
        }

        [Fact]
        public async Task OnAuthenticate()
        {
            var authMessage = JsonSerializer.ToJsonString(
                new AuthenticationMessageStub(
                    Session.AppKey,
                    await Session.GetTokenAsync()));

            await Subscription.Authenticate();
            Assert.Equal(authMessage, Writer.LastLineWritten);
        }
    }
}
