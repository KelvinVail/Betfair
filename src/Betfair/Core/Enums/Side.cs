namespace Betfair.Core.Enums;

[JsonConverter(typeof(UpperCaseEnumJsonConverter<Side>))]
public enum Side
{
    Back,
    Lay,
}