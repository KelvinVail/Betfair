using System.Collections;
using Betfair.Extensions.Markets;

namespace Betfair.Extensions.Tests.Markets;

public class PriceDictionaryTests
{
    [Fact]
    public void WhenCreatedContainsAllValidPrices()
    {
        var ladder = new PriceDictionary();

        ladder.Count.Should().Be(PriceExtensions.ValidPrices.Length);
    }

    [Fact]
    public void CanRetrieveAValueUsingPriceAsTheKey()
    {
        var ladder = new PriceDictionary();

        var value = ladder[Price.Of(1.01)];

        value.Should().Be(0);
    }

    [Theory]
    [InlineData(1.01, 9.99)]
    [InlineData(3.5, 12345.92)]
    public void CanUpdateAValueUsingPriceAsTheKey(double price, double size)
    {
        var ladder = new PriceDictionary();

        ladder.Update(Price.Of(price), size);

        ladder[Price.Of(price)].Should().Be(size);
    }

    [Fact]
    public void AnInvalidPriceCanBeAddedToTheDictionary() =>
        CanUpdateAValueUsingPriceAsTheKey(101.5, 9.99);

    [Fact]
    public void AnInvalidPriceCanBeUpdatedInTheDictionary()
    {
        var ladder = new PriceDictionary();

        ladder.Update(Price.Of(101.5), 9.99);

        ladder[Price.Of(101.5)].Should().Be(9.99);
    }

    [Fact]
    public void ReturnsTheEnumerator()
    {
        var ladder = new PriceDictionary();

        using var kvp = ladder.GetEnumerator();

        kvp.Should().NotBeNull();
    }

    [Fact]
    public void ReturnsTheEnumeratorAsAnIEnumerable()
    {
        var ladder = new PriceDictionary();

        var kvp = (IEnumerable)ladder;

        kvp.Should().NotBeNull();
    }

    [Fact]
    public void TheEnumeratorContainsAllPrices()
    {
        var ladder = new PriceDictionary();

        var prices = ladder.Select(kvp => kvp.Key);

        prices.Should().BeEquivalentTo(PriceExtensions.ValidPrices.Select(Price.Of));
    }

    [Fact]
    public void ContainsKeyReturnsTrueForAValidPrice()
    {
        var ladder = new PriceDictionary();

        ladder.ContainsKey(Price.Of(1.01)).Should().BeTrue();
    }

    [Fact]
    public void ContainsKeyReturnsFalseForAnInvalidPrice()
    {
        var ladder = new PriceDictionary();

        ladder.ContainsKey(Price.Of(101.5)).Should().BeFalse();
    }

    [Fact]
    public void TryGetValueReturnsTrueForAValidPrice()
    {
        var ladder = new PriceDictionary();

        ladder.TryGetValue(Price.Of(1.01), out var value).Should().BeTrue();
        value.Should().Be(0);
    }

    [Fact]
    public void TryGetValueReturnsFalseForAnInvalidPrice()
    {
        var ladder = new PriceDictionary();

        ladder.TryGetValue(Price.Of(101.5), out var value).Should().BeFalse();
        value.Should().Be(0);
    }

    [Fact]
    public void KeysReturnsAllValidPrices()
    {
        var ladder = new PriceDictionary();

        ladder.Keys.Should().BeEquivalentTo(PriceExtensions.ValidPrices.Select(Price.Of));
    }
}
