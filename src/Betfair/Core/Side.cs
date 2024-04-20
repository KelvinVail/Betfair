namespace Betfair.Core;

[JsonConverter(typeof(UpperCaseEnumJsonConverter<Side>))]
public enum Side
{
    Back,
    Lay,
}