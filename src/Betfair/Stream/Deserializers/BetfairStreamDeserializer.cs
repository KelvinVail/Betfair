using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Betfair.Stream.Responses;

namespace Betfair.Stream.Deserializers;

/// <summary>
/// Ultra-high-performance JSON deserializer for Betfair stream data.
/// Optimized for known JSON structure with fixed property order.
/// Performance target: comparable to reading raw bytes from array.
/// </summary>
public sealed class BetfairStreamDeserializer
{
    /// <summary>
    /// Deserializes a ChangeMessage from JSON bytes using ultra-fast direct parsing.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChangeMessage? DeserializeChangeMessage(byte[] lineBytes)
    {
        if (lineBytes == null || lineBytes.Length == 0)
            return null;

        return DeserializeChangeMessage(lineBytes.AsSpan());
    }

    /// <summary>
    /// Deserializes a ChangeMessage from JSON bytes span using ultra-fast direct parsing.
    /// Optimized for the exact structure found in MarketStream.txt.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChangeMessage? DeserializeChangeMessage(ReadOnlySpan<byte> lineSpan)
    {
        // Ultra-fast path: direct byte scanning for known patterns
        return UltraFastChangeMessageDeserializer.Deserialize(lineSpan);
    }
}
