using Betfair.Core;

namespace Betfair.Tests.Core;

public class OrderTypeTests
{
    [Fact]
    public void EnumOrderTypeHasCorrectValues()
    {
        ((int)OrderType.Limit).Should().Be(0);
        ((int)OrderType.Limit_On_Close).Should().Be(1);
        ((int)OrderType.Market_On_Close).Should().Be(2);
    }

    [Fact]
    public void EnumOrderTypeHasCorrectStringValues()
    {
        OrderType.Limit.ToString().Should().Be("Limit");
        OrderType.Limit_On_Close.ToString().Should().Be("Limit_On_Close");
        OrderType.Market_On_Close.ToString().Should().Be("Market_On_Close");
    }

    [Fact]
    public void CanParseEnumOrderTypeFromString()
    {
        OrderType.Limit.Should().Be(Enum.Parse<OrderType>("Limit"));
        OrderType.Limit_On_Close.Should().Be(Enum.Parse<OrderType>("Limit_On_Close"));
        OrderType.Market_On_Close.Should().Be(Enum.Parse<OrderType>("Market_On_Close"));
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
        OrderType.Limit_On_Close.Should().Be(orderTypeLimitOnClose);
        OrderType.Market_On_Close.Should().Be(orderTypeMarketOnClose);
    }

    [Fact]
    public void CanSerializeEnumOrderTypeToJsonString()
    {
        const string jsonStringLimit = "\"LIMIT\"";
        const string jsonStringLimitOnClose = "\"LIMIT_ON_CLOSE\"";
        const string jsonStringMarketOnClose = "\"MARKET_ON_CLOSE\"";

        var orderTypeLimit = JsonSerializer.Serialize(OrderType.Limit);
        var orderTypeLimitOnClose = JsonSerializer.Serialize(OrderType.Limit_On_Close);
        var orderTypeMarketOnClose = JsonSerializer.Serialize(OrderType.Market_On_Close);

        orderTypeLimit.Should().Be(jsonStringLimit);
        orderTypeLimitOnClose.Should().Be(jsonStringLimitOnClose);
        orderTypeMarketOnClose.Should().Be(jsonStringMarketOnClose);
    }
}