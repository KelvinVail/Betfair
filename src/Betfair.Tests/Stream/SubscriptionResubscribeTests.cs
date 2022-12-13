//using Betfair.Tests.Stream.TestDoubles;

//namespace Betfair.Tests.Stream;

//public sealed class SubscriptionResubscribeTests : SubscriptionTests
//{
//    [Fact]
//    public async Task OnResubscribeOriginalMarketSubscriptionMessageIsSent()
//    {
//        var subscriptionMessage = new SubscriptionMessageStub("marketSubscription", 1).ToJson();
//        await Subscription.Subscribe(null, null);
//        Assert.Equal(subscriptionMessage, Writer.LastLineWritten);

//        var reSubscriptionMessage = new SubscriptionMessageStub("marketSubscription", 1).ToJson();
//        await TriggerResubscribe();
//        Assert.Equal(reSubscriptionMessage, Writer.LastLineWritten);
//    }

//    [Fact]
//    public async Task OnResubscribeOriginalOrderSubscriptionMessageIsSent()
//    {
//        var subscriptionMessage = new SubscriptionMessageStub("orderSubscription", 1).ToJson();
//        await Subscription.SubscribeToOrders();
//        Assert.Equal(subscriptionMessage, Writer.LastLineWritten);

//        var reSubscriptionMessage = new SubscriptionMessageStub("orderSubscription", 1).ToJson();
//        await TriggerResubscribe();
//        Assert.Equal(reSubscriptionMessage, Writer.LastLineWritten);
//    }

//    [Fact]
//    public async Task OnResubscribeAllSubscriptionMessagesAreSent()
//    {
//        await Subscription.Subscribe(null, null);
//        await Subscription.Subscribe(null, null);
//        await Subscription.SubscribeToOrders();
//        var subscriptionMessage = new SubscriptionMessageStub("marketSubscription", 1).ToJson();
//        var subscriptionMessage2 = new SubscriptionMessageStub("marketSubscription", 2).ToJson();
//        var subscriptionMessage3 = new SubscriptionMessageStub("orderSubscription", 3).ToJson();

//        await TriggerResubscribe();
//        Assert.Contains(subscriptionMessage, Writer.AllLinesWritten);
//        Assert.Contains(subscriptionMessage2, Writer.AllLinesWritten);
//        Assert.Contains(subscriptionMessage3, Writer.AllLinesWritten);
//    }

//    [Theory]
//    [InlineData("InitialClock")]
//    [InlineData("NewClock")]
//    [InlineData("RefreshedClock")]
//    public async Task OnResubscribeInitialClockIsInMessage(string initialClock)
//    {
//        await Subscription.Subscribe(null, null);
//        await SendLineAsync($"{{\"op\":\"mcm\",\"id\":1,\"initialClk\":\"{initialClock}\",\"clk\":\"Clock\"}}");

//        var subscriptionMessage = new SubscriptionMessageStub("marketSubscription", 1)
//            .WithInitialClock(initialClock)
//            .WithClock("Clock")
//            .ToJson();

//        await TriggerResubscribe();
//        Assert.Equal(subscriptionMessage, Writer.LastLineWritten);
//    }

//    [Fact]
//    public async Task OnResubscribeAllSubscriptionMessagesSentContainInitialClock()
//    {
//        await Subscription.Subscribe(null, null);
//        await Subscription.Subscribe(null, null);
//        await Subscription.SubscribeToOrders();

//        await SendLineAsync("{\"op\":\"mcm\",\"id\":1,\"initialClk\":\"One\",\"clk\":\"Clock\"}");
//        await SendLineAsync("{\"op\":\"mcm\",\"id\":2,\"initialClk\":\"Two\",\"clk\":\"Clock\"}");
//        await SendLineAsync("{\"op\":\"ocm\",\"id\":3,\"initialClk\":\"Three\",\"clk\":\"Clock\"}");

//        var subscriptionMessage = new SubscriptionMessageStub("marketSubscription", 1)
//            .WithInitialClock("One").WithClock("Clock").ToJson();
//        var subscriptionMessage2 = new SubscriptionMessageStub("marketSubscription", 2)
//            .WithInitialClock("Two").WithClock("Clock").ToJson();
//        var subscriptionMessage3 = new SubscriptionMessageStub("orderSubscription", 3)
//            .WithInitialClock("Three").WithClock("Clock").ToJson();

//        await TriggerResubscribe();
//        Assert.Contains(subscriptionMessage, Writer.AllLinesWritten);
//        Assert.Contains(subscriptionMessage2, Writer.AllLinesWritten);
//        Assert.Contains(subscriptionMessage3, Writer.AllLinesWritten);
//    }

//    [Fact]
//    public async Task OnReadDoNotSetInitialClockIfNoSubscriptionMessageExists()
//    {
//        await SendLineAsync("{\"op\":\"mcm\",\"id\":1,\"initialClk\":\"One\",\"clk\":\"Clock\"}");
//    }

//    [Theory]
//    [InlineData("Clock")]
//    [InlineData("NewClock")]
//    [InlineData("RefreshedClock")]
//    public async Task OnResubscribeClockIsInMessage(string clock)
//    {
//        await Subscription.Subscribe(null, null);
//        await SendLineAsync($"{{\"op\":\"mcm\",\"id\":1,\"initialClk\":\"InitialClock\",\"clk\":\"{clock}\"}}");

//        var subscriptionMessage = new SubscriptionMessageStub("marketSubscription", 1)
//            .WithInitialClock("InitialClock")
//            .WithClock(clock)
//            .ToJson();

//        await TriggerResubscribe();
//        Assert.Equal(subscriptionMessage, Writer.LastLineWritten);
//    }

//    [Fact]
//    public async Task OnResubscribeLatestClockIsInMessage()
//    {
//        await Subscription.Subscribe(null, null);
//        await SendLineAsync($"{{\"op\":\"mcm\",\"id\":1,\"initialClk\":\"InitialClock\",\"clk\":\"Clock\"}}");
//        await SendLineAsync($"{{\"op\":\"mcm\",\"id\":1,\"clk\":\"NewClock\"}}");

//        var subscriptionMessage = new SubscriptionMessageStub("marketSubscription", 1)
//            .WithInitialClock("InitialClock")
//            .WithClock("NewClock")
//            .ToJson();

//        await TriggerResubscribe();
//        Assert.Equal(subscriptionMessage, Writer.LastLineWritten);
//    }

//    private async Task TriggerResubscribe()
//    {
//        Writer.ClearPreviousResults();
//        await Subscription.Resubscribe();
//    }
//}