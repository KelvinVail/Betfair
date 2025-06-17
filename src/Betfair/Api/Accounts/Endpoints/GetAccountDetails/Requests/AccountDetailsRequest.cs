using System.Text.Json.Serialization;

namespace Betfair.Api.Accounts.Endpoints.GetAccountDetails.Requests;

/// <summary>
/// Request for account details. This is an empty request body.
/// </summary>
#pragma warning disable S2094 // Remove this empty class, write its code or make it an "interface"
internal class AccountDetailsRequest
{
    // Empty request body for account details - the getAccountDetails endpoint requires no parameters
}
#pragma warning restore S2094
