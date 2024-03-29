﻿namespace Betfair.Api.Requests;

public sealed class MarketSort
{
    private MarketSort(string value) =>
        Value = value;

    public static MarketSort MinimumTraded =>
        new ("MINIMUM_TRADED");

    public static MarketSort MaximumTraded =>
        new ("MAXIMUM_TRADED");

    public static MarketSort MinimumAvailable =>
        new ("MINIMUM_AVAILABLE");

    public static MarketSort MaximumAvailable =>
        new ("MAXIMUM_AVAILABLE");

    public static MarketSort FirstToStart =>
        new ("FIRST_TO_START");

    public static MarketSort LastToStart =>
        new ("LAST_TO_START");

    public string Value { get; }
}
