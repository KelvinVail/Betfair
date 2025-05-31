﻿using Betfair.Api.Requests;

namespace Betfair.Api.Responses;

/// <summary>
/// Time range result.
/// </summary>
public class TimeRangeResult
{
    /// <summary>
    /// Gets the time range.
    /// </summary>
    public DateRange? TimeRange { get; internal set; }

    /// <summary>
    /// Gets the count of markets associated with this time range.
    /// </summary>
    public int MarketCount { get; internal set; }
}
