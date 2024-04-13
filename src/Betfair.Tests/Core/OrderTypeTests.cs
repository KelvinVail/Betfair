using Betfair.Core;

namespace Betfair.Tests.Core;

public class OrderTypeTests
{
    [Fact]
    public void LimitShouldReturnTheCorrectValue()
    {
        OrderType.Limit.Value.Should().Be("LIMIT");
        OrderType.Limit.ToString().Should().Be("LIMIT");
    }

    [Fact]
    public void LimitOnCloseShouldReturnTheCorrectValue()
    {
        OrderType.LimitOnClose.Value.Should().Be("LIMIT_ON_CLOSE");
        OrderType.LimitOnClose.ToString().Should().Be("LIMIT_ON_CLOSE");
    }

    [Fact]
    public void MarketOnCloseShouldReturnTheCorrectValue()
    {
        OrderType.MarketOnClose.Value.Should().Be("MARKET_ON_CLOSE");
        OrderType.MarketOnClose.ToString().Should().Be("MARKET_ON_CLOSE");
    }
}
