using System.Runtime.CompilerServices;

namespace Betfair.Stream.Deserializers;

/// <summary>
/// Optimized property lookup for Betfair stream JSON properties.
/// Uses length-based switching and direct byte comparisons for maximum performance.
/// </summary>
internal static class PropertyLookup
{
    /// <summary>
    /// Optimized property lookup using length-based switching and direct byte comparisons
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static PropertyType GetPropertyType(ReadOnlySpan<byte> propertyName)
    {
        // Fast path for most common properties by length
        return propertyName.Length switch
        {
            2 => GetTwoCharProperty(propertyName),
            3 => GetThreeCharProperty(propertyName),
            4 => GetFourCharProperty(propertyName),
            5 => GetFiveCharProperty(propertyName),
            6 => GetSixCharProperty(propertyName),
            7 => GetSevenCharProperty(propertyName),
            8 => GetEightCharProperty(propertyName),
            9 => GetNineCharProperty(propertyName),
            10 => GetTenCharProperty(propertyName),
            11 => GetElevenCharProperty(propertyName),
            12 => GetTwelveCharProperty(propertyName),
            13 => GetThirteenCharProperty(propertyName),
            14 => GetFourteenCharProperty(propertyName),
            15 => GetFifteenCharProperty(propertyName),
            16 => GetSixteenCharProperty(propertyName),
            17 => GetSeventeenCharProperty(propertyName),
            18 => GetEighteenCharProperty(propertyName),
            19 => GetNineteenCharProperty(propertyName),
            20 => GetTwentyCharProperty(propertyName),
            21 => GetTwentyOneCharProperty(propertyName),
            _ => PropertyType.Unknown
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetTwoCharProperty(ReadOnlySpan<byte> span)
    {
        if (span[0] == 'o' && span[1] == 'p') return PropertyType.Op;
        if (span[0] == 'i' && span[1] == 'd') return PropertyType.Id;
        if (span[0] == 'p' && span[1] == 't') return PropertyType.Pt;
        if (span[0] == 'c' && span[1] == 't') return PropertyType.Ct;
        if (span[0] == 'm' && span[1] == 'c') return PropertyType.Mc;
        if (span[0] == 'o' && span[1] == 'c') return PropertyType.Oc;
        if (span[0] == 't' && span[1] == 'v') return PropertyType.Tv;
        if (span[0] == 'r' && span[1] == 'c') return PropertyType.Rc;
        if (span[0] == 'h' && span[1] == 'c') return PropertyType.Hc;
        if (span[0] == 'm' && span[1] == 'l') return PropertyType.Ml;
        if (span[0] == 'm' && span[1] == 'b') return PropertyType.Mb;
        if (span[0] == 'u' && span[1] == 'o') return PropertyType.Uo;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetThreeCharProperty(ReadOnlySpan<byte> span)
    {
        if (span[0] == 'c' && span[1] == 'l' && span[2] == 'k') return PropertyType.Clk;
        if (span[0] == 'i' && span[1] == 'm' && span[2] == 'g') return PropertyType.Img;
        if (span[0] == 'c' && span[1] == 'o' && span[2] == 'n') return PropertyType.Con;
        if (span[0] == 'l' && span[1] == 't' && span[2] == 'p') return PropertyType.Ltp;
        if (span[0] == 's' && span[1] == 'p' && span[2] == 'f') return PropertyType.Spf;
        if (span[0] == 's' && span[1] == 'p' && span[2] == 'n') return PropertyType.Spn;
        if (span[0] == 's' && span[1] == 'p' && span[2] == 'b') return PropertyType.Spb;
        if (span[0] == 't' && span[1] == 'r' && span[2] == 'd') return PropertyType.Trd;
        if (span[0] == 'a' && span[1] == 't' && span[2] == 'b') return PropertyType.Atb;
        if (span[0] == 's' && span[1] == 'p' && span[2] == 'l') return PropertyType.Spl;
        if (span[0] == 'a' && span[1] == 't' && span[2] == 'l') return PropertyType.Atl;
        if (span[0] == 'b' && span[1] == 's' && span[2] == 'p') return PropertyType.Bsp;
        if (span[0] == 'o' && span[1] == 'r' && span[2] == 'c') return PropertyType.Orc;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetFourCharProperty(ReadOnlySpan<byte> span)
    {
        if (span.SequenceEqual("batb"u8)) return PropertyType.Batb;
        if (span.SequenceEqual("batl"u8)) return PropertyType.Batl;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetFiveCharProperty(ReadOnlySpan<byte> span)
    {
        if (span.SequenceEqual("bdatl"u8)) return PropertyType.Bdatl;
        if (span.SequenceEqual("bdatb"u8)) return PropertyType.Bdatb;
        if (span.SequenceEqual("venue"u8)) return PropertyType.Venue;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetSixCharProperty(ReadOnlySpan<byte> span)
    {
        if (span.SequenceEqual("status"u8)) return PropertyType.Status;
        if (span.SequenceEqual("closed"u8)) return PropertyType.Closed;
        if (span.SequenceEqual("inPlay"u8)) return PropertyType.InPlay;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetSevenCharProperty(ReadOnlySpan<byte> span)
    {
        if (span.SequenceEqual("version"u8)) return PropertyType.Version;
        if (span.SequenceEqual("runners"u8)) return PropertyType.Runners;
        if (span.SequenceEqual("eventId"u8)) return PropertyType.EventId;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetEightCharProperty(ReadOnlySpan<byte> span)
    {
        if (span.SequenceEqual("timezone"u8)) return PropertyType.Timezone;
        if (span.SequenceEqual("openDate"u8)) return PropertyType.OpenDate;
        if (span.SequenceEqual("betDelay"u8)) return PropertyType.BetDelay;
        if (span.SequenceEqual("complete"u8)) return PropertyType.Complete;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetNineCharProperty(ReadOnlySpan<byte> span)
    {
        if (span.SequenceEqual("accountId"u8)) return PropertyType.AccountId;
        if (span.SequenceEqual("bspMarket"u8)) return PropertyType.BspMarket;
        if (span.SequenceEqual("fullImage"u8)) return PropertyType.FullImage;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetTenCharProperty(ReadOnlySpan<byte> span)
    {
        if (span.SequenceEqual("statusCode"u8)) return PropertyType.StatusCode;
        if (span.SequenceEqual("initialClk"u8)) return PropertyType.InitialClk;
        if (span.SequenceEqual("conflateMs"u8)) return PropertyType.ConflateMs;
        if (span.SequenceEqual("marketType"u8)) return PropertyType.MarketType;
        if (span.SequenceEqual("marketTime"u8)) return PropertyType.MarketTime;
        if (span.SequenceEqual("regulators"u8)) return PropertyType.Regulators;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetElevenCharProperty(ReadOnlySpan<byte> span)
    {
        if (span.SequenceEqual("segmentType"u8)) return PropertyType.SegmentType;
        if (span.SequenceEqual("bettingType"u8)) return PropertyType.BettingType;
        if (span.SequenceEqual("heartbeatMs"u8)) return PropertyType.HeartbeatMs;
        if (span.SequenceEqual("settledTime"u8)) return PropertyType.SettledTime;
        if (span.SequenceEqual("suspendTime"u8)) return PropertyType.SuspendTime;
        if (span.SequenceEqual("eventTypeId"u8)) return PropertyType.EventTypeId;
        if (span.SequenceEqual("removalDate"u8)) return PropertyType.RemovalDate;
        if (span.SequenceEqual("countryCode"u8)) return PropertyType.CountryCode;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetTwelveCharProperty(ReadOnlySpan<byte> span)
    {
        if (span.SequenceEqual("connectionId"u8)) return PropertyType.ConnectionId;
        if (span.SequenceEqual("sortPriority"u8)) return PropertyType.SortPriority;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetThirteenCharProperty(ReadOnlySpan<byte> span)
    {
        if (span.SequenceEqual("crossMatching"u8)) return PropertyType.CrossMatching;
        if (span.SequenceEqual("bspReconciled"u8)) return PropertyType.BspReconciled;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetFourteenCharProperty(ReadOnlySpan<byte> span)
    {
        if (span.SequenceEqual("eachWayDivisor"u8)) return PropertyType.EachWayDivisor;
        if (span.SequenceEqual("marketBaseRate"u8)) return PropertyType.MarketBaseRate;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetFifteenCharProperty(ReadOnlySpan<byte> span)
    {
        if (span.SequenceEqual("numberOfWinners"u8)) return PropertyType.NumberOfWinners;
        if (span.SequenceEqual("runnersVoidable"u8)) return PropertyType.RunnersVoidable;
        if (span.SequenceEqual("discountAllowed"u8)) return PropertyType.DiscountAllowed;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetSixteenCharProperty(ReadOnlySpan<byte> span)
    {
        if (span.SequenceEqual("marketDefinition"u8)) return PropertyType.MarketDefinition;
        if (span.SequenceEqual("connectionClosed"u8)) return PropertyType.ConnectionClosed;
        if (span.SequenceEqual("adjustmentFactor"u8)) return PropertyType.AdjustmentFactor;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetSeventeenCharProperty(ReadOnlySpan<byte> span)
    {
        if (span.SequenceEqual("turnInPlayEnabled"u8)) return PropertyType.TurnInPlayEnabled;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetEighteenCharProperty(ReadOnlySpan<byte> span)
    {
        if (span.SequenceEqual("persistenceEnabled"u8)) return PropertyType.PersistenceEnabled;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetNineteenCharProperty(ReadOnlySpan<byte> span)
    {
        if (span.SequenceEqual("connectionsAvailable"u8)) return PropertyType.ConnectionsAvailable;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetTwentyCharProperty(ReadOnlySpan<byte> span)
    {
        if (span.SequenceEqual("numberOfActiveRunners"u8)) return PropertyType.NumberOfActiveRunners;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetTwentyOneCharProperty(ReadOnlySpan<byte> span)
    {
        if (span.SequenceEqual("numberOfActiveRunners"u8)) return PropertyType.NumberOfActiveRunners;
        return PropertyType.Unknown;
    }
}
