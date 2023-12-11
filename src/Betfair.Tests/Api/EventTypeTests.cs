using Betfair.Api;

namespace Betfair.Tests.Api;

public class EventTypeTests
{
    [Fact]
    public void CanCreateEventTypeSoccer()
    {
        var type = EventType.Soccer;

        type.Id.Should().Be(1);
    }

    [Fact]
    public void CanCreateEventTypeTennis()
    {
        var type = EventType.Tennis;

        type.Id.Should().Be(2);
    }

    [Fact]
    public void CanCreateEventTypeGolf()
    {
        var type = EventType.Golf;

        type.Id.Should().Be(3);
    }

    [Fact]
    public void CanCreateEventTypeCricket()
    {
        var type = EventType.Cricket;

        type.Id.Should().Be(4);
    }

    [Fact]
    public void CanCreateEventTypeRugbyUnion()
    {
        var type = EventType.RugbyUnion;

        type.Id.Should().Be(5);
    }

    [Fact]
    public void CanCreateEventTypeRugbyLeague()
    {
        var type = EventType.RugbyLeague;

        type.Id.Should().Be(1477);
    }

    [Fact]
    public void CanCreateEventTypeBoxing()
    {
        var type = EventType.Boxing;

        type.Id.Should().Be(6);
    }

    [Fact]
    public void CanCreateEventTypeHorseRacing()
    {
        var type = EventType.HorseRacing;

        type.Id.Should().Be(7);
    }

    [Fact]
    public void CanCreateEventTypeMotorSport()
    {
        var type = EventType.MotorSport;

        type.Id.Should().Be(8);
    }

    [Fact]
    public void CanCreateEventTypeESports()
    {
        var type = EventType.ESports;

        type.Id.Should().Be(27454571);
    }

    [Fact]
    public void CanCreateEventTypeSpecialBets()
    {
        var type = EventType.SpecialBets;

        type.Id.Should().Be(10);
    }

    [Fact]
    public void CanCreateEventTypeVolleyball()
    {
        var type = EventType.Volleyball;

        type.Id.Should().Be(998917);
    }

    [Fact]
    public void CanCreateEventTypeCycling()
    {
        var type = EventType.Cycling;

        type.Id.Should().Be(11);
    }

    [Fact]
    public void CanCreateEventTypeGaelicGames()
    {
        var type = EventType.GaelicGames;

        type.Id.Should().Be(2152880);
    }

    [Fact]
    public void CanCreateEventTypeSnooker()
    {
        var type = EventType.Snooker;

        type.Id.Should().Be(6422);
    }

    [Fact]
    public void CanCreateEventTypeAmericanFootball()
    {
        var type = EventType.AmericanFootball;

        type.Id.Should().Be(6423);
    }

    [Fact]
    public void CanCreateEventTypeBaseball()
    {
        var type = EventType.Baseball;

        type.Id.Should().Be(7511);
    }

    [Fact]
    public void CanCreateEventTypeWinterSports()
    {
        var type = EventType.WinterSports;

        type.Id.Should().Be(451485);
    }

    [Fact]
    public void CanCreateEventTypeBasketball()
    {
        var type = EventType.Basketball;

        type.Id.Should().Be(7522);
    }

    [Fact]
    public void CanCreateEventTypeIceHockey()
    {
        var type = EventType.IceHockey;

        type.Id.Should().Be(7524);
    }

    [Fact]
    public void CanCreateEventTypeAustralianRules()
    {
        var type = EventType.AustralianRules;

        type.Id.Should().Be(61420);
    }

    [Fact]
    public void CanCreateEventTypeHandball()
    {
        var type = EventType.Handball;

        type.Id.Should().Be(468328);
    }

    [Fact]
    public void CanCreateEventTypeDarts()
    {
        var type = EventType.Darts;

        type.Id.Should().Be(3503);
    }

    [Fact]
    public void CanCreateEventTypeMixedMartialArts()
    {
        var type = EventType.MixedMartialArts;

        type.Id.Should().Be(26420387);
    }

    [Fact]
    public void CanCreateEventTypeGreyhoundRacing()
    {
        var type = EventType.GreyhoundRacing;

        type.Id.Should().Be(4339);
    }

    [Fact]
    public void CanCreateEventTypePolitics()
    {
        var type = EventType.Politics;

        type.Id.Should().Be(2378961);
    }

    [Theory]
    [InlineData(999)]
    [InlineData(777)]
    public void CustomEventTypeCanBeCreated(int id)
    {
        var type = EventType.Create(id);

        type.Id.Should().Be(id);
    }
}
