using Betfair.Api.Orders;
using Betfair.Core;

namespace Betfair.Tests.Api.Requests.Orders;

public class FillOrKillOrderTests
{
    [Theory]
    [InlineData(1234567890L)]
    [InlineData(9999L)]
    public void CanBeCreatedWithSpecifiedSelectionId(long selectionId)
    {
        var order = new FillOrKillOrder(selectionId, Side.Back, 1.01, 9.99);

        order.SelectionId.Should().Be(selectionId);
        order.ToInstruction().SelectionId.Should().Be(selectionId);
    }

    [Fact]
    public void CanBeCreatedAsABackOrder()
    {
        var order = new FillOrKillOrder(1, Side.Back, 1.01, 9.99);

        order.Side.Should().Be(Side.Back.Value);
        order.ToInstruction().Side.Should().Be(Side.Back.Value);
    }

    [Fact]
    public void CanBeCreatedAsALayOrder()
    {
        var order = new FillOrKillOrder(1, Side.Lay, 1.01, 9.99);

        order.Side.Should().Be(Side.Lay.Value);
        order.ToInstruction().Side.Should().Be(Side.Lay.Value);
    }

    [Theory]
    [InlineData(1.01)]
    [InlineData(3)]
    [InlineData(150)]
    public void PriceCanBeSet(double price)
    {
        var order = new FillOrKillOrder(1, Side.Back, price, 9.99);

        order.Price.Should().Be(price);
        order.ToInstruction().LimitOrder.Should().NotBeNull();
        order.ToInstruction().LimitOrder?.Price.Should().Be(price);
    }

    [Theory]
    [InlineData(9.99)]
    [InlineData(1234.56)]
    [InlineData(0.01)]
    public void SizeCanBeSet(double size)
    {
        var order = new FillOrKillOrder(1, Side.Back, 1.01, size);

        order.Size.Should().Be(size);
        order.ToInstruction().LimitOrder?.Size.Should().Be(size);
    }

    [Fact]
    public void TimeInForceIsFillOrKill()
    {
        var order = new FillOrKillOrder(1, Side.Back, 1.01, 9.99);

        order.ToInstruction().LimitOrder?.TimeInForce.Should().Be("FILL_OR_KILL");
    }

    [Theory]
    [InlineData(9.98)]
    [InlineData(5.00)]
    [InlineData(0.10)]
    public void MinimumFillSizeCanBeSet(double minFillSize)
    {
        var order = new FillOrKillOrder(1, Side.Back, 1.01, 9.99, minFillSize);

        order.MinFillSize.Should().Be(minFillSize);
        order.ToInstruction().LimitOrder?.MinFillSize.Should().Be(minFillSize);
    }

    [Fact]
    public void MinimumFillSizeShouldBeNullAsDefault()
    {
        var order = new FillOrKillOrder(1, Side.Back, 1.01, 9.99);

        order.MinFillSize.Should().BeNull();
        order.ToInstruction().LimitOrder?.MinFillSize.Should().BeNull();
    }
}
