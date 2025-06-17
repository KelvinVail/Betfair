using Betfair.Api.Betting.Endpoints.PlaceOrders;

namespace Betfair.Tests.Api.Requests.Orders;

public class MarketOnCloseOrderTests
{
    [Fact]
    public void LiabilityPropertyShouldBeSettable()
    {
        var order = new MarketOnCloseOrder { Liability = 1000.0 };
        order.Liability.Should().Be(1000.0);
    }
}
