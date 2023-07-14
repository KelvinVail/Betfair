using Betfair.Domain;

namespace Betfair.Tests.Domain;

public class LadderTests
{
    [Fact]
    public void AvailableReturnsZeroIfPriceIsNull()
    {
        var ladder = Ladder.Create();

        ladder.Size(null!).Value.Should().Be(0);
    }

    [Fact]
    public void AvailableReturnsZeroIfStepHasNotBeenSet()
    {
        var ladder = Ladder.Create();

        ladder.Size(Price.Of(1.01m)).Value.Should().Be(0);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(1.99)]
    [InlineData(999)]
    public void StepCanBeAddedWithAmountAvailable(decimal size)
    {
        var price = Price.Of(1.01m);
        var available = Size.Of(size);

        var ladder = Ladder.Create();
        ladder.AddOrUpdate(price, available);

        ladder.Size(price).Value.Should().Be(size);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(1.99)]
    [InlineData(999)]
    public void StepCanBeUpdatedWithAmountAvailable(decimal size)
    {
        var price = Price.Of(1.01m);
        var available = Size.Of(size);
        var ladder = Ladder.Create();
        ladder.AddOrUpdate(price, Size.Of(100));

        ladder.AddOrUpdate(price, available);

        ladder.Size(price).Value.Should().Be(size);
    }

    [Fact]
    public void LadderCanBeEnumerated()
    {
        var ladder = Ladder.Create();
        ladder.AddOrUpdate(Price.Of(1.01m), Size.Of(1.01m));

        ladder.First().Key.DecimalOdds.Should().Be(1.01m);
    }
}
