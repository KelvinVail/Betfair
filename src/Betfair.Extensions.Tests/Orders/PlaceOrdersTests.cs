using Betfair.Api.Requests.OrderDtos;

namespace Betfair.Extensions.Tests.Orders;

public class PlaceOrdersTests
{
    [Theory]
    [InlineData("1.23456789")]
    [InlineData("1.9876")]
    public void MarketIdShouldBeSet(string marketId)
    {
        var placeOrders = new PlaceOrders(marketId);

        placeOrders.MarketId.Should().Be(marketId);
    }

    [Fact]
    public void InstructionsShouldBeEmpty()
    {
        var placeOrders = new PlaceOrders("1.23456789");

        placeOrders.Instructions.Should().BeEmpty();
    }

    [Theory]
    [InlineData("customerRef123")]
    [InlineData(null)]
    public void CustomerRefShouldBeSet(string? customerRef)
    {
        var placeOrders = new PlaceOrders("1.23456789")
        {
            CustomerRef = customerRef,
        };

        placeOrders.CustomerRef.Should().Be(customerRef);
    }

    [Theory]
    [InlineData(1234567890L)]
    [InlineData(null)]
    public void MarketVersionShouldBeSet(long? marketVersion)
    {
        var placeOrders = new PlaceOrders("1.23456789")
        {
            MarketVersion = marketVersion,
        };

        placeOrders.MarketVersion.Should().Be(marketVersion);
    }

    [Theory]
    [InlineData("strategyRef123")]
    [InlineData(null)]
    public void CustomerStrategyRefShouldBeSet(string? strategyRef)
    {
        var placeOrders = new PlaceOrders("1.23456789")
        {
            CustomerStrategyRef = strategyRef,
        };

        placeOrders.CustomerStrategyRef.Should().Be(strategyRef);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public void AsyncShouldBeSet(bool? value)
    {
        var placeOrders = new PlaceOrders("1.23456789")
        {
            Async = value,
        };

        placeOrders.Async.Should().Be(value);
    }
}
