namespace Betfair.Api.Betting.Enums;

[JsonConverter(typeof(SnakeCaseEnumJsonConverter<Side>))]
public enum Side
{
    Back,
    Lay,
}