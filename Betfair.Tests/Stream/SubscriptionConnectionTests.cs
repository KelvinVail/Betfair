namespace Betfair.Tests.Stream
{
    using System.Threading.Tasks;
    using Betfair.Tests.Stream.TestDoubles;
    using Newtonsoft.Json;
    using Xunit;

    public sealed class SubscriptionConnectionTests : SubscriptionTests
    {
        [Fact]
        public async Task OnReadConnectionOperationConnectedIsTrue()
        {
            Assert.False(this.Subscription.Connected);
            await this.SendLineAsync("{\"op\":\"connection\",\"connectionId\":\"ConnectionId\"}");
            Assert.True(this.Subscription.Connected);
        }

        [Theory]
        [InlineData("ConnectionId")]
        [InlineData("NewConnectionId")]
        [InlineData("RefreshedConnectionId")]
        public async Task OnReadConnectionOperationConnectionIdIsRecorded(string connectionId)
        {
            await this.SendLineAsync($"{{\"op\":\"connection\",\"connectionId\":\"{connectionId}\"}}");
            Assert.Equal(connectionId, this.Subscription.ConnectionId);
        }

        [Fact]
        public async Task OnReadStatusTimeoutOperationConnectedIsFalse()
        {
            await this.SendLineAsync("{\"op\":\"connection\",\"connectionId\":\"ConnectionId\"}");

            var message =
                "{\"op\":\"status\"," +
                "\"statusCode\":\"FAILURE\"," +
                "\"errorCode\":\"TIMEOUT\"," +
                "\"errorMessage\":\"Timed out trying to read message make sure to add \\\\r\\\\n\\nRead data : ﻿\"," +
                "\"connectionClosed\":true," +
                "\"connectionId\":\"ConnectionId\"}";

            await this.SendLineAsync(message);
            Assert.False(this.Subscription.Connected);
        }

        [Fact]
        public async Task OnReadStatusUpdateConnectionStatusIsUpdated()
        {
            await this.SendLineAsync("{\"op\":\"connection\",\"connectionId\":\"ConnectionId\"}");
            await this.SendLineAsync("{\"op\":\"status\",\"connectionClosed\":false}");
            Assert.True(this.Subscription.Connected);

            await this.SendLineAsync("{\"op\":\"status\",\"connectionClosed\":true}");
            Assert.False(this.Subscription.Connected);
        }

        [Fact]
        public async Task OnReadHandleUnknownOperationWithoutThrowing()
        {
            await this.SendLineAsync("{\"op\":\"unknown\"}");
        }

        [Fact]
        public async Task OnAuthenticateGetSessionTokenIsCalled()
        {
            await this.Subscription.Authenticate();
            Assert.Equal(1, this.Session.TimesGetSessionTokenAsyncCalled);
        }

        [Fact]
        public async Task OnAuthenticate()
        {
            var authMessage = JsonConvert.SerializeObject(
                new AuthenticationMessageStub(
                    this.Session.AppKey,
                    await this.Session.GetTokenAsync()));

            await this.Subscription.Authenticate();
            Assert.Equal(authMessage, this.Writer.LastLineWritten);
        }
    }
}
