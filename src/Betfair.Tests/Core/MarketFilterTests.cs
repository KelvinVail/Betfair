using Betfair.Core;

namespace Betfair.Tests.Core;

public class MarketFilterTests : MarketFilter<MarketFilterTests>
{
    [Fact]
    public void IsCreatedWithNullMarketTypes() =>
        MarketTypes.Should().BeNull();

    [Fact]
    public void MarketTypeIdsAreAddedToMarketTypes()
    {
        WithMarketType(MarketType.Win)
            .WithMarketType(MarketType.CorrectScore);

        MarketTypes.Should().Contain(MarketType.Win.Id);
        MarketTypes.Should().Contain(MarketType.CorrectScore.Id);
    }

    [Fact]
    public void TextMarketTypeIdsAreAddedToMarketTypes()
    {
        WithMarketType("WIN")
            .WithMarketType("CORRECT_SCORE");

        MarketTypes.Should().Contain(MarketType.Win.Id);
        MarketTypes.Should().Contain(MarketType.CorrectScore.Id);
    }

    [Fact]
    public void IsCreatedWithNullMarketTypeCodes() =>
        MarketTypeCodes.Should().BeNull();

    [Fact]
    public void MarketTypeIdsAreAddedToMarketTypeCodes()
    {
        WithMarketType(MarketType.Win)
            .WithMarketType(MarketType.CorrectScore);

        MarketTypeCodes.Should().Contain(MarketType.Win.Id);
        MarketTypeCodes.Should().Contain(MarketType.CorrectScore.Id);
    }

    [Fact]
    public void TextMarketTypeIdsAreAddedToMarketTypeCodes()
    {
        WithMarketType("WIN")
            .WithMarketType("CORRECT_SCORE");

        MarketTypeCodes.Should().Contain(MarketType.Win.Id);
        MarketTypeCodes.Should().Contain(MarketType.CorrectScore.Id);
    }

    [Fact]
    public void AddingNullMarketTypeHasNoEffect()
    {
        WithMarketType(MarketType.Win)
            .WithMarketType((MarketType)null!);

        MarketTypes.Should().Contain(MarketType.Win.Id);
        MarketTypes.Should().NotContainNulls();
        MarketTypeCodes.Should().Contain(MarketType.Win.Id);
        MarketTypeCodes.Should().NotContainNulls();
    }

    [Fact]
    public void AddingNullMarketTypeStringHasNoEffect()
    {
        WithMarketType("WIN")
            .WithMarketType((string)null!);

        MarketTypes.Should().Contain(MarketType.Win.Id);
        MarketTypes.Should().NotContainNulls();
        MarketTypeCodes.Should().Contain(MarketType.Win.Id);
        MarketTypeCodes.Should().NotContainNulls();
    }

    [Fact]
    public void IsCreatedWithNullEventTypeIds() =>
        EventTypeIds.Should().BeNull();

    [Fact]
    public void EventTypesAreAddedToEventTypesIds()
    {
        WithEventType(EventType.HorseRacing)
            .WithEventType(EventType.AmericanFootball);

        EventTypeIds.Should().Contain(EventType.HorseRacing.Id);
        EventTypeIds.Should().Contain(EventType.AmericanFootball.Id);
    }

    [Fact]
    public void TextEventTypeIdsAreAddedToEventTypes()
    {
        WithEventType(1)
            .WithEventType(7);

        EventTypeIds.Should().Contain(1);
        EventTypeIds.Should().Contain(7);
    }

    [Fact]
    public void AddingNullEventTypeHasNoEffect()
    {
        WithEventType(EventType.HorseRacing)
            .WithEventType(null!);

        EventTypeIds.Should().Contain(EventType.HorseRacing.Id);
        EventTypeIds.Should().NotContainNulls();
    }

    [Fact]
    public void IsCreatedWithNullMarketIds() =>
        MarketIds.Should().BeNull();

    [Theory]
    [InlineData("1.23456789")]
    public void MarketIdIsAddedToMarketIds(string marketId)
    {
        WithMarketId(marketId)
            .WithMarketId("1.999");

        MarketIds.Should().Contain(marketId);
        MarketIds.Should().Contain("1.999");
    }

    [Fact]
    public void AddingNullMarketIdHasNoEffect()
    {
        WithMarketId(null!)
            .WithMarketId("1.999");

        MarketIds.Should().Contain("1.999");
        MarketIds.Should().NotContainNulls();
    }
}
