using System.Runtime.CompilerServices;
using System.Text.Json;
using Betfair.Stream.Responses;

namespace Betfair.Stream.Deserializers;

/// <summary>
/// High-performance deserializer for ChangeMessage objects.
/// </summary>
internal static class ChangeMessageDeserializer
{
    /// <summary>
    /// Deserializes a ChangeMessage from JSON bytes using optimized FastJsonReader parsing.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ChangeMessage? Deserialize(ReadOnlySpan<byte> jsonBytes)
    {
        try
        {
            var reader = new FastJsonReader(jsonBytes);
            return ReadChangeMessage(ref reader);
        }
        catch
        {
            // Skip invalid JSON messages
            return null;
        }
    }

    /// <summary>
    /// Reads a ChangeMessage using optimized FastJsonReader parsing.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ChangeMessage? ReadChangeMessage(ref FastJsonReader reader)
    {
        if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
            return null;

        // Collect all properties first
        string? operation = null;
        int id = 0;
        string? initialClock = null;
        string? clock = null;
        long? publishTime = null;
        string? changeType = null;
        string? statusCode = null;
        string? errorCode = null;
        string? connectionId = null;
        bool? connectionClosed = null;
        int? connectionsAvailable = null;
        int? conflateMs = null;
        int? heartbeatMs = null;
        string? segmentType = null;
        List<MarketChange>? marketChanges = null;
        List<OrderChange>? orderChanges = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType == JsonTokenType.String)
            {
                var propertyType = PropertyLookup.GetPropertyType(reader.ValueSpan);
                if (!reader.Read()) // Move to value
                    throw new JsonException("Incomplete JSON: expected value after property name");

                switch (propertyType)
                {
                    case PropertyType.Op:
                        operation = reader.GetString();
                        break;
                    case PropertyType.Id:
                        id = reader.GetInt32();
                        break;
                    case PropertyType.InitialClk:
                        initialClock = reader.GetString();
                        break;
                    case PropertyType.Clk:
                        clock = reader.GetString();
                        break;
                    case PropertyType.Pt:
                        publishTime = reader.GetInt64();
                        break;
                    case PropertyType.Ct:
                        changeType = reader.GetString();
                        break;
                    case PropertyType.StatusCode:
                        statusCode = reader.GetString();
                        break;
                    case PropertyType.ErrorCode:
                        errorCode = reader.GetString();
                        break;
                    case PropertyType.ConnectionId:
                        connectionId = reader.GetString();
                        break;
                    case PropertyType.ConnectionClosed:
                        connectionClosed = reader.GetBoolean();
                        break;
                    case PropertyType.ConnectionsAvailable:
                        connectionsAvailable = reader.GetInt32();
                        break;
                    case PropertyType.ConflateMs:
                        conflateMs = reader.GetInt32();
                        break;
                    case PropertyType.HeartbeatMs:
                        heartbeatMs = reader.GetInt32();
                        break;
                    case PropertyType.SegmentType:
                        segmentType = reader.GetString();
                        break;
                    case PropertyType.Mc:
                        marketChanges = MarketChangeDeserializer.ReadMarketChanges(ref reader);
                        break;
                    case PropertyType.Oc:
                        orderChanges = OrderChangeDeserializer.ReadOrderChanges(ref reader);
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }
        }

        // Create the object with all properties
        return new ChangeMessage
        {
            Operation = operation,
            Id = id,
            InitialClock = initialClock,
            Clock = clock,
            PublishTime = publishTime,
            ChangeType = changeType,
            StatusCode = statusCode,
            ErrorCode = errorCode,
            ConnectionId = connectionId,
            ConnectionClosed = connectionClosed,
            ConnectionsAvailable = connectionsAvailable,
            ConflateMs = conflateMs,
            HeartbeatMs = heartbeatMs,
            SegmentType = segmentType,
            MarketChanges = marketChanges,
            OrderChanges = orderChanges,
        };
    }
}
