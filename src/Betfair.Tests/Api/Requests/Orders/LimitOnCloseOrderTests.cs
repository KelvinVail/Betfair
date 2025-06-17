using Betfair.Api.Betting.Endpoints.PlaceOrders.Responses;

namespace Betfair.Tests.Api.Requests.Orders;

public class LimitOnCloseOrderTests
{
    [Fact]
    public void LiabilityPropertyShouldBeSettable()
    {
        var order = new LimitOnCloseOrder { Liability = 1000.0 };
        order.Liability.Should().Be(1000.0);
    }

    [Fact]
    public void PricePropertyShouldBeSettable()
    {
        var order = new LimitOnCloseOrder { Price = 1.5 };
        order.Price.Should().Be(1.5);
    }
}
