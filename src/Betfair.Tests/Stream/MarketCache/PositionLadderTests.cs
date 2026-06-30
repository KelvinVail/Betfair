using Betfair.Stream.MarketCache;

namespace Betfair.Tests.Stream.MarketCache;

public class PositionLadderTests
{
    private readonly PositionLadder _ladder = new ();

    [Fact]
    public void NewLadderHasZeroCount()
    {
        _ladder.Count.Should().Be(0);
    }

    [Theory]
    [InlineData(0, 2.5, 100.0)]
    [InlineData(1, 3.0, 50.0)]
    [InlineData(2, 4.5, 25.0)]
    public void UpdateSetsPriceAndSizeAtPosition(int position, double price, double size)
    {
        _ladder.Update(position, price, size);

        _ladder.GetPrice(position).Should().Be(price);
        _ladder.GetSize(position).Should().Be(size);
    }

    [Fact]
    public void CountReflectsHighestOccupiedPosition()
    {
        _ladder.Update(0, 2.0, 10.0);
        _ladder.Update(2, 4.0, 30.0);

        _ladder.Count.Should().Be(3);
    }

    [Fact]
    public void UpdateWithZeroSizeRemovesPosition()
    {
        _ladder.Update(0, 2.0, 10.0);
        _ladder.Update(1, 3.0, 20.0);
        _ladder.Update(2, 4.0, 30.0);

        _ladder.Update(2, 0, 0);

        _ladder.GetPrice(2).Should().Be(0);
        _ladder.GetSize(2).Should().Be(0);
        _ladder.Count.Should().Be(2);
    }

    [Fact]
    public void RemovingHighestPositionRecalculatesCount()
    {
        _ladder.Update(0, 2.0, 10.0);
        _ladder.Update(1, 3.0, 20.0);
        _ladder.Update(2, 4.0, 30.0);

        _ladder.Update(2, 0, 0);
        _ladder.Update(1, 0, 0);

        _ladder.Count.Should().Be(1);
    }

    [Fact]
    public void RemovingAllPositionsResultsInZeroCount()
    {
        _ladder.Update(0, 2.0, 10.0);
        _ladder.Update(0, 0, 0);

        _ladder.Count.Should().Be(0);
    }

    [Fact]
    public void GetPriceReturnsZeroForUnoccupiedPosition()
    {
        _ladder.GetPrice(5).Should().Be(0);
    }

    [Fact]
    public void GetSizeReturnsZeroForUnoccupiedPosition()
    {
        _ladder.GetSize(5).Should().Be(0);
    }

    [Fact]
    public void GetPriceReturnsZeroForOutOfBoundsPosition()
    {
        _ladder.GetPrice(10).Should().Be(0);
        _ladder.GetPrice(99).Should().Be(0);
    }

    [Fact]
    public void GetSizeReturnsZeroForOutOfBoundsPosition()
    {
        _ladder.GetSize(10).Should().Be(0);
        _ladder.GetSize(99).Should().Be(0);
    }

    [Fact]
    public void UpdateOutOfBoundsPositionIsIgnored()
    {
        _ladder.Update(10, 2.0, 100.0);
        _ladder.Update(99, 3.0, 200.0);

        _ladder.Count.Should().Be(0);
    }

    [Fact]
    public void ClearResetsAllPositions()
    {
        _ladder.Update(0, 2.0, 10.0);
        _ladder.Update(1, 3.0, 20.0);
        _ladder.Update(2, 4.0, 30.0);

        _ladder.Clear();

        _ladder.Count.Should().Be(0);
        _ladder.GetPrice(0).Should().Be(0);
        _ladder.GetSize(0).Should().Be(0);
    }

    [Fact]
    public void UpdateReplacesExistingPositionValues()
    {
        _ladder.Update(0, 2.0, 10.0);
        _ladder.Update(0, 5.0, 50.0);

        _ladder.GetPrice(0).Should().Be(5.0);
        _ladder.GetSize(0).Should().Be(50.0);
        _ladder.Count.Should().Be(1);
    }

    [Fact]
    public void AllTenPositionsCanBeUsed()
    {
        for (int i = 0; i < 10; i++)
            _ladder.Update(i, i + 1.0, (i + 1) * 10.0);

        _ladder.Count.Should().Be(10);
        _ladder.GetPrice(9).Should().Be(10.0);
        _ladder.GetSize(9).Should().Be(100.0);
    }
}
