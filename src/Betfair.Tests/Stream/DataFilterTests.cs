using Betfair.Stream.Messages;

namespace Betfair.Tests.Stream;

public class DataFilterTests
{
    private readonly DataFilter _filter = new();

    [Fact]
    public void WhenInitializedLadderLevelsIsThree() =>
        _filter.LadderLevels.Should().Be(3);

    [Fact]
    public void WhenInitializedFieldsIsNull() =>
        _filter.Fields.Should().BeNull();

    [Fact]
    public void CanBeInitializedWithMarketDefinition()
    {
        _filter.WithMarketDefinition();

        _filter.Fields.Should().Contain("EX_MARKET_DEF");
    }

    [Fact]
    public void CanBeInitializedWithBestPricesIncludingVirtual()
    {
        _filter.WithBestPricesIncludingVirtual();

        _filter.Fields.Should().Contain("EX_BEST_OFFERS_DISP");
    }

    [Fact]
    public void CanBeInitializedWithMultipleDataFields()
    {
        _filter.WithBestPricesIncludingVirtual();
        _filter.WithMarketDefinition();

        _filter.Fields.Should().Contain("EX_BEST_OFFERS_DISP");
        _filter.Fields.Should().Contain("EX_MARKET_DEF");
    }

    [Fact]
    public void CanBeInitializedWithBestPrices()
    {
        _filter.WithBestPrices();

        _filter.Fields.Should().Contain("EX_BEST_OFFERS");
    }

    [Fact]
    public void CanBeInitializedWithFullOffersLadder()
    {
        _filter.WithFullOffersLadder;

        _filter.Fields.Should().Contain("EX_ALL_OFFERS");
    }

    [Fact]
    public void CanBeInitializedWithFullTradedLadder()
    {
        _filter.WithFullTradedLadder();

        _filter.Fields.Should().Contain("EX_TRADED");
    }

    [Fact]
    public void CanBeInitializedWithMarketAndRunnerTradedVolume()
    {
        _filter.WithTradedVolume();

        _filter.Fields.Should().Contain("EX_TRADED_VOL");
    }

    [Fact]
    public void CanBeInitializedWithLastTradedPrice()
    {
        _filter.WithLastTradedPrice();

        _filter.Fields.Should().Contain("EX_LTP");
    }

    [Fact]
    public void CanBeInitializedWithStartingPriceLadder()
    {
        _filter.WithStartingPriceLadder();

        _filter.Fields.Should().Contain("SP_TRADED");
    }

    [Fact]
    public void CanBeInitializedWithStartingPriceProjection()
    {
        _filter.WithStartingPriceProjection();

        _filter.Fields.Should().Contain("SP_PROJECTED");
    }

    [Fact]
    public void CanBeInitializedWithAllDataFields()
    {
        _filter.WithMarketDefinition();
        _filter.WithBestPricesIncludingVirtual();
        _filter.WithBestPrices();
        _filter.WithFullOffersLadder;
        _filter.WithFullTradedLadder();
        _filter.WithTradedVolume();
        _filter.WithLastTradedPrice();
        _filter.WithStartingPriceLadder();
        _filter.WithStartingPriceProjection();

        _filter.Fields.Should().NotBeNull();
        _filter.Fields?.Count.Should().Be(9);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void CanBeInitializedWithLadderLevel(int levels)
    {
        _filter.WithLadderLevels(levels);
        Assert.Equal(levels, _filter.LadderLevels);
    }

    [Fact]
    public void CanNotBeCreatedWithMoreThanTenLadderLevels()
    {
        _filter.WithLadderLevels(11);

        _filter.LadderLevels.Should().Be(10);
    }
}