using Betfair.Api.Betting.Enums;

namespace Betfair.Api.Accounts.Endpoints.GetAccountStatement.Enums;

/// <summary>
/// Wallet types.
/// </summary>
[JsonConverter(typeof(SnakeCaseEnumJsonConverter<Wallet>))]
public enum Wallet
{
    /// <summary>
    /// The Global Exchange wallet.
    /// </summary>
    Uk,
}
