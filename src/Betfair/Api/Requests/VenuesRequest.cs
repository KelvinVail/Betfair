﻿namespace Betfair.Api.Requests;

internal class VenuesRequest
{
    [JsonPropertyName("filter")]
    public ApiMarketFilter Filter { get; set; } = new();
}
