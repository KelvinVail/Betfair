using Betfair.Domain;

namespace Betfair.Tests.Domain;

public class SizeTests
{
    [Theory]
    [InlineData(2)]
    [InlineData(9.99)]
    [InlineData(1234.5678)]
    public void CanBeEqual(decimal size)
    {
        var size1 = Size.Of(size);
        var size2 = Size.Of(size);

        size1.Should().Be(size2);
        size1.CompareTo(size2).Should().Be(0);
    }

    [Theory]
    [InlineData(10.001, 10.00)]
    [InlineData(9.9999999999, 9.99)]
    public void SizesAreRoundedDownToTwoDecimalPlaces(decimal value, decimal expected)
    {
        var size = Size.Of(value);

        size.Value.Should().Be(expected);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-9.99)]
    public void CanNotBeLessThanZero(decimal value)
    {
        var size = Size.Of(value);

        size.Value.Should().Be(0);
    }

    [Theory]
    [InlineData(2, 1)]
    [InlineData(9.99, 9.98)]
    public void CanBeComparedUsingGreaterAndLessThan(decimal value1, decimal value2)
    {
        var size1 = Size.Of(value1);
        var size2 = Size.Of(value2);

        size1.Should().BeGreaterThan(size2);
        size1.CompareTo(size2).Should().Be(1);
        size2.Should().BeLessThan(size1);
        size2.CompareTo(size1).Should().Be(-1);
    }

    [Theory]
    [InlineData(2, 1)]
    [InlineData(9.99, 9.98)]
    [InlineData(10, 10)]
    public void CanBeComparedUsingGreaterOrEqualAndLessThanOrEqual(decimal value1, decimal value2)
    {
        var size1 = Size.Of(value1);
        var size2 = Size.Of(value2);

        size1.Should().BeGreaterOrEqualTo(size2);
        size2.Should().BeLessThanOrEqualTo(size1);
    }

    [Fact]
    public void StringIsFormattedWithCommaSeparator()
    {
        var size = Size.Of(1000000);

        size.ToString().Should().Be("1,000,000.00");
    }

    [Theory]
    [InlineData(1, 2, 3)]
    [InlineData(10, 10, 20)]
    [InlineData(5.25, 2.88, 8.13)]
    public void CanBeAdded(decimal value1, decimal value2, decimal expected)
    {
        var size1 = Size.Of(value1);
        var size2 = Size.Of(value2);

        var result = size1 + size2;
        result.Value.Should().Be(expected);

        var resultAdd = size1.Add(size2);
        resultAdd.Value.Should().Be(expected);
    }

    [Theory]
    [InlineData(2, 1, 1)]
    [InlineData(10, 10, 0)]
    [InlineData(5.25, 2.88, 2.37)]
    public void CanBeSubtracted(decimal value1, decimal value2, decimal expected)
    {
        var size1 = Size.Of(value1);
        var size2 = Size.Of(value2);

        var result = size1 - size2;
        result.Value.Should().Be(expected);

        var resultAdd = size1.Subtract(size2);
        resultAdd.Value.Should().Be(expected);
    }

    [Theory]
    [InlineData(2, 2, 4)]
    [InlineData(10, 10, 100)]
    [InlineData(5.25, 2.88, 15.12)]
    public void CanBeMultiplied(decimal value1, decimal value2, decimal expected)
    {
        var size1 = Size.Of(value1);

        var result = size1 * value2;
        result.Value.Should().Be(expected);

        var resultAdd = size1.Multiply(value2);
        resultAdd.Value.Should().Be(expected);
    }

    [Theory]
    [InlineData(4, 2, 2)]
    [InlineData(100, 10, 10)]
    [InlineData(15.12, 2.88, 5.25)]
    public void CanBeDivided(decimal value1, decimal value2, decimal expected)
    {
        var size1 = Size.Of(value1);

        var result = size1 / value2;
        result.Value.Should().Be(expected);

        var resultAdd = size1.Divide(value2);
        resultAdd.Value.Should().Be(expected);
    }
}