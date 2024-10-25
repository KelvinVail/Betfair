using Betfair.Extensions.Markets;

namespace Betfair.Extensions.Tests.Markets;

public class RunnerTests
{
    [Fact]
    public void RunnerIdMustBeGreaterThanZero()
    {
        var result = Runner.Create(0);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Runner id must be greater than zero.");
    }
}