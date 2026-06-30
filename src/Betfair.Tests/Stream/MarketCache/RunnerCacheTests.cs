using Betfair.Stream.MarketCache;

namespace Betfair.Tests.Stream.MarketCache;

public class RunnerCacheTests
{
    [Theory]
    [InlineData(12345)]
    [InlineData(99999)]
    public void SelectionIdIsSetFromConstructor(long selectionId)
    {
        var runner = new RunnerCache(selectionId);

        runner.SelectionId.Should().Be(selectionId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1.5)]
    [InlineData(-0.25)]
    public void HandicapIsSetFromConstructor(double handicap)
    {
        var runner = new RunnerCache(1, handicap);

        runner.Handicap.Should().Be(handicap);
    }

    [Fact]
    public void HandicapDefaultsToZero()
    {
        var runner = new RunnerCache(1);

        runner.Handicap.Should().Be(0);
    }

    [Fact]
    public void LastTradedPriceDefaultsToNaN()
    {
        var runner = new RunnerCache(1);

        runner.HasLastTradedPrice.Should().BeFalse();
        double.IsNaN(runner.LastTradedPrice).Should().BeTrue();
    }

    [Fact]
    public void TotalMatchedDefaultsToNaN()
    {
        var runner = new RunnerCache(1);

        runner.HasTotalMatched.Should().BeFalse();
        double.IsNaN(runner.TotalMatched).Should().BeTrue();
    }

    [Fact]
    public void StartingPriceNearDefaultsToNaN()
    {
        var runner = new RunnerCache(1);

        runner.HasStartingPriceNear.Should().BeFalse();
        double.IsNaN(runner.StartingPriceNear).Should().BeTrue();
    }

    [Fact]
    public void StartingPriceFarDefaultsToNaN()
    {
        var runner = new RunnerCache(1);

        runner.HasStartingPriceFar.Should().BeFalse();
        double.IsNaN(runner.StartingPriceFar).Should().BeTrue();
    }

    [Theory]
    [InlineData(5.5)]
    [InlineData(100.0)]
    public void SettingLastTradedPriceMakesHasLastTradedPriceTrue(double price)
    {
        var runner = new RunnerCache(1) { LastTradedPrice = price };

        runner.HasLastTradedPrice.Should().BeTrue();
        runner.LastTradedPrice.Should().Be(price);
    }

    [Theory]
    [InlineData(1000.0)]
    [InlineData(0.5)]
    public void SettingTotalMatchedMakesHasTotalMatchedTrue(double volume)
    {
        var runner = new RunnerCache(1) { TotalMatched = volume };

        runner.HasTotalMatched.Should().BeTrue();
        runner.TotalMatched.Should().Be(volume);
    }

    [Fact]
    public void SettingStartingPriceNearMakesHasStartingPriceNearTrue()
    {
        var runner = new RunnerCache(1) { StartingPriceNear = 3.5 };

        runner.HasStartingPriceNear.Should().BeTrue();
        runner.StartingPriceNear.Should().Be(3.5);
    }

    [Fact]
    public void SettingStartingPriceFarMakesHasStartingPriceFarTrue()
    {
        var runner = new RunnerCache(1) { StartingPriceFar = 7.0 };

        runner.HasStartingPriceFar.Should().BeTrue();
        runner.StartingPriceFar.Should().Be(7.0);
    }

    [Fact]
    public void AllPriceLaddersAreInitialized()
    {
        var runner = new RunnerCache(1);

        runner.AvailableToBack.Should().NotBeNull();
        runner.AvailableToLay.Should().NotBeNull();
        runner.Traded.Should().NotBeNull();
        runner.StartingPriceBack.Should().NotBeNull();
        runner.StartingPriceLay.Should().NotBeNull();
    }

    [Fact]
    public void AllPositionLaddersAreInitialized()
    {
        var runner = new RunnerCache(1);

        runner.BestAvailableToBack.Should().NotBeNull();
        runner.BestAvailableToLay.Should().NotBeNull();
        runner.BestDisplayAvailableToBack.Should().NotBeNull();
        runner.BestDisplayAvailableToLay.Should().NotBeNull();
    }

    [Fact]
    public void ClearLaddersResetsAllLadders()
    {
        var runner = new RunnerCache(1)
        {
            LastTradedPrice = 5.0,
            TotalMatched = 1000.0,
            StartingPriceNear = 3.0,
            StartingPriceFar = 7.0,
        };
        runner.AvailableToBack.Update(2.0, 50.0);
        runner.AvailableToLay.Update(3.0, 60.0);
        runner.BestAvailableToBack.Update(0, 2.0, 50.0);
        runner.Traded.Update(4.0, 100.0);

        runner.ClearLadders();

        runner.HasLastTradedPrice.Should().BeFalse();
        runner.HasTotalMatched.Should().BeFalse();
        runner.HasStartingPriceNear.Should().BeFalse();
        runner.HasStartingPriceFar.Should().BeFalse();
        runner.AvailableToBack.Count.Should().Be(0);
        runner.AvailableToLay.Count.Should().Be(0);
        runner.BestAvailableToBack.Count.Should().Be(0);
        runner.Traded.Count.Should().Be(0);
    }
}
