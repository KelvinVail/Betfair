using Betfair.Betting;

namespace Betfair.Tests.Betting;

public class EventTypeTests
{
    [Fact]
    public void TwoEmptyEventTypesAreEqual()
    {
        var event1 = new EventType();
        var event2 = new EventType();

        event1.Should().Be(event2);
    }

    [Fact]
    public void TwoEventTypesWithTheSameIdAreEqual()
    {
        var event1 = new EventType { Id = "1" };
        var event2 = new EventType { Id = "1" };

        event1.Should().Be(event2);
    }

    [Fact]
    public void TwoEventTypesWithDifferentIdsAreNotEqual()
    {
        var event1 = new EventType { Id = "1", Name = "Same" };
        var event2 = new EventType { Id = "2", Name = "Same" };

        event1.Should().NotBe(event2);
    }
}
