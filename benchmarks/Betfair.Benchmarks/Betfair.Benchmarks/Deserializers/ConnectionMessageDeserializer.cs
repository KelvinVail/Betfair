using System.Text.Json;

namespace Betfair.Benchmarks.Deserializers;

public static class ConnectionMessageDeserializer
{
    public static byte[] ConnectionId(byte[] data)
    {
        var reader = new Utf8JsonReader(data);
        reader.Read();
        reader.Read();
        reader.Read();
        reader.Read();
        reader.Read();
        return reader.ValueSpan.ToArray();
    }
}
