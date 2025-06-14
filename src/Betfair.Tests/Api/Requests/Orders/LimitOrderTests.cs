using Betfair.Api.Requests.Orders;
using Betfair.Core.Enums;

namespace Betfair.Tests.Api.Requests.Orders;

public class LimitOrderTests
{
    [Fact]
    public void SizePropertyShouldBeSettable()
    {
        var order = new LimitOrder { Size = 10.0 };
        order.Size.Should().Be(10.0);
    }

    [Fact]
    public void PricePropertyShouldBeSettable()
    {
        var order = new LimitOrder { Price = 20.0 };
        order.Price.Should().Be(20.0);
    }

    [Fact]
    public void PersistenceTypePropertyShouldBeSettable()
    {
        var order = new LimitOrder { PersistenceType = PersistenceType.Persist };
        order.PersistenceType.Should().Be(PersistenceType.Persist);
    }

    [Fact]
    public void TimeInForcePropertyShouldBeSettable()
    {
        var order = new LimitOrder { TimeInForce = "FILL_OR_KILL" };
        order.TimeInForce.Should().Be("FILL_OR_KILL");
    }

    [Fact]
    public void MinFillSizePropertyShouldBeSettable()
    {
        var order = new LimitOrder { MinFillSize = 30.0 };
        order.MinFillSize.Should().Be(30.0);
    }

    [Fact]
    public void BetTargetSizePropertyShouldBeSettable()
    {
        var order = new LimitOrder { BetTargetSize = 40.0 };
        order.BetTargetSize.Should().Be(40.0);
    }

    [Fact]
    public void BetTargetTypePropertyShouldBeSettable()
    {
        var order = new LimitOrder { BetTargetType = "PAYOUT" };
        order.BetTargetType.Should().Be("PAYOUT");
    }
}
