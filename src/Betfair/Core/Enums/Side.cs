namespace Betfair.Core.Enums;

[JsonConverter(typeof(SnakeCaseEnumJsonConverter<Side>))]
public enum Side
{
    Back,
    Lay,
}