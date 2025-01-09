using Betfair.Extensions.Markets;

namespace Betfair.Extensions.Tests.Markets;

public class PriceLadderTests
{
    [Theory]
        [InlineData(1.5, 100)]
        [InlineData(2.5, 200)]
        public void UpdateShouldAddOrUpdateEntry(double price, double size)
        {
            var ladder = new PriceLadderDictionary();

            ladder.Update(price, size);

            ladder.ContainsKey(price).Should().BeTrue();
            ladder[price].Should().Be(size);
        }

        [Theory]
        [InlineData(1.5)]
        [InlineData(2.5)]
        public void ContainsKeyShouldReturnTrueIfKeyExists(double price)
        {
            var ladder = new PriceLadderDictionary();
            ladder.Update(price, 100);

            ladder.ContainsKey(price).Should().BeTrue();
        }

        [Theory]
        [InlineData(1.5)]
        [InlineData(2.5)]
        public void ContainsKeyShouldReturnFalseIfKeyDoesNotExist(double price)
        {
            var ladder = new PriceLadderDictionary();

            ladder.ContainsKey(price).Should().BeFalse();
        }

        [Theory]
        [InlineData(1.5, 100)]
        [InlineData(2.5, 200)]
        public void TryGetValueShouldReturnTrueAndValueIfKeyExists(double price, double size)
        {
            var ladder = new PriceLadderDictionary();
            ladder.Update(price, size);

            bool result = ladder.TryGetValue(price, out var value);

            result.Should().BeTrue();
            value.Should().Be(size);
        }

        [Theory]
        [InlineData(1.5)]
        [InlineData(2.5)]
        public void TryGetValueShouldReturnFalseIfKeyDoesNotExist(double price)
        {
            var ladder = new PriceLadderDictionary();

            bool result = ladder.TryGetValue(price, out var value);

            result.Should().BeFalse();
            value.Should().Be(default);
        }

        [Theory]
        [InlineData(1.5, 100)]
        [InlineData(2.5, 200)]
        public void IndexerShouldReturnCorrectValue(double price, double size)
        {
            var ladder = new PriceLadderDictionary();
            ladder.Update(price, size);

            var value = ladder[price];

            value.Should().Be(size);
        }

        [Fact]
        public void KeysShouldReturnAllKeys()
        {
            var ladder = new PriceLadderDictionary();
            ladder.Update(1.5, 100);
            ladder.Update(2.5, 200);

            ladder.Keys.Should().Contain(1.5);
            ladder.Keys.Should().Contain(2.5);
        }

        [Fact]
        public void ValuesShouldReturnAllValues()
        {
            var ladder = new PriceLadderDictionary();
            ladder.Update(1.5, 100);
            ladder.Update(2.5, 200);

            ladder.Values.Should().Contain(100);
            ladder.Values.Should().Contain(200);
        }

        [Fact]
        public void CountShouldReturnCorrectNumberOfEntries()
        {
            var ladder = new PriceLadderDictionary();
            ladder.Update(1.5, 100);
            ladder.Update(2.5, 200);

            ladder.Count.Should().Be(2);
        }
}
