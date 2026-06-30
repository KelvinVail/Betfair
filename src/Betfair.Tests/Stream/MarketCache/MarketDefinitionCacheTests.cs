using System.Text;
using System.Text.Json;
using Betfair.Stream.MarketCache;

namespace Betfair.Tests.Stream.MarketCache;

public class MarketDefinitionCacheTests
{
    private readonly MarketDefinitionCache _cache = new ();

    [Fact]
    public void AllPropertiesDefaultToNull()
    {
        _cache.Status.Should().BeNull();
        _cache.InPlay.Should().BeNull();
        _cache.Venue.Should().BeNull();
        _cache.BettingType.Should().BeNull();
        _cache.MarketType.Should().BeNull();
        _cache.CountryCode.Should().BeNull();
        _cache.EventId.Should().BeNull();
        _cache.EventTypeId.Should().BeNull();
        _cache.Timezone.Should().BeNull();
        _cache.BspMarket.Should().BeNull();
        _cache.NumberOfWinners.Should().BeNull();
        _cache.BetDelay.Should().BeNull();
        _cache.NumberOfActiveRunners.Should().BeNull();
        _cache.Version.Should().BeNull();
        _cache.EachWayDivisor.Should().BeNull();
        _cache.MarketBaseRate.Should().BeNull();
    }

    [Fact]
    public void ReadFromParsesStatusField()
    {
        ReadDefinition("""{"status":"OPEN"}""");

        _cache.Status.Should().Be("OPEN");
    }

    [Fact]
    public void ReadFromParsesInPlayField()
    {
        ReadDefinition("""{"inPlay":true}""");

        _cache.InPlay.Should().BeTrue();
    }

    [Fact]
    public void ReadFromParsesVenueField()
    {
        ReadDefinition("""{"venue":"Lingfield"}""");

        _cache.Venue.Should().Be("Lingfield");
    }

    [Fact]
    public void ReadFromParsesBettingTypeField()
    {
        ReadDefinition("""{"bettingType":"ODDS"}""");

        _cache.BettingType.Should().Be("ODDS");
    }

    [Fact]
    public void ReadFromParsesMarketTypeField()
    {
        ReadDefinition("""{"marketType":"WIN"}""");

        _cache.MarketType.Should().Be("WIN");
    }

    [Fact]
    public void ReadFromParsesCountryCodeField()
    {
        ReadDefinition("""{"countryCode":"GB"}""");

        _cache.CountryCode.Should().Be("GB");
    }

    [Fact]
    public void ReadFromParsesEventIdField()
    {
        ReadDefinition("""{"eventId":"34173357"}""");

        _cache.EventId.Should().Be("34173357");
    }

    [Fact]
    public void ReadFromParsesEventTypeIdField()
    {
        ReadDefinition("""{"eventTypeId":"7"}""");

        _cache.EventTypeId.Should().Be("7");
    }

    [Fact]
    public void ReadFromParsesBetDelayField()
    {
        ReadDefinition("""{"betDelay":5}""");

        _cache.BetDelay.Should().Be(5);
    }

    [Fact]
    public void ReadFromParsesNumberOfActiveRunnersField()
    {
        ReadDefinition("""{"numberOfActiveRunners":12}""");

        _cache.NumberOfActiveRunners.Should().Be(12);
    }

    [Fact]
    public void ReadFromParsesNumberOfWinnersField()
    {
        ReadDefinition("""{"numberOfWinners":3}""");

        _cache.NumberOfWinners.Should().Be(3);
    }

    [Fact]
    public void ReadFromParsesVersionField()
    {
        ReadDefinition("""{"version":6537458050}""");

        _cache.Version.Should().Be(6537458050);
    }

    [Fact]
    public void ReadFromParsesMarketBaseRateField()
    {
        ReadDefinition("""{"marketBaseRate":5.0}""");

        _cache.MarketBaseRate.Should().Be(5.0);
    }

    [Fact]
    public void ReadFromParsesEachWayDivisorField()
    {
        ReadDefinition("""{"eachWayDivisor":4.0}""");

        _cache.EachWayDivisor.Should().Be(4.0);
    }

    [Fact]
    public void ReadFromParsesBooleanFlags()
    {
        ReadDefinition("""{"bspMarket":true,"turnInPlayEnabled":true,"persistenceEnabled":true,"crossMatching":false,"runnersVoidable":false,"discountAllowed":true,"complete":true,"bspReconciled":false}""");

        _cache.BspMarket.Should().BeTrue();
        _cache.TurnInPlayEnabled.Should().BeTrue();
        _cache.PersistenceEnabled.Should().BeTrue();
        _cache.CrossMatching.Should().BeFalse();
        _cache.RunnersVoidable.Should().BeFalse();
        _cache.DiscountAllowed.Should().BeTrue();
        _cache.Complete.Should().BeTrue();
        _cache.BspReconciled.Should().BeFalse();
    }

    [Fact]
    public void ReadFromParsesRunnerDefinitions()
    {
        ReadDefinition("""{"runners":[{"id":270592,"status":"ACTIVE","sortPriority":1,"adjustmentFactor":20.833},{"id":56297029,"status":"ACTIVE","sortPriority":2,"adjustmentFactor":18.719}]}""");

        _cache.Runners.Should().HaveCount(2);
        _cache.Runners[0].SelectionId.Should().Be(270592);
        _cache.Runners[0].Status.Should().Be("ACTIVE");
        _cache.Runners[0].SortPriority.Should().Be(1);
        _cache.Runners[0].AdjustmentFactor.Should().Be(20.833);
        _cache.Runners[1].SelectionId.Should().Be(56297029);
    }

    [Fact]
    public void ReadFromParsesRunnerHandicap()
    {
        ReadDefinition("""{"runners":[{"id":12345,"hc":1.5}]}""");

        _cache.Runners.Should().HaveCount(1);
        _cache.Runners[0].SelectionId.Should().Be(12345);
        _cache.Runners[0].Handicap.Should().Be(1.5);
    }

    [Fact]
    public void ReadFromParsesRunnerBspLiability()
    {
        ReadDefinition("""{"runners":[{"id":12345,"bsp":3.75}]}""");

        _cache.Runners.Should().HaveCount(1);
        _cache.Runners[0].SelectionId.Should().Be(12345);
        _cache.Runners[0].BspLiability.Should().Be(3.75);
    }

    [Fact]
    public void ReadFromParsesRunnerWithAllFields()
    {
        ReadDefinition("""{"runners":[{"id":99999,"status":"REMOVED","sortPriority":5,"adjustmentFactor":10.0,"hc":-0.25,"bsp":2.5}]}""");

        var runner = _cache.Runners[0];
        runner.SelectionId.Should().Be(99999);
        runner.Status.Should().Be("REMOVED");
        runner.SortPriority.Should().Be(5);
        runner.AdjustmentFactor.Should().Be(10.0);
        runner.Handicap.Should().Be(-0.25);
        runner.BspLiability.Should().Be(2.5);
    }

    [Fact]
    public void ReadFromSkipsUnknownRunnerFields()
    {
        ReadDefinition("""{"runners":[{"id":100,"unknownField":"value","status":"ACTIVE","nestedUnknown":{"x":1}}]}""");

        _cache.Runners.Should().HaveCount(1);
        _cache.Runners[0].SelectionId.Should().Be(100);
        _cache.Runners[0].Status.Should().Be("ACTIVE");
    }

    [Fact]
    public void ReadFromSkipsUnknownFields()
    {
        var act = () => ReadDefinition("""{"unknownField":"value","status":"OPEN","anotherUnknown":123}""");

        act.Should().NotThrow();
        _cache.Status.Should().Be("OPEN");
    }

    [Fact]
    public void ReadFromSkipsComplexUnknownFields()
    {
        var act = () => ReadDefinition("""{"regulators":["MR_INT"],"status":"OPEN","priceLadderDefinition":{"type":"CLASSIC"}}""");

        act.Should().NotThrow();
        _cache.Status.Should().Be("OPEN");
    }

    [Fact]
    public void RepeatedReadDoesNotAllocateWhenStringUnchanged()
    {
        ReadDefinition("""{"status":"OPEN","venue":"Lingfield"}""");
        var firstStatus = _cache.Status;
        var firstVenue = _cache.Venue;

        // Read same values again
        ReadDefinition("""{"status":"OPEN","venue":"Lingfield"}""");

        // Same reference means no new allocation
        _cache.Status.Should().BeSameAs(firstStatus);
        _cache.Venue.Should().BeSameAs(firstVenue);
    }

    [Fact]
    public void RepeatedReadAllocatesWhenStringChanges()
    {
        ReadDefinition("""{"status":"OPEN"}""");

        ReadDefinition("""{"status":"SUSPENDED"}""");

        _cache.Status.Should().Be("SUSPENDED");
    }

    [Fact]
    public void ReadFromParsesTimezoneField()
    {
        ReadDefinition("""{"timezone":"Europe/London"}""");

        _cache.Timezone.Should().Be("Europe/London");
    }

    private void ReadDefinition(string json)
    {
        var bytes = Encoding.UTF8.GetBytes(json);
        var reader = new Utf8JsonReader(bytes);
        reader.Read(); // Move to StartObject
        _cache.ReadFrom(ref reader);
    }
}
