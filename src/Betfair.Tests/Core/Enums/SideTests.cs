using Betfair.Core.Enums;

namespace Betfair.Tests.Core.Enums;

public class SideTests
{
    [Fact]
    public void EnumSideHasCorrectValues()
    {
        Side.Back.Should().Be(0);
        ((int)Side.Lay).Should().Be(1);
    }

    [Fact]
    public void EnumSideHasCorrectStringValues()
    {
        Side.Back.ToString().Should().Be("Back");
        Side.Lay.ToString().Should().Be("Lay");
    }

    [Fact]
    public void CanParseEnumSideFromString()
    {
        Side.Back.Should().Be(Enum.Parse<Side>("Back"));
        Side.Lay.Should().Be(Enum.Parse<Side>("Lay"));
    }

    [Fact]
    public void CanParseEnumSideFromJsonString()
    {
        Side.Back.Should().Be(JsonSerializer.Deserialize<Side>("\"BACK\""));
        Side.Lay.Should().Be(JsonSerializer.Deserialize<Side>("\"LAY\""));
    }

    [Fact]
    public void CarSerializeToJsonString()
    {
        JsonSerializer.Serialize(Side.Back).Should().Be("\"BACK\"");
        JsonSerializer.Serialize(Side.Lay).Should().Be("\"LAY\"");
    }
}
