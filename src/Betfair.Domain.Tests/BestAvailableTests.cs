namespace Betfair.Domain.Tests;

public class BestAvailableTests
{
    [Fact]
    public void ReturnNoneIfInputIsNull()
    {
        var available = BestAvailable.Create(null);

        available.HasValue.Should().BeFalse();
    }

    [Theory]
    [InlineData(1.01)]
    [InlineData(3.5)]
    public void LevelZeroPriceIsDisplayedAsAvailablePrice(double price)
    {
        var input = new List<List<double>> { new () { 0, price, 1.99 } };

        var available = BestAvailable.Create(input);

        available.Value.BestPrice.Should().Be(Price.Of(price));
    }

    [Theory]
    [InlineData(1.99)]
    [InlineData(56.21)]
    public void LevelZeroSizeIsDisplayedAsAvailableSize(double size)
    {
        var input = new List<List<double>> { new () { 0, 1.01, size } };

        var available = BestAvailable.Create(input);

        available.Value.BestSize.Should().Be(size);
    }

    [Theory]
    [InlineData(1.99, 3.56, 9.99, 15.54)]
    [InlineData(56.21, 10.43, 29.50, 96.14)]
    public void TotalSizeIsDisplayed(
        double size0,
        double size1,
        double size2,
        double total)
    {
        var input = new List<List<double>>
        {
            new () { 0, 1.01, size0 },
            new () { 1, 1.01, size1 },
            new () { 2, 1.01, size2 },
        };

        var available = BestAvailable.Create(input);

        available.Value.TotalSize.Should().Be(total);
    }

    [Fact]
    public void ProfileTest()
    {
        for (int i = 0; i < 10000; i++)
        {
            var input = new List<List<double>>
            {
                new () { 0, 1.01, 1.99 },
                new () { 1, 1.01, 1.99 },
                new () { 2, 1.01, 1.99 },
            };

            var available = BestAvailable.Create(input);

            available.Value.BestSize.Should().Be(1.99);
        }
    }
}
