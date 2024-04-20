using Betfair.Core;
using Betfair.Extensions.Orders;

namespace Betfair.Extensions.Tests.Orders;

public class PayoutTargetOrderTests
{
    [Theory]
    [InlineData(1234567890L)]
    [InlineData(9999L)]
    public void CanBeCreatedWithSpecifiedSelectionId(long selectionId)
    {
        var order = new PayoutTargetOrder(selectionId, Side.Back, 1.01, 9.99);

        order.SelectionId.Should().Be(selectionId);
        order.ToInstruction().SelectionId.Should().Be(selectionId);
    }

    [Fact]
    public void CanBeCreatedAsABackOrder()
    {
        var order = new PayoutTargetOrder(1, Side.Back, 1.01, 9.99);

        order.Side.Should().Be(Side.Back);
        order.ToInstruction().Side.Should().Be(Side.Back);
    }

    [Fact]
    public void CanBeCreatedAsALayOrder()
    {
        var order = new PayoutTargetOrder(1, Side.Lay, 1.01, 9.99);

        order.Side.Should().Be(Side.Lay);
        order.ToInstruction().Side.Should().Be(Side.Lay);
    }

    [Theory]
    [InlineData(1.01)]
    [InlineData(3)]
    [InlineData(150)]
    public void PriceCanBeSet(double price)
    {
        var order = new PayoutTargetOrder(1, Side.Back, price, 9.99);

        order.Price.Should().Be(price);
        order.ToInstruction().LimitOrder.Should().NotBeNull();
        order.ToInstruction().LimitOrder?.Price.Should().Be(price);
    }

    [Fact]
    public void DefaultPersistenceTypeIsLapse()
    {
        var order = new PayoutTargetOrder(1, Side.Back, 1.01, 9.99);

        order.PersistenceType.Should().Be(PersistenceType.Lapse);
        order.ToInstruction().LimitOrder?.PersistenceType.Should().Be(PersistenceType.Lapse);
    }

    [Fact]
    public void PersistenceTypeCanBeSetToPersist()
    {
        var order = new PayoutTargetOrder(1, Side.Back, 1.01, 9.99, PersistenceType.Persist);

        order.PersistenceType.Should().Be(PersistenceType.Persist);
        order.ToInstruction().LimitOrder?.PersistenceType.Should().Be(PersistenceType.Persist);
    }

    [Fact]
    public void PersistenceTypeCanBeSetToMarketOnClose()
    {
        var order = new PayoutTargetOrder(1, Side.Back, 1.01, 9.99, PersistenceType.Market_On_Close);

        order.PersistenceType.Should().Be(PersistenceType.Market_On_Close);
        order.ToInstruction().LimitOrder?.PersistenceType.Should().Be(PersistenceType.Market_On_Close);
    }

    [Fact]
    public void TimeInForceIsNull()
    {
        var order = new PayoutTargetOrder(1, Side.Back, 1.01, 9.99);

        order.ToInstruction().LimitOrder?.TimeInForce.Should().BeNull();
    }

    [Fact]
    public void MinFillSizeIsNull()
    {
        var order = new PayoutTargetOrder(1, Side.Back, 1.01, 9.99);

        order.ToInstruction().LimitOrder?.MinFillSize.Should().BeNull();
    }

    [Fact]
    public void SizeShouldBeBull()
    {
        var order = new PayoutTargetOrder(1, Side.Back, 1.01, 9.99);

        order.ToInstruction().LimitOrder?.Size.Should().BeNull();
    }

    [Fact]
    public void BetTargetTypeShouldBePayout()
    {
        var order = new PayoutTargetOrder(1, Side.Back, 1.01, 9.99);

        order.ToInstruction().LimitOrder?.BetTargetType.Should().Be("PAYOUT");
    }

    [Theory]
    [InlineData(9.99)]
    [InlineData(1234.56)]
    [InlineData(0.01)]
    public void PayoutTargetCanBeSet(double size)
    {
        var order = new PayoutTargetOrder(1, Side.Back, 1.01, size);

        order.PayoutTarget.Should().Be(size);
        order.ToInstruction().LimitOrder?.BetTargetSize.Should().Be(size);
    }
}
