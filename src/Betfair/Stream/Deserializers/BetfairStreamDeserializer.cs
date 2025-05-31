using System.Runtime.CompilerServices;
using Betfair.Stream.Responses;

namespace Betfair.Stream.Deserializers;

/// <summary>
/// High-performance JSON deserializer for Betfair stream data.
/// Coordinates specialized deserializers for optimal performance.
/// </summary>
public sealed class BetfairStreamDeserializer
{
    /// <summary>
    /// Deserializes a ChangeMessage from JSON bytes.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChangeMessage? DeserializeChangeMessage(byte[] lineBytes)
    {
        if (lineBytes == null || lineBytes.Length == 0)
            return null;

        return DeserializeChangeMessage(lineBytes.AsSpan());
    }

    /// <summary>
    /// Deserializes a ChangeMessage from JSON bytes span.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChangeMessage? DeserializeChangeMessage(ReadOnlySpan<byte> lineSpan)
    {
        return ChangeMessageDeserializer.Deserialize(lineSpan);
    }
}
