namespace Betfair.Core;

public class EventType
{
    private EventType(int id) => Id = id;

    public static EventType AmericanFootball => new(6423);

    public static EventType AustralianRules => new(61420);

    public static EventType Baseball => new(7511);

    public static EventType Basketball => new(7522);

    public static EventType Boxing => new(6);

    public static EventType Cricket => new(4);

    public static EventType Cycling => new(11);

    public static EventType Darts => new(3503);

    public static EventType ESports => new(27454571);

    public static EventType GaelicGames => new(2152880);

    public static EventType Golf => new(3);

    public static EventType GreyhoundRacing => new(4339);

    public static EventType Handball => new(468328);

    public static EventType HorseRacing => new(7);

    public static EventType IceHockey => new(7524);

    public static EventType MixedMartialArts => new(26420387);

    public static EventType MotorSport => new(8);

    public static EventType Politics => new(2378961);

    public static EventType RugbyLeague => new(1477);

    public static EventType RugbyUnion => new(5);

    public static EventType Snooker => new(6422);

    public static EventType Soccer => new(1);

    public static EventType SpecialBets => new(10);

    public static EventType Tennis => new(2);

    public static EventType Volleyball => new(998917);

    public static EventType WinterSports => new(451485);

    public int Id { get; private set; }

    public static EventType Of(int id) => new(id);
}
