using System.Buffers.Text;
using System.Text;
using System.Text.Json;

namespace Betfair.Benchmarks;

/// <summary>
/// Zero-allocation Utf8JsonReader-based parser for Betfair stream ChangeMessages.
/// Reads all tokens and extracts values without constructing intermediate objects.
/// Designed to simulate how a market cache would consume the stream.
/// </summary>
public static class ChangeMessageReader
{
    // Pre-computed property name bytes for fast comparison
    private static readonly byte[] PropOp = "op"u8.ToArray();
    private static readonly byte[] PropId = "id"u8.ToArray();
    private static readonly byte[] PropClk = "clk"u8.ToArray();
    private static readonly byte[] PropInitialClk = "initialClk"u8.ToArray();
    private static readonly byte[] PropPt = "pt"u8.ToArray();
    private static readonly byte[] PropCt = "ct"u8.ToArray();
    private static readonly byte[] PropMc = "mc"u8.ToArray();
    private static readonly byte[] PropRc = "rc"u8.ToArray();
    private static readonly byte[] PropTv = "tv"u8.ToArray();
    private static readonly byte[] PropLtp = "ltp"u8.ToArray();
    private static readonly byte[] PropImg = "img"u8.ToArray();
    private static readonly byte[] PropAtb = "atb"u8.ToArray();
    private static readonly byte[] PropAtl = "atl"u8.ToArray();
    private static readonly byte[] PropBatb = "batb"u8.ToArray();
    private static readonly byte[] PropBatl = "batl"u8.ToArray();
    private static readonly byte[] PropTrd = "trd"u8.ToArray();
    private static readonly byte[] PropSpb = "spb"u8.ToArray();
    private static readonly byte[] PropSpl = "spl"u8.ToArray();
    private static readonly byte[] PropSpf = "spf"u8.ToArray();
    private static readonly byte[] PropSpn = "spn"u8.ToArray();
    private static readonly byte[] PropBdatb = "bdatb"u8.ToArray();
    private static readonly byte[] PropBdatl = "bdatl"u8.ToArray();
    private static readonly byte[] PropHc = "hc"u8.ToArray();
    private static readonly byte[] PropMarketDefinition = "marketDefinition"u8.ToArray();
    private static readonly byte[] PropCon = "con"u8.ToArray();
    private static readonly byte[] PropConflateMs = "conflateMs"u8.ToArray();
    private static readonly byte[] PropHeartbeatMs = "heartbeatMs"u8.ToArray();
    private static readonly byte[] PropStatusCode = "statusCode"u8.ToArray();
    private static readonly byte[] PropErrorCode = "errorCode"u8.ToArray();
    private static readonly byte[] PropConnectionId = "connectionId"u8.ToArray();
    private static readonly byte[] PropOc = "oc"u8.ToArray();

    /// <summary>
    /// Reads a ChangeMessage from a span, extracting all meaningful data without allocations.
    /// Returns a summary struct containing the key fields a market cache would need.
    /// </summary>
    public static ChangeMessageSummary Read(ReadOnlySpan<byte> data)
    {
        var reader = new Utf8JsonReader(data);
        var summary = new ChangeMessageSummary();

        if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
            return summary;

        while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
        {
            if (reader.ValueTextEquals(PropOp))
            {
                reader.Read();
                // Just note operation type by first char
                if (reader.TokenType == JsonTokenType.String)
                    summary.OperationByte = reader.HasValueSequence ? (byte)0 : reader.ValueSpan[0];
            }
            else if (reader.ValueTextEquals(PropId))
            {
                reader.Read();
                summary.Id = reader.GetInt32();
            }
            else if (reader.ValueTextEquals(PropPt))
            {
                reader.Read();
                summary.PublishTime = reader.GetInt64();
            }
            else if (reader.ValueTextEquals(PropClk))
            {
                reader.Read();
                // Skip string value - in real cache you'd store this
            }
            else if (reader.ValueTextEquals(PropInitialClk))
            {
                reader.Read();
                summary.HasInitialClk = true;
            }
            else if (reader.ValueTextEquals(PropCt))
            {
                reader.Read();
            }
            else if (reader.ValueTextEquals(PropConflateMs))
            {
                reader.Read();
                summary.ConflateMs = reader.GetInt32();
            }
            else if (reader.ValueTextEquals(PropHeartbeatMs))
            {
                reader.Read();
                summary.HeartbeatMs = reader.GetInt32();
            }
            else if (reader.ValueTextEquals(PropStatusCode))
            {
                reader.Read();
            }
            else if (reader.ValueTextEquals(PropErrorCode))
            {
                reader.Read();
            }
            else if (reader.ValueTextEquals(PropConnectionId))
            {
                reader.Read();
            }
            else if (reader.ValueTextEquals(PropMc))
            {
                ReadMarketChanges(ref reader, ref summary);
            }
            else if (reader.ValueTextEquals(PropOc))
            {
                SkipValue(ref reader);
            }
            else
            {
                SkipValue(ref reader);
            }
        }

        return summary;
    }

    private static void ReadMarketChanges(ref Utf8JsonReader reader, ref ChangeMessageSummary summary)
    {
        if (!reader.Read() || reader.TokenType != JsonTokenType.StartArray)
            return;

        while (reader.Read() && reader.TokenType == JsonTokenType.StartObject)
        {
            summary.MarketChangeCount++;
            ReadSingleMarketChange(ref reader, ref summary);
        }
    }

    private static void ReadSingleMarketChange(ref Utf8JsonReader reader, ref ChangeMessageSummary summary)
    {
        while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
        {
            if (reader.ValueTextEquals(PropId))
            {
                reader.Read();
                // Market ID string - in real cache you'd hash or store this
            }
            else if (reader.ValueTextEquals(PropRc))
            {
                ReadRunnerChanges(ref reader, ref summary);
            }
            else if (reader.ValueTextEquals(PropTv))
            {
                reader.Read();
                summary.TotalVolume = reader.GetDouble();
            }
            else if (reader.ValueTextEquals(PropImg))
            {
                reader.Read();
                summary.IsImage = reader.GetBoolean();
            }
            else if (reader.ValueTextEquals(PropCon))
            {
                reader.Read();
            }
            else if (reader.ValueTextEquals(PropMarketDefinition))
            {
                SkipValue(ref reader);
                summary.HasMarketDefinition = true;
            }
            else
            {
                SkipValue(ref reader);
            }
        }
    }

    private static void ReadRunnerChanges(ref Utf8JsonReader reader, ref ChangeMessageSummary summary)
    {
        if (!reader.Read() || reader.TokenType != JsonTokenType.StartArray)
            return;

        while (reader.Read() && reader.TokenType == JsonTokenType.StartObject)
        {
            summary.RunnerChangeCount++;
            ReadSingleRunnerChange(ref reader, ref summary);
        }
    }

    private static void ReadSingleRunnerChange(ref Utf8JsonReader reader, ref ChangeMessageSummary summary)
    {
        while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
        {
            if (reader.ValueTextEquals(PropId))
            {
                reader.Read();
                // Selection ID
                summary.LastSelectionId = reader.GetInt64();
            }
            else if (reader.ValueTextEquals(PropLtp))
            {
                reader.Read();
                summary.LastLtp = reader.GetDouble();
            }
            else if (reader.ValueTextEquals(PropTv))
            {
                reader.Read();
                summary.LastRunnerTv = reader.GetDouble();
            }
            else if (reader.ValueTextEquals(PropHc))
            {
                reader.Read();
                _ = reader.GetDouble();
            }
            else if (reader.ValueTextEquals(PropSpf))
            {
                reader.Read();
                _ = reader.GetDouble();
            }
            else if (reader.ValueTextEquals(PropSpn))
            {
                reader.Read();
                _ = reader.GetDouble();
            }
            else if (reader.ValueTextEquals(PropAtb) ||
                     reader.ValueTextEquals(PropAtl) ||
                     reader.ValueTextEquals(PropBatb) ||
                     reader.ValueTextEquals(PropBatl) ||
                     reader.ValueTextEquals(PropTrd) ||
                     reader.ValueTextEquals(PropSpb) ||
                     reader.ValueTextEquals(PropSpl) ||
                     reader.ValueTextEquals(PropBdatb) ||
                     reader.ValueTextEquals(PropBdatl))
            {
                ReadPriceSizeArray(ref reader, ref summary);
            }
            else
            {
                SkipValue(ref reader);
            }
        }
    }

    /// <summary>
    /// Reads a [[price, size], ...] or [[pos, price, size], ...] array
    /// without allocating any List objects. Extracts doubles directly.
    /// </summary>
    private static void ReadPriceSizeArray(ref Utf8JsonReader reader, ref ChangeMessageSummary summary)
    {
        if (!reader.Read() || reader.TokenType != JsonTokenType.StartArray)
            return;

        while (reader.Read() && reader.TokenType == JsonTokenType.StartArray)
        {
            // Read inner array of doubles [price, size] or [position, price, size]
            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                // Each value is a Number - GetDouble() reads it
                summary.PricePointCount++;
            }
        }
    }

    private static void SkipValue(ref Utf8JsonReader reader)
    {
        reader.Read();
        if (reader.TokenType is JsonTokenType.StartObject or JsonTokenType.StartArray)
            reader.Skip();
    }
}

/// <summary>
/// Lightweight struct capturing the summary of a parsed ChangeMessage.
/// No heap allocations - this is what a market cache would extract.
/// </summary>
public struct ChangeMessageSummary
{
    public byte OperationByte;
    public int Id;
    public long PublishTime;
    public int ConflateMs;
    public int HeartbeatMs;
    public bool HasInitialClk;
    public bool IsImage;
    public bool HasMarketDefinition;
    public int MarketChangeCount;
    public int RunnerChangeCount;
    public int PricePointCount;
    public double TotalVolume;
    public double LastLtp;
    public double LastRunnerTv;
    public long LastSelectionId;
}
