using Betfair.Core;

namespace Betfair.Tests.Core;

public class SideTests
{
    [Fact]
    public void CanCreateBack()
    {
        Side.Back.Value.Should().Be("BACK");
        Side.Back.ToString().Should().Be("BACK");
    }

    [Fact]
    public void CanCreateLay()
    {
        Side.Lay.Value.Should().Be("LAY");
        Side.Lay.ToString().Should().Be("LAY");
    }
}
