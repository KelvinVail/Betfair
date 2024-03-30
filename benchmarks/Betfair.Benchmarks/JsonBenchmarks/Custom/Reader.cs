using System.Buffers;
using System.Text.Json;
using Betfair.Benchmarks.JsonBenchmarks.Responses;

namespace Betfair.Benchmarks.JsonBenchmarks.Custom;

public struct Reader
{
    private readonly System.IO.Stream _stream;

    public Reader(System.IO.Stream stream)
    {
        _stream = stream;
    }

    public ChangeMessage Read()
    {
        ChangeMessage result = new ChangeMessage();
        var buffer = ArrayPool<byte>.Shared.Rent(4096);

        try
        {
            int lastReadBytes;

            while ((lastReadBytes = _stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }

        return result;
    }

    public static ChangeMessage Deserialize(byte[] data)
    {
        var result = new ChangeMessage();
        var reader = new Utf8JsonReader(data);
        byte[] op = new [] { (byte)'o', (byte)'p' };

        while (reader.Read())
        {
            if (reader.TokenType != JsonTokenType.PropertyName) continue;
            //if (!reader.ValueTextEquals("op")) continue;
            if (!reader.ValueTextEquals(op)) continue;
            reader.Read();
            result.Operation = reader.GetString();
            return result;
        }
        throw new InvalidOperationException("Unknown stream operation");
    }
}
