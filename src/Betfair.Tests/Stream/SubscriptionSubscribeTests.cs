//using Betfair.Stream;
//using Betfair.Tests.Stream.TestDoubles;

//namespace Betfair.Tests.Stream;

//public sealed class SubscriptionSubscribeTests : SubscriptionTests
//{
//    [Theory]
//    [InlineData("1.120684740")]
//    [InlineData("1.234567890")]
//    [InlineData("1.098765432")]
//    public async Task OnSubscribeSubscriptionMessageIsSent(string marketId)
//    {
//        var marketFilter = new MarketFilter().WithMarketId(marketId);
//        var subscriptionMessage = new SubscriptionMessageStub("marketSubscription", 1)
//            .WithMarketFilter(marketFilter)
//            .ToJson();
//        await Subscription.Subscribe(marketFilter, null);
//        Assert.Equal(subscriptionMessage, Writer.LastLineWritten);
//    }

//    [Fact]
//    public async Task OnSubscribeMarketDataFilterIsSet()
//    {
//        var dataFilter = new MarketDataFilter().WithBestPrices();
//        var subscriptionMessage = new SubscriptionMessageStub("marketSubscription", 1)
//            .WithMarketDateFilter(dataFilter)
//            .ToJson();
//        await Subscription.Subscribe(null, dataFilter);
//        Assert.Equal(subscriptionMessage, Writer.LastLineWritten);
//    }

//    [Fact]
//    public async Task OnMarketSubscriptionIdIncrements()
//    {
//        var subscriptionMessage = new SubscriptionMessageStub("marketSubscription", 1).ToJson();
//        await Subscription.Subscribe(null, null);
//        Assert.Equal(subscriptionMessage, Writer.LastLineWritten);

//        var subscriptionMessage2 = new SubscriptionMessageStub("marketSubscription", 2).ToJson();
//        await Subscription.Subscribe(null, null);
//        Assert.Equal(subscriptionMessage2, Writer.LastLineWritten);
//    }

//    [Fact]
//    public async Task OnSubscribeToOrdersOrderSubscriptionIsSent()
//    {
//        var subscriptionMessage = new SubscriptionMessageStub("orderSubscription", 1).ToJson();
//        await Subscription.SubscribeToOrders();
//        Assert.Equal(subscriptionMessage, Writer.LastLineWritten);
//    }

//    [Fact]
//    public async Task OnSubscribeToOrdersIdIncrements()
//    {
//        var subscriptionMessage = new SubscriptionMessageStub("orderSubscription", 1).ToJson();
//        await Subscription.SubscribeToOrders();
//        Assert.Equal(subscriptionMessage, Writer.LastLineWritten);

//        var subscriptionMessage2 = new SubscriptionMessageStub("orderSubscription", 2).ToJson();
//        await Subscription.SubscribeToOrders();
//        Assert.Equal(subscriptionMessage2, Writer.LastLineWritten);
//    }

//    [Fact]
//    public async Task OnAnySubscriptionIdIncrements()
//    {
//        var subscriptionMessage = new SubscriptionMessageStub("marketSubscription", 1).ToJson();
//        await Subscription.Subscribe(null, null);
//        Assert.Equal(subscriptionMessage, Writer.LastLineWritten);

//        var subscriptionMessage2 = new SubscriptionMessageStub("orderSubscription", 2).ToJson();
//        await Subscription.SubscribeToOrders();
//        Assert.Equal(subscriptionMessage2, Writer.LastLineWritten);
//    }
//}