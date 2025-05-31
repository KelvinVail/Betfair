﻿namespace Betfair.Api.Requests;

internal class CountriesRequest
{
    [JsonPropertyName("filter")]
    public ApiMarketFilter Filter { get; set; } = new();
}
