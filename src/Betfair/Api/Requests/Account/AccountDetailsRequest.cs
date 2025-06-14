namespace Betfair.Api.Requests.Account;

/// <summary>
/// Request for account details. This is intentionally empty as the API requires an empty JSON object.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "S2094:Remove this empty class, write its code or make it an \"interface\".", Justification = "Empty class is required for JSON serialization of empty request body")]
internal class AccountDetailsRequest
{
    // Empty request body for account details - required for JSON serialization
}
