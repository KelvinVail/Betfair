using Betfair.Markets;

namespace Betfair.Tests.Markets;

public class AvailableTests
{
    [Fact]
    public void Exists()
    {
        var available = Available.Create();
    }
}
