using Betfair.Api.Betting.Enums;

namespace Betfair.Api.Accounts.Endpoints.GetAccountStatement.Enums;

/// <summary>
/// Include item types for account statement.
/// </summary>
[JsonConverter(typeof(SnakeCaseEnumJsonConverter<IncludeItem>))]
public enum IncludeItem
{
    /// <summary>
    /// Include all items.
    /// </summary>
    All,

    /// <summary>
    /// Include payments only.
    /// </summary>
    DepositsWithdrawals,

    /// <summary>
    /// Include exchange bets only.
    /// </summary>
    Exchange,

    /// <summary>
    /// Include poker transactions only.
    /// </summary>
    PokerRoom,
}
