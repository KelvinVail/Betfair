using Betfair.Api.Accounts.Endpoints.GetAccountStatement.Enums;
using Betfair.Api.Betting;

namespace Betfair.Api.Accounts.Endpoints.GetAccountStatement.Requests;

internal class AccountStatementRequest
{
    [JsonPropertyName("locale")]
    public string? Locale { get; set; }

    [JsonPropertyName("fromRecord")]
    public int FromRecord { get; set; }

    [JsonPropertyName("recordCount")]
    public int RecordCount { get; set; }

    [JsonPropertyName("itemDateRange")]
    public DateRange? ItemDateRange { get; set; }

    [JsonPropertyName("includeItem")]
    public IncludeItem? IncludeItem { get; set; }

    [JsonPropertyName("wallet")]
    public string? Wallet { get; set; }
}
