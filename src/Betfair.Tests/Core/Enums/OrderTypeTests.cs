using Betfair.Core.Enums;

namespace Betfair.Tests.Core.Enums;

public class OrderTypeTests
{
    [Fact]
    public void EnumOrderTypeHasCorrectValues()
    {
        ((int)OrderType.Limit).Should().Be(0);
        ((int)OrderType.LimitOnClose).Should().Be(1);
        ((int)OrderType.MarketOnClose).Should().Be(2);
    }

    [Fact]
    public void EnumOrderTypeHasCorrectStringValues()
    {
        OrderType.Limit.ToString().Should().Be("Limit");
        OrderType.LimitOnClose.ToString().Should().Be("LimitOnClose");
        OrderType.MarketOnClose.ToString().Should().Be("MarketOnClose");
    }

    [Fact]
    public void CanParseEnumOrderTypeFromString()
    {
        OrderType.Limit.Should().Be(Enum.Parse<OrderType>("Limit"));
        OrderType.LimitOnClose.Should().Be(Enum.Parse<OrderType>("LimitOnClose"));
        OrderType.MarketOnClose.Should().Be(Enum.Parse<OrderType>("MarketOnClose"));
    }

    [Fact]
    public void CanParseEnumOrderTypeFromJsonString()
    {
        const string jsonStringLimit = "\"LIMIT\"";
        const string jsonStringLimitOnClose = "\"LIMIT_ON_CLOSE\"";
        const string jsonStringMarketOnClose = "\"MARKET_ON_CLOSE\"";

        var orderTypeLimit = JsonSerializer.Deserialize<OrderType>(jsonStringLimit);
        var orderTypeLimitOnClose = JsonSerializer.Deserialize<OrderType>(jsonStringLimitOnClose);
        var orderTypeMarketOnClose = JsonSerializer.Deserialize<OrderType>(jsonStringMarketOnClose);

        OrderType.Limit.Should().Be(orderTypeLimit);
        OrderType.LimitOnClose.Should().Be(orderTypeLimitOnClose);
        OrderType.MarketOnClose.Should().Be(orderTypeMarketOnClose);
    }

    [Fact]
    public void CanSerializeEnumOrderTypeToJsonString()
    {
        const string jsonStringLimit = "\"LIMIT\"";
        const string jsonStringLimitOnClose = "\"LIMIT_ON_CLOSE\"";
        const string jsonStringMarketOnClose = "\"MARKET_ON_CLOSE\"";

        var orderTypeLimit = JsonSerializer.Serialize(OrderType.Limit);
        var orderTypeLimitOnClose = JsonSerializer.Serialize(OrderType.LimitOnClose);
        var orderTypeMarketOnClose = JsonSerializer.Serialize(OrderType.MarketOnClose);

        orderTypeLimit.Should().Be(jsonStringLimit);
        orderTypeLimitOnClose.Should().Be(jsonStringLimitOnClose);
        orderTypeMarketOnClose.Should().Be(jsonStringMarketOnClose);
    }
}