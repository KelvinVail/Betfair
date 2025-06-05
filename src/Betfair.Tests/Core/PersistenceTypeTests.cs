using Betfair.Core;

namespace Betfair.Tests.Core;

public class PersistenceTypeTests
{
    [Fact]
    public void EnumOrderTypeHasCorrectValues()
    {
        ((int)PersistenceType.Lapse).Should().Be(0);
        ((int)PersistenceType.Persist).Should().Be(1);
        ((int)PersistenceType.MarketOnClose).Should().Be(2);
    }

    [Fact]
    public void EnumOrderTypeHasCorrectStringValues()
    {
        PersistenceType.Lapse.ToString().Should().Be("Lapse");
        PersistenceType.Persist.ToString().Should().Be("Persist");
        PersistenceType.MarketOnClose.ToString().Should().Be("MarketOnClose");
    }

    [Fact]
    public void CanParseEnumOrderTypeFromString()
    {
        Enum.Parse<PersistenceType>("Lapse").Should().Be(PersistenceType.Lapse);
        Enum.Parse<PersistenceType>("Persist").Should().Be(PersistenceType.Persist);
        Enum.Parse<PersistenceType>("MarketOnClose").Should().Be(PersistenceType.MarketOnClose);
    }

    [Fact]
    public void CanParseEnumOrderTypeFromJsonString()
    {
        JsonSerializer.Deserialize<PersistenceType>("\"LAPSE\"").Should().Be(PersistenceType.Lapse);
        JsonSerializer.Deserialize<PersistenceType>("\"PERSIST\"").Should().Be(PersistenceType.Persist);
        JsonSerializer.Deserialize<PersistenceType>("\"MARKET_ON_CLOSE\"").Should().Be(PersistenceType.MarketOnClose);
    }

    [Fact]
    public void CanSerializeEnumOrderTypeToJsonString()
    {
        JsonSerializer.Serialize(PersistenceType.Lapse).Should().Be("\"LAPSE\"");
        JsonSerializer.Serialize(PersistenceType.Persist).Should().Be("\"PERSIST\"");
        JsonSerializer.Serialize(PersistenceType.MarketOnClose).Should().Be("\"MARKET_ON_CLOSE\"");
    }
}
