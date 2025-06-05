using Betfair.Api.Requests.Orders;
using Betfair.Core;

namespace Betfair.Tests.Api.Requests.Orders;

public class PlaceInstructionTests
{
    [Fact]
    public void SelectionIdCanBeSet()
    {
        var instruction = new PlaceInstruction { SelectionId = 1234567890L };

        instruction.SelectionId.Should().Be(1234567890L);
    }

    [Theory]
    [InlineData(Side.Back)]
    [InlineData(Side.Lay)]
    public void SideCanBeSet(Side side)
    {
        var instruction = new PlaceInstruction { Side = side };

        instruction.Side.Should().Be(side);
    }

    [Theory]
    [InlineData(OrderType.Limit)]
    [InlineData(OrderType.LimitOnClose)]
    [InlineData(OrderType.MarketOnClose)]
    public void OrderTypeCanBeSet(OrderType orderType)
    {
        var instruction = new PlaceInstruction { OrderType = orderType };

        instruction.OrderType.Should().Be(orderType);
    }

    [Fact]
    public void LimitOrderCanBeSet()
    {
        var limitOrder = new LimitOrder();
        var instruction = new PlaceInstruction { LimitOrder = limitOrder };

        instruction.LimitOrder.Should().Be(limitOrder);
    }

    [Fact]
    public void LimitOnCloseCanBeSet()
    {
        var limitOnClose = new LimitOnCloseOrder();
        var instruction = new PlaceInstruction { LimitOnClose = limitOnClose };

        instruction.LimitOnClose.Should().Be(limitOnClose);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(1.5)]
    [InlineData(null)]
    public void HandicapCanBeSet(double? handicap)
    {
        var instruction = new PlaceInstruction { Handicap = handicap };

        instruction.Handicap.Should().Be(handicap);
    }

    [Theory]
    [InlineData("orderRef123")]
    [InlineData(null)]
    public void CustomerOrderRefCanBeSet(string? orderRef)
    {
        var instruction = new PlaceInstruction { CustomerOrderRef = orderRef };

        instruction.CustomerOrderRef.Should().Be(orderRef);
    }

    [Fact]
    public void MarketOnCloseCanBeSet()
    {
        var marketOnClose = new MarketOnCloseOrder();
        var instruction = new PlaceInstruction { MarketOnClose = marketOnClose };

        instruction.MarketOnClose.Should().Be(marketOnClose);
    }
}
