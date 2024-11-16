using System.Buffers.Binary;
using System.Buffers.Text;
using System.Globalization;
using System.Text;
using Betfair.Core.Login;
using Betfair.Extensions.ByteReaders;
using Betfair.Extensions.Markets;
using Betfair.Extensions.Tests.TestDoubles;

namespace Betfair.Extensions.Tests.ByteReaderTests;

public class BetfairJsonReaderTests
{
    private readonly byte[] _byteLine;

    public BetfairJsonReaderTests()
    {
        var path = Path.Combine("Data", "MarketDefinitions", "InitialImage.json");

        var line = File.ReadAllText(path);
        var cleanLine = line.Replace("\r", string.Empty, StringComparison.OrdinalIgnoreCase).Replace("\n", string.Empty, StringComparison.OrdinalIgnoreCase);
        _byteLine = Encoding.UTF8.GetBytes(cleanLine);
    }

    [Fact]
    public void CanReadTheWholeMarket()
    {
        var market = Market.Create(new Credentials("u", "p", "a"), "1.235123059", new SubscriptionStub()).Value;
        var path = Path.Combine("Data", "messages.txt");

        var lineCount = 0;
        foreach (var line in File.ReadLines(path))
        {
            lineCount++;
            var reader = new BetfairJsonReader(Encoding.UTF8.GetBytes(line));

            try
            {
                market.ReadChangeMessage(ref reader);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

    }

    [Fact]
    public void CanReadTheWholeLine()
    {
        var reader = new BetfairJsonReader(_byteLine);

        while (reader.Read())
        {
        }
    }

    [Fact]
    public void DetectsTheFirstPropertyName()
    {
        var reader = new BetfairJsonReader(_byteLine);

        while (reader.Read())
            if (reader.TokenType == JsonTokenType.PropertyName) break;

        var propertyName = Encoding.UTF8.GetString(reader.ValueSpan);
        propertyName.Should().Be("op");
    }

    [Fact]
    public void DetectsTheFirstPropertyValue()
    {
        var reader = new BetfairJsonReader(_byteLine);

        while (reader.Read())
            if (reader.TokenType == JsonTokenType.PropertyName && reader.ValueSpan.SequenceEqual("op"u8)) break;

        reader.Read();
        Encoding.UTF8.GetString(reader.ValueSpan).Should().Be("mcm");
    }

    [Fact]
    public void DetectsTheFirstIntPropertyValue()
    {
        var reader = new BetfairJsonReader(_byteLine);

        while (reader.Read())
            if (reader.TokenType == JsonTokenType.PropertyName && reader.ValueSpan.SequenceEqual("id"u8)) break;

        reader.Read();
        reader.GetInt32().Should().Be(2);
    }

    [Fact]
    public void DetectsTheFirstLongPropertyValue()
    {
        var reader = new BetfairJsonReader(_byteLine);

        while (reader.Read())
            if (reader.TokenType == JsonTokenType.PropertyName && reader.ValueSpan.SequenceEqual("pt"u8)) break;

        reader.Read();
        reader.GetInt64().Should().Be(1730202588118);
    }

    [Fact]
    public void DetectsTheFirstDataTimePropertyValue()
    {
        var reader = new BetfairJsonReader(_byteLine);

        while (reader.Read())
            if (reader.TokenType == JsonTokenType.PropertyName && reader.ValueSpan.SequenceEqual("marketTime"u8)) break;

        reader.Read();
        var expected = DateTimeOffset.Parse("2024-10-29T12:40:00.000Z", DateTimeFormatInfo.CurrentInfo);
        reader.GetDateTimeOffset().Should().Be(expected);
    }

    [Fact]
    public void DetectsTheFirstBoolTrue()
    {
        var reader = new BetfairJsonReader(_byteLine);

        while (reader.Read())
            if (reader.TokenType == JsonTokenType.PropertyName && reader.ValueSpan.SequenceEqual("inPlay"u8)) break;

        reader.Read();
        reader.GetBoolean().Should().BeTrue();
    }

    [Fact]
    public void DetectsTheFirstBoolFalse()
    {
        var reader = new BetfairJsonReader(_byteLine);

        while (reader.Read())
            if (reader.TokenType == JsonTokenType.PropertyName && reader.ValueSpan.SequenceEqual("bspReconciled"u8)) break;

        reader.Read();
        reader.GetBoolean().Should().BeFalse();
    }

    [Fact]
    public void DetectsTheFirstDouble()
    {
        var reader = new BetfairJsonReader(_byteLine);

        while (reader.Read())
            if (reader.TokenType == JsonTokenType.PropertyName && reader.ValueSpan.SequenceEqual("tv"u8)) break;

        reader.Read();
        reader.GetDouble().Should().Be(17540.83);
    }
}
