using Betfair.Api.Accounts.Endpoints.GetAccountStatement.Enums;

namespace Betfair.Tests.Api.Accounts.Endpoints.GetAccountStatement.Enums;

public class IncludeItemTests
{
    [Fact]
    public void EnumIncludeItemHasCorrectValues()
    {
        ((int)IncludeItem.All).Should().Be(0);
        ((int)IncludeItem.DepositsWithdrawals).Should().Be(1);
        ((int)IncludeItem.Exchange).Should().Be(2);
        ((int)IncludeItem.PokerRoom).Should().Be(3);
    }

    [Fact]
    public void EnumIncludeItemHasCorrectStringValues()
    {
        IncludeItem.All.ToString().Should().Be("All");
        IncludeItem.DepositsWithdrawals.ToString().Should().Be("DepositsWithdrawals");
        IncludeItem.Exchange.ToString().Should().Be("Exchange");
        IncludeItem.PokerRoom.ToString().Should().Be("PokerRoom");
    }

    [Fact]
    public void CanParseEnumIncludeItemFromString()
    {
        IncludeItem.All.Should().Be(Enum.Parse<IncludeItem>("All"));
        IncludeItem.DepositsWithdrawals.Should().Be(Enum.Parse<IncludeItem>("DepositsWithdrawals"));
        IncludeItem.Exchange.Should().Be(Enum.Parse<IncludeItem>("Exchange"));
        IncludeItem.PokerRoom.Should().Be(Enum.Parse<IncludeItem>("PokerRoom"));
    }

    [Fact]
    public void CanParseEnumIncludeItemFromJsonString()
    {
        IncludeItem.All.Should().Be(JsonSerializer.Deserialize<IncludeItem>("\"ALL\""));
        IncludeItem.DepositsWithdrawals.Should().Be(JsonSerializer.Deserialize<IncludeItem>("\"DEPOSITS_WITHDRAWALS\""));
        IncludeItem.Exchange.Should().Be(JsonSerializer.Deserialize<IncludeItem>("\"EXCHANGE\""));
        IncludeItem.PokerRoom.Should().Be(JsonSerializer.Deserialize<IncludeItem>("\"POKER_ROOM\""));
    }

    [Fact]
    public void CanSerializeEnumIncludeItemToJsonString()
    {
        JsonSerializer.Serialize(IncludeItem.All).Should().Be("\"ALL\"");
        JsonSerializer.Serialize(IncludeItem.DepositsWithdrawals).Should().Be("\"DEPOSITS_WITHDRAWALS\"");
        JsonSerializer.Serialize(IncludeItem.Exchange).Should().Be("\"EXCHANGE\"");
        JsonSerializer.Serialize(IncludeItem.PokerRoom).Should().Be("\"POKER_ROOM\"");
    }

    [Theory]
    [InlineData(IncludeItem.All, "ALL")]
    [InlineData(IncludeItem.DepositsWithdrawals, "DEPOSITS_WITHDRAWALS")]
    [InlineData(IncludeItem.Exchange, "EXCHANGE")]
    [InlineData(IncludeItem.PokerRoom, "POKER_ROOM")]
    public void SerializationRoundTripWorks(IncludeItem enumValue, string expectedJson)
    {
        var serialized = JsonSerializer.Serialize(enumValue);
        var deserialized = JsonSerializer.Deserialize<IncludeItem>(serialized);

        serialized.Should().Be($"\"{expectedJson}\"");
        deserialized.Should().Be(enumValue);
    }

    [Fact]
    public void AllEnumValuesCanBeSerializedAndDeserialized()
    {
        var allValues = Enum.GetValues<IncludeItem>();

        foreach (var value in allValues)
        {
            var serialized = JsonSerializer.Serialize(value);
            var deserialized = JsonSerializer.Deserialize<IncludeItem>(serialized);

            serialized.Should().NotBeNullOrEmpty($"IncludeItem.{value} should serialize to a non-empty string");
            deserialized.Should().Be(value, $"IncludeItem.{value} should deserialize back to the same value");
        }
    }
}
