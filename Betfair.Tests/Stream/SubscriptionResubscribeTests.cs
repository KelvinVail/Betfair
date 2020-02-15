namespace Betfair.Tests.Stream
{
    using System.Threading.Tasks;
    using Betfair.Tests.Stream.TestDoubles;
    using Xunit;

    public sealed class SubscriptionResubscribeTests : SubscriptionTests
    {
        [Fact]
        public async Task OnResubscribeOriginalMarketSubscriptionMessageIsSent()
        {
            var subscriptionMessage = new SubscriptionMessageStub("marketSubscription", 1).ToJson();
            await this.Subscription.Subscribe(null, null);
            Assert.Equal(subscriptionMessage, this.Writer.LastLineWritten);

            var reSubscriptionMessage = new SubscriptionMessageStub("marketSubscription", 1).ToJson();
            await this.TriggerResubscribe();
            Assert.Equal(reSubscriptionMessage, this.Writer.LastLineWritten);
        }

        [Fact]
        public async Task OnResubscribeOriginalOrderSubscriptionMessageIsSent()
        {
            var subscriptionMessage = new SubscriptionMessageStub("orderSubscription", 1).ToJson();
            await this.Subscription.SubscribeToOrders();
            Assert.Equal(subscriptionMessage, this.Writer.LastLineWritten);

            var reSubscriptionMessage = new SubscriptionMessageStub("orderSubscription", 1).ToJson();
            await this.TriggerResubscribe();
            Assert.Equal(reSubscriptionMessage, this.Writer.LastLineWritten);
        }

        [Fact]
        public async Task OnResubscribeAllSubscriptionMessagesAreSent()
        {
            await this.Subscription.Subscribe(null, null);
            await this.Subscription.Subscribe(null, null);
            await this.Subscription.SubscribeToOrders();
            var subscriptionMessage = new SubscriptionMessageStub("marketSubscription", 1).ToJson();
            var subscriptionMessage2 = new SubscriptionMessageStub("marketSubscription", 2).ToJson();
            var subscriptionMessage3 = new SubscriptionMessageStub("orderSubscription", 3).ToJson();

            await this.TriggerResubscribe();
            Assert.Contains(subscriptionMessage, this.Writer.AllLinesWritten);
            Assert.Contains(subscriptionMessage2, this.Writer.AllLinesWritten);
            Assert.Contains(subscriptionMessage3, this.Writer.AllLinesWritten);
        }

        [Theory]
        [InlineData("InitialClock")]
        [InlineData("NewClock")]
        [InlineData("RefreshedClock")]
        public async Task OnResubscribeInitialClockIsInMessage(string initialClock)
        {
            await this.Subscription.Subscribe(null, null);
            await this.SendLineAsync($"{{\"op\":\"mcm\",\"id\":1,\"initialClk\":\"{initialClock}\",\"clk\":\"Clock\"}}");

            var subscriptionMessage = new SubscriptionMessageStub("marketSubscription", 1)
                .WithInitialClock(initialClock)
                .WithClock("Clock")
                .ToJson();

            await this.TriggerResubscribe();
            Assert.Equal(subscriptionMessage, this.Writer.LastLineWritten);
        }

        [Fact]
        public async Task OnResubscribeAllSubscriptionMessagesSentContainInitialClock()
        {
            await this.Subscription.Subscribe(null, null);
            await this.Subscription.Subscribe(null, null);
            await this.Subscription.SubscribeToOrders();

            await this.SendLineAsync("{\"op\":\"mcm\",\"id\":1,\"initialClk\":\"One\",\"clk\":\"Clock\"}");
            await this.SendLineAsync("{\"op\":\"mcm\",\"id\":2,\"initialClk\":\"Two\",\"clk\":\"Clock\"}");
            await this.SendLineAsync("{\"op\":\"ocm\",\"id\":3,\"initialClk\":\"Three\",\"clk\":\"Clock\"}");

            var subscriptionMessage = new SubscriptionMessageStub("marketSubscription", 1)
                .WithInitialClock("One").WithClock("Clock").ToJson();
            var subscriptionMessage2 = new SubscriptionMessageStub("marketSubscription", 2)
                .WithInitialClock("Two").WithClock("Clock").ToJson();
            var subscriptionMessage3 = new SubscriptionMessageStub("orderSubscription", 3)
                .WithInitialClock("Three").WithClock("Clock").ToJson();

            await this.TriggerResubscribe();
            Assert.Contains(subscriptionMessage, this.Writer.AllLinesWritten);
            Assert.Contains(subscriptionMessage2, this.Writer.AllLinesWritten);
            Assert.Contains(subscriptionMessage3, this.Writer.AllLinesWritten);
        }

        [Fact]
        public async Task OnReadDoNotSetInitialClockIfNoSubscriptionMessageExists()
        {
            await this.SendLineAsync("{\"op\":\"mcm\",\"id\":1,\"initialClk\":\"One\",\"clk\":\"Clock\"}");
        }

        [Theory]
        [InlineData("Clock")]
        [InlineData("NewClock")]
        [InlineData("RefreshedClock")]
        public async Task OnResubscribeClockIsInMessage(string clock)
        {
            await this.Subscription.Subscribe(null, null);
            await this.SendLineAsync($"{{\"op\":\"mcm\",\"id\":1,\"initialClk\":\"InitialClock\",\"clk\":\"{clock}\"}}");

            var subscriptionMessage = new SubscriptionMessageStub("marketSubscription", 1)
                .WithInitialClock("InitialClock")
                .WithClock(clock)
                .ToJson();

            await this.TriggerResubscribe();
            Assert.Equal(subscriptionMessage, this.Writer.LastLineWritten);
        }

        [Fact]
        public async Task OnResubscribeLatestClockIsInMessage()
        {
            await this.Subscription.Subscribe(null, null);
            await this.SendLineAsync($"{{\"op\":\"mcm\",\"id\":1,\"initialClk\":\"InitialClock\",\"clk\":\"Clock\"}}");
            await this.SendLineAsync($"{{\"op\":\"mcm\",\"id\":1,\"clk\":\"NewClock\"}}");

            var subscriptionMessage = new SubscriptionMessageStub("marketSubscription", 1)
                .WithInitialClock("InitialClock")
                .WithClock("NewClock")
                .ToJson();

            await this.TriggerResubscribe();
            Assert.Equal(subscriptionMessage, this.Writer.LastLineWritten);
        }

        private async Task TriggerResubscribe()
        {
            this.Writer.ClearPreviousResults();
            await this.Subscription.Resubscribe();
        }
    }
}
