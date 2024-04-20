using Betfair.Core;
using Betfair.Extensions.Orders;

namespace Betfair.Extensions.Tests.Orders;

public class LimitOrderTests
{
    [Theory]
    [InlineData(1234567890L)]
    [InlineData(9999L)]
    public void CanBeCreatedWithSpecifiedSelectionId(long selectionId)
    {
        var order = new LimitOrder(selectionId, Side.Back, 1.01, 9.99);

        order.SelectionId.Should().Be(selectionId);
        order.ToInstruction().SelectionId.Should().Be(selectionId);
    }

    [Fact]
    public void CanBeCreatedAsABackOrder()
    {
        var order = new LimitOrder(1, Side.Back, 1.01, 9.99);

        order.Side.Should().Be(Side.Back.Value);
        order.ToInstruction().Side.Should().Be(Side.Back.Value);
    }

    [Fact]
    public void CanBeCreatedAsALayOrder()
    {
        var order = new LimitOrder(1, Side.Lay, 1.01, 9.99);

        order.Side.Should().Be(Side.Lay.Value);
        order.ToInstruction().Side.Should().Be(Side.Lay.Value);
    }

    [Theory]
    [InlineData(1.01)]
    [InlineData(3)]
    [InlineData(150)]
    public void PriceCanBeSet(double price)
    {
        var order = new LimitOrder(1, Side.Back, price, 9.99);

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
        var order = new LimitOrder(1, Side.Back, 1.01, size);

        order.Size.Should().Be(size);
        order.ToInstruction().LimitOrder?.Size.Should().Be(size);
    }

    [Fact]
    public void DefaultPersistenceTypeIsLapse()
    {
        var order = new LimitOrder(1, Side.Back, 1.01, 9.99);

        order.PersistenceType.Should().Be(PersistenceType.Lapse.Value);
        order.ToInstruction().LimitOrder?.PersistenceType.Should().Be(PersistenceType.Lapse.Value);
    }

    [Fact]
    public void PersistenceTypeCanBeSetToPersist()
    {
        var order = new LimitOrder(1, Side.Back, 1.01, 9.99, PersistenceType.Persist);

        order.PersistenceType.Should().Be(PersistenceType.Persist.Value);
        order.ToInstruction().LimitOrder?.PersistenceType.Should().Be(PersistenceType.Persist.Value);
    }

    [Fact]
    public void PersistenceTypeCanBeSetToMarketOnClose()
    {
        var order = new LimitOrder(1, Side.Back, 1.01, 9.99, PersistenceType.MarketOnClose);

        order.PersistenceType.Should().Be(PersistenceType.MarketOnClose.Value);
        order.ToInstruction().LimitOrder?.PersistenceType.Should().Be(PersistenceType.MarketOnClose.Value);
    }

    [Fact]
    public void TimeInForceIsNull()
    {
        var order = new LimitOrder(1, Side.Back, 1.01, 9.99);

        order.ToInstruction().LimitOrder?.TimeInForce.Should().BeNull();
    }

    [Fact]
    public void MinFillSizeIsNull()
    {
        var order = new LimitOrder(1, Side.Back, 1.01, 9.99);

        order.ToInstruction().LimitOrder?.MinFillSize.Should().BeNull();
    }

    [Fact]
    public void BetTargetSizeIsNull()
    {
        var order = new LimitOrder(1, Side.Back, 1.01, 9.99);

        order.ToInstruction().LimitOrder?.BetTargetSize.Should().BeNull();
    }

    [Fact]
    public void BetTargetTypeIsNull()
    {
        var order = new LimitOrder(1, Side.Back, 1.01, 9.99);

        order.ToInstruction().LimitOrder?.BetTargetType.Should().BeNull();
    }
}
