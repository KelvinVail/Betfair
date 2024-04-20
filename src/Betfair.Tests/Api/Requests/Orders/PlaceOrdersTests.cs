using Betfair.Api.Requests.Orders;

namespace Betfair.Tests.Api.Requests.Orders;

public class PlaceOrdersTests
{
    [Fact]
    public void MarketIdShouldNotBeNull()
    {
        Action act = () => new PlaceOrders(null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("marketId");
    }

    [Fact]
    public void InstructionsShouldNotBeNull()
    {
        var placeOrders = new PlaceOrders("1.23456789");

        placeOrders.Instructions.Should().NotBeNull();
    }

    [Fact]
    public void InstructionsCanBeAdded()
    {
        var placeOrders = new PlaceOrders("1.23456789");
        var instruction = new PlaceInstruction();

        placeOrders.Instructions.Add(instruction);

        placeOrders.Instructions.Should().Contain(instruction);
    }

    [Theory]
    [InlineData("customerRef123")]
    [InlineData(null)]
    public void CustomerRefCanBeSet(string? customerRef)
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
    public void MarketVersionCanBeSet(long? marketVersion)
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
    public void CustomerStrategyRefCanBeSet(string? strategyRef)
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
    public void AsyncCanBeSet(bool? value)
    {
        var placeOrders = new PlaceOrders("1.23456789")
        {
            Async = value
        };

        placeOrders.Async.Should().Be(value);
    }
}
