using Betfair.Extensions.Markets;

namespace Betfair.Extensions.Tests.Markets;

public class LevelLadderDictionaryTests
{
    [Theory]
    [InlineData(1, 1.5, 100)]
    [InlineData(2, 2.5, 200)]
    public void UpdateShouldAddEntry(int level, double price, double size)
    {
        var ladder = new LevelLadderDictionary();

        ladder.Update(level, price, size);

        ladder.ContainsKey(level).Should().BeTrue();
        ladder[level].Should().Be((price, size));
    }

    [Theory]
    [InlineData(1, 1.5, 100)]
    [InlineData(2, 2.5, 200)]
    public void UpdateShouldUpdateEntry(int level, double price, double size)
    {
        var ladder = new LevelLadderDictionary();
        ladder.Update(level, 0, 0);

        ladder.Update(level, price, size);

        ladder.ContainsKey(level).Should().BeTrue();
        ladder[level].Should().Be((price, size));
    }

    [Fact]
    public void ContainsKeyShouldReturnTrueIfKeyExists()
    {
        var ladder = new LevelLadderDictionary();
        const int level = 1;

        ladder.Update(level, 1.5, 100);

        ladder.ContainsKey(level).Should().BeTrue();
    }

    [Fact]
    public void ContainsKeyShouldReturnFalseIfKeyDoesNotExist()
    {
        var ladder = new LevelLadderDictionary();
        const int level = 1;

        ladder.ContainsKey(level).Should().BeFalse();
    }

    [Theory]
    [InlineData(1, 1.5, 100)]
    [InlineData(2, 2.5, 200)]
    public void TryGetValueShouldReturnTrueAndValueIfKeyExists(int level, double price, double size)
    {
        var ladder = new LevelLadderDictionary();
        ladder.Update(level, price, size);

        bool result = ladder.TryGetValue(level, out var value);

        result.Should().BeTrue();
        value.Should().Be((price, size));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void TryGetValueShouldReturnFalseIfKeyDoesNotExist(int level)
    {
        var ladder = new LevelLadderDictionary();

        bool result = ladder.TryGetValue(level, out var value);

        result.Should().BeFalse();
        value.Should().Be(default);
    }

    [Theory]
    [InlineData(1, 1.5, 100)]
    [InlineData(2, 2.5, 200)]
    public void IndexerShouldReturnCorrectValue(int level, double price, double size)
    {
        var ladder = new LevelLadderDictionary();
        ladder.Update(level, price, size);

        ladder[level].Should().Be((price, size));
    }

    [Fact]
    public void KeysShouldReturnAllKeys()
    {
        var ladder = new LevelLadderDictionary();
        ladder.Update(1, 1.5, 100);
        ladder.Update(2, 2.5, 200);

        ladder.Keys.Should().Contain(1);
        ladder.Keys.Should().Contain(2);
    }

    [Fact]
    public void ValuesShouldReturnAllValues()
    {
        var ladder = new LevelLadderDictionary();
        ladder.Update(1, 1.5, 100);
        ladder.Update(2, 2.5, 200);

        ladder.Values.Should().Contain((1.5, 100));
        ladder.Values.Should().Contain((2.5, 200));
    }

    [Fact]
    public void CountShouldReturnCorrectNumberOfEntries()
    {
        var ladder = new LevelLadderDictionary();
        ladder.Update(1, 1.5, 100);
        ladder.Update(2, 2.5, 200);

        ladder.Count.Should().Be(2);
    }
}
