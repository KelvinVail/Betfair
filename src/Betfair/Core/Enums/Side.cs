using Betfair.Api.Betting.Enums;

namespace Betfair.Core.Enums;

[JsonConverter(typeof(SnakeCaseEnumJsonConverter<Side>))]
public enum Side
{
    /// <summary>Unknown or not yet set.</summary>
    Unknown = 0,

    /// <summary>Back (buy).</summary>
    Back,

    /// <summary>Lay (sell).</summary>
    Lay,
}
